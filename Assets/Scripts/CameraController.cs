using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    private readonly struct CameraMode
    {
        public Vector3 TrailingDistance { get; }
        public bool IsBehindPlane { get; }

        public CameraMode(Vector3 distance)
        {
            TrailingDistance = distance;
            IsBehindPlane = distance.z < 0;
        }
    }

    [SerializeField]
    private Transform _playerTransform;
    [SerializeField]
    private Vector3 _crashDistance;
    [SerializeField]
    private float _smoothSpeed;
    [SerializeField]
    private UnityEvent<bool> _waterFilter;

    private Vector3 _distanceToTarget;
    private Transform _cameraTransform;
    private IEnumerator _transitionRoutine;
    private IEnumerator _rotationRoutine;
    private IEnumerator _cameraModeTransition;
    private IEnumerator _lookAtCrashSite;
    private bool _transitionChangeInProgress;
    private bool _rotationChangeInProgress;
    private bool _cameraModeChangeInProgress;
    private bool _closeToSurface;
    private FrameRule _checkGroundDistanceRule;
    private int _surfacesLayerMask;
    private Vector3 _followPoint;
    private CameraMode[] _cameraModes;
    private int _cameraIndex;
    private bool _verticalFollowLocked;
    private bool _playerAirbourne;
    private float _rotationSpeed;
    private bool _trailingBehind;
    private bool _airToSurfaceTransition;
    private bool _surfaceToAirTransition;
    private bool _rotationLocked;
    private bool _belowWater;
    private bool _followingPlayer;
    private Quaternion _newRotation;
    private Vector3 _nextPosition;
    private RaycastHit _surfaceHit;


    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = transform;
        _surfacesLayerMask = LayerMask.GetMask(Constants.BuildingLayerName, Constants.WaterLayerName);
        _checkGroundDistanceRule = new(Constants.CameraSurfaceCheckInterval);

        _cameraModes = new CameraMode[Constants.CameraModesCount];
        _cameraModes[0] = new(Constants.CameraTrailingDistanceBehindAbove);
        _cameraModes[1] = new(Constants.CameraTrailingDistanceBehindCentered);
        _cameraModes[2] = new(Constants.CameraTrailingDistancePropeller);
        _cameraModes[3] = new(Constants.CameraTrailingDistanceFirstPerson);
        ResetCamera();
    }

    private void ResetCamera()
    {
        _transitionChangeInProgress = false;
        _rotationChangeInProgress = false;
        _cameraModeChangeInProgress = false;
        _airToSurfaceTransition = false;
        _surfaceToAirTransition = false;
        _rotationLocked = false;
        _closeToSurface = true;
        _belowWater = false;
        _followingPlayer = true;
        _cameraIndex = 0;
        _distanceToTarget = _cameraModes[_cameraIndex].TrailingDistance;
        _verticalFollowLocked = false;
        _rotationSpeed = Constants.CameraRotationSpeedDefault;
        _trailingBehind = _cameraModes[_cameraIndex].IsBehindPlane;
        _newRotation = _playerTransform.rotation;
        _nextPosition = _playerTransform.position;
    }

    private void Update()
    {
        if (_followingPlayer)
        {
            if (_trailingBehind)    // Camera is behind the plane
            {
                _closeToSurface = Physics.Raycast(_cameraTransform.position, Vector3.down, out _surfaceHit, Constants.CameraSpecialModeHeightThreshold, _surfacesLayerMask);                
                if (_playerAirbourne && _checkGroundDistanceRule.CheckFrameRule() && _closeToSurface)
                {   // Camera is too close to surface
                    if ((_cameraTransform.position.y - _surfaceHit.point.y) < Constants.CameraUnderwaterEffectThreshold && !_belowWater)
                    {
                        _belowWater = true;
                        _waterFilter.Invoke(_belowWater);
                    }
                    else if ((_cameraTransform.position.y - _surfaceHit.point.y) > Constants.CameraUnderwaterEffectThreshold && _belowWater)
                    {
                        _belowWater = false;
                        _waterFilter.Invoke(_belowWater);
                    }

                    if (!_rotationLocked && HelperMethods.GetSignedAngleFromEuler(_cameraTransform.eulerAngles.x) < Constants.CameraImpactAngleThreshold)
                    {   // Plane cannot maintain altitude due to pitch and is about to crash
                        _rotationLocked = true;
                        StopRotationIfActive();
                        StopTransitionIfActive();
                        _cameraModeTransition = ImpactViewTransition();
                        StartCoroutine(_cameraModeTransition);
                    }
                    else if (!_rotationLocked && !_airToSurfaceTransition && !_surfaceToAirTransition)
                    {   // Activating "close to surface" camera mode
                        _airToSurfaceTransition = true;
                        StopTransitionIfActive();
                        _cameraModeTransition = AirToSurfaceViewTransition();
                        StartCoroutine(_cameraModeTransition);
                    }
                    else if (_airToSurfaceTransition && !_surfaceToAirTransition &&
                        (_playerTransform.position.y - _cameraTransform.position.y) > Constants.CameraSurfaceToAirHeightTrigger)
                    {   // Going back to regular camera mode after plane gains enough altitude above surface
                        StopCoroutine(_cameraModeTransition);
                        StopRotationIfActive();
                        StopTransitionIfActive();
                        _airToSurfaceTransition = false;
                        _surfaceToAirTransition = true;
                        _cameraModeTransition = SurfaceToAirViewTransition();
                        StartCoroutine(_cameraModeTransition);
                    }
                }
                UpdateCameraPositionBehindPlayer();
                _checkGroundDistanceRule.AdvanceCounter();
                if (!_rotationLocked)
                {
                    _newRotation.eulerAngles = new Vector3(_playerTransform.eulerAngles.x, _playerTransform.eulerAngles.y, 0);
                    _cameraTransform.rotation = Quaternion.Slerp(_cameraTransform.rotation, _newRotation, _rotationSpeed * Time.deltaTime);
                }
            }
            else // Camera is in front of the plane
            {
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _playerTransform.TransformPoint(_distanceToTarget),
                    _smoothSpeed * Time.deltaTime);
                _cameraTransform.eulerAngles = _playerTransform.eulerAngles;
            }
        }
    }

    public void SetAirbourneFlag(bool flag)
    {
        _playerAirbourne = flag;
    }

    public void SetCameraMode(float next)
    {
        if (_transitionChangeInProgress || _rotationChangeInProgress || _airToSurfaceTransition || _surfaceToAirTransition ||
            _closeToSurface) return;
        int previousIndex = _cameraIndex;
        int newIndex = _cameraIndex + (int)next;
        _cameraIndex = (newIndex < 0) ? _cameraModes.Count() - 1 : ((newIndex >= _cameraModes.Count()) ? 0 : newIndex);

        if (_cameraModes[_cameraIndex].IsBehindPlane == _cameraModes[previousIndex].IsBehindPlane) 
        {
            if (_cameraModes[previousIndex].IsBehindPlane)
            {
                ChangeCameraPosition(_cameraModes[_cameraIndex].TrailingDistance);
            }
            else
            {
                _distanceToTarget = _cameraModes[_cameraIndex].TrailingDistance;
            }
        }
        else
        {
            if (_cameraModes[previousIndex].IsBehindPlane)
            {
                _cameraModeTransition = CameraModeTransitionBehindToFront();
                StartCoroutine(_cameraModeTransition);
            }
            else
            {
                _cameraModeTransition = (CameraModeTransitionFrontToBehind());
                StartCoroutine(_cameraModeTransition);
            }
        }
    }

    public void AttachToPlayer()
    {
        if (!_followingPlayer)
        {
            StopCoroutine(_lookAtCrashSite);
            _followingPlayer = true;
        }
        ResetCamera();
        UpdateCameraPositionBehindPlayer();
    }

    public void DetachFromPlayer(Vector3 crashLocation)
    {
        StopTransitionIfActive();
        StopRotationIfActive();
        if (_cameraModeChangeInProgress) StopCoroutine(_cameraModeTransition);
        _followingPlayer = false;
        _distanceToTarget = _cameraTransform.position + Constants.CameraCrashDistance;
        _lookAtCrashSite = LookAtCrashSite(crashLocation);
        StartCoroutine(_lookAtCrashSite);
    }

    public void ChangeCameraPosition(Vector3 newDistance)
    {
        _transitionRoutine = TransitionTrailingDistance(newDistance, Constants.CameraTransitionSpeedDefault);
        StartCoroutine(_transitionRoutine);
    }

    public void InstantRotateTowardsPlayerY()
    {
        if (_airToSurfaceTransition || _surfaceToAirTransition)
        {
            Vector3 newEulerAngles = _cameraTransform.eulerAngles;
            newEulerAngles.y -= _playerTransform.eulerAngles.y;
            _cameraTransform.Rotate(newEulerAngles);
        }
    }

    private void UpdateCameraPositionBehindPlayer()
    {
        _nextPosition.x = _playerTransform.position.x;
        _nextPosition.z = _playerTransform.position.z;
        if (!_verticalFollowLocked) _nextPosition.y = _playerTransform.position.y;
        _followPoint = _nextPosition + (_distanceToTarget.y * Vector3.up);  // A point above the player
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _followPoint + (_playerTransform.forward * _distanceToTarget.z),
            _smoothSpeed * Time.deltaTime);
    }

    private IEnumerator SlowRotationTransition(Vector3 angle, float speed)
    {
        if (_rotationChangeInProgress) yield break; // Only one rotation transition should run at a time
        _rotationChangeInProgress = true;
        _newRotation.eulerAngles = angle;
        while (_followingPlayer && Vector3.Distance(_cameraTransform.eulerAngles, angle) > 0.1f)
        {
            _cameraTransform.rotation = Quaternion.RotateTowards(_cameraTransform.rotation, _newRotation, speed * Time.deltaTime);
            yield return null;
        }
        _rotationChangeInProgress = false;
    }

    private IEnumerator SlowRotationTowardsPLayerTransition(float speed)
    {
        if (_rotationChangeInProgress) yield break; // Only one rotation transition should run at a time
        _rotationChangeInProgress = true;
        while (_followingPlayer && Vector3.Distance(_cameraTransform.eulerAngles, _playerTransform.eulerAngles) > 0.1f)
        {
            _newRotation.eulerAngles = _playerTransform.eulerAngles;
            _cameraTransform.rotation = Quaternion.RotateTowards(_cameraTransform.rotation, _newRotation, speed * Time.deltaTime);
            yield return null;
        }
        _rotationChangeInProgress = false;
    }

    private void StopRotationIfActive()
    {
        if (_rotationChangeInProgress)
        {
            StopCoroutine(_rotationRoutine);
            _rotationChangeInProgress = false;
        }
    }

    private IEnumerator TransitionTrailingDistance(Vector3 newDistance, float speed)
    {
        if (_transitionChangeInProgress) yield break;   // Only one trailing transition should run at a time
        _transitionChangeInProgress = true;
        while (Vector3.Distance(_distanceToTarget, newDistance) > 0.001f)
        {
            _distanceToTarget = Vector3.MoveTowards(_distanceToTarget, newDistance, speed * Time.deltaTime);
            yield return null;
        }
        _transitionChangeInProgress = false;
    }

    private void StopTransitionIfActive()
    {
        if (_transitionChangeInProgress)
        {
            StopCoroutine(_transitionRoutine);
            _transitionChangeInProgress = false;
        }
    }

    private IEnumerator LookAtCrashSite(Vector3 followPosition)
    {
        while (_cameraTransform.position != _distanceToTarget && followPosition != null)
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _distanceToTarget,
                Constants.CameraTransitionSpeedGameOver * Time.deltaTime);
            _cameraTransform.LookAt(followPosition);
            yield return null;
        }
        _distanceToTarget = Constants.CameraTrailingDistanceBehindAbove;
    }

    private IEnumerator AirToSurfaceViewTransition()
    {
        _cameraModeChangeInProgress = true;
        _verticalFollowLocked = true;
        _rotationLocked = true;

        Vector3 newAngle = new(Constants.CameraRotationSpeedAirToSurface, _cameraTransform.eulerAngles.y , 0);
        _rotationRoutine = SlowRotationTransition(newAngle, Constants.CameraRotationSpeedAirToSurface);
        StartCoroutine(_rotationRoutine);

        _transitionRoutine = TransitionTrailingDistance(Constants.CameraTrailingDistanceSurface, Constants.CameraTransitionSpeedAirToSurface);
        StartCoroutine(_transitionRoutine);

        yield return new WaitUntil(() => !_transitionChangeInProgress && !_rotationChangeInProgress);
        _cameraModeChangeInProgress = false;
    }

    private IEnumerator SurfaceToAirViewTransition()
    {
        _cameraModeChangeInProgress = true;
        _surfaceToAirTransition = true;
        _verticalFollowLocked = false;

        _rotationRoutine = SlowRotationTowardsPLayerTransition(Constants.CameraRotationSpeedSurfaceToAir);
        yield return StartCoroutine(_rotationRoutine);
        _rotationLocked = false;

        _transitionRoutine = TransitionTrailingDistance(_cameraModes[_cameraIndex].TrailingDistance,
            Constants.CameraTransitionSpeedSurfaceToAir);
        yield return StartCoroutine(_transitionRoutine);

        _surfaceToAirTransition = false;
        _trailingBehind = true;
        _cameraModeChangeInProgress = false;
        _closeToSurface = false;
    }

    private IEnumerator ImpactViewTransition()
    {
        _cameraModeChangeInProgress = true;
        _transitionRoutine = TransitionTrailingDistance(Constants.CameraTrailingDistanceImpact, Constants.CameraTransitionSpeedImpact);
        StartCoroutine(_transitionRoutine);

        _rotationRoutine = SlowRotationTransition(Constants.CameraImpactViewAngleX * Vector3.right, Constants.CameraRotationSpeedImpact);
        StartCoroutine(_rotationRoutine);
        yield return new WaitUntil(() => !_transitionChangeInProgress && !_rotationChangeInProgress);
        _cameraModeChangeInProgress = false;
    }

    private IEnumerator CameraModeTransitionBehindToFront()
    {
        _cameraModeChangeInProgress = true;
        _transitionRoutine = TransitionTrailingDistance(Constants.CameraModeTransitionPoint, Constants.CameraTransitionSpeedFrontBack);
        yield return StartCoroutine(_transitionRoutine);
        _distanceToTarget = _cameraModes[_cameraIndex].TrailingDistance;
        _trailingBehind = false;
        _cameraModeChangeInProgress = false;
    }

    private IEnumerator CameraModeTransitionFrontToBehind()
    {
        _cameraModeChangeInProgress = true;
        _distanceToTarget = Constants.CameraModeTransitionPoint;
        _transitionRoutine = TransitionTrailingDistance(_cameraModes[_cameraIndex].TrailingDistance, Constants.CameraTransitionSpeedFrontBack);
        StartCoroutine(_transitionRoutine);
        _trailingBehind = true;
        _cameraModeChangeInProgress = false;
        yield return null;
    }

}
