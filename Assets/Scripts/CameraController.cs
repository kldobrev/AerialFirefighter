using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTransform;
    [SerializeField]
    private Vector3 _crashDistance;
    [SerializeField]
    private float _smoothSpeed;

    public Vector3 _targetPosition;
    private Transform _cameraTrns;
    private IEnumerator _trailingTransition;
    private static bool _trailingChangeInProgress;

    public float temp_DistanceToGround;
    private FrameRule _checkGroundDistanceRule;
    private int _surfacesLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        _cameraTrns = transform;
        _targetPosition = Constants.CameraTrailingDistanceSurface;
        _cameraTrns.position = _playerTransform.position + Constants.CameraTrailingDistanceSurface;
        _trailingChangeInProgress = false;

        _surfacesLayerMask = LayerMask.GetMask(Constants.GroundLayerName, Constants.BuildingLayerName, Constants.WaterLayerName);
        _checkGroundDistanceRule = new(20);
        StartCoroutine(ScanForSurface());
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTransform != null)
        {
            _cameraTrns.position = Vector3.Lerp(_cameraTrns.position, _playerTransform.TransformPoint(_targetPosition), 
                _smoothSpeed);
            _cameraTrns.LookAt(_playerTransform);
        }
    }

    public void StopFollowingPlayer(Vector3 crashLocation)
    {
        _playerTransform = null;
        if (_trailingChangeInProgress)
        {
            StopCoroutine(_trailingTransition);
            _trailingChangeInProgress = false;
        }
        _targetPosition = _cameraTrns.position + Constants.CameraCrashDistance;
        StartCoroutine(TransitionCamera(crashLocation));
    }

    public void ChangeCameraPosition(Vector3 newDistance)
    {
        _trailingTransition = TransitionTrailingDistance(newDistance);
        StartCoroutine(_trailingTransition);
    }

    private IEnumerator TransitionTrailingDistance(Vector3 newDistance)
    {
        _trailingChangeInProgress = true;
        while (_targetPosition != newDistance)
        {
            _targetPosition = Vector3.Lerp(_targetPosition, newDistance, 
                Constants.CameraTransitionSpeed);
            yield return null;
        }
        _trailingChangeInProgress = false;
    }

    private IEnumerator TransitionCamera(Vector3 followPosition)
    {
        while (_cameraTrns.position != _targetPosition && followPosition != null)
        {
            _cameraTrns.position = Vector3.Lerp(_cameraTrns.position, _targetPosition,
                Constants.CameraTransitionSpeed);
            _cameraTrns.LookAt(followPosition);
            yield return null;
        }
        _targetPosition = Constants.CameraTrailingDistanceDefault;
    }

    private IEnumerator ScanForSurface()
    {
        while (!_playerTransform.IsUnityNull())
        {
            Debug.DrawRay(_playerTransform.position, Vector3.down * Constants.DistanceToGroundCameraTrigger, Color.red);

            if (_checkGroundDistanceRule.CheckFrameRule())
            {
                if (Physics.Raycast(_playerTransform.position, Vector3.down, Constants.DistanceToGroundCameraTrigger, _surfacesLayerMask))
                {
                    if (_targetPosition != Constants.CameraTrailingDistanceSurface && !_trailingChangeInProgress) {
                        ChangeCameraPosition(Constants.CameraTrailingDistanceSurface);
                    }
                }
                else if (_targetPosition != Constants.CameraTrailingDistanceDefault && !_trailingChangeInProgress)
                {
                    ChangeCameraPosition(Constants.CameraTrailingDistanceDefault);
                }
            }
            _checkGroundDistanceRule.AdvanceCounter();
            yield return null;
        }
    }

}
