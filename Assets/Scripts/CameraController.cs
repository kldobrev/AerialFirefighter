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

    private Vector3 _targetPosition;
    private Transform _cameraTrns;
    private IEnumerator _trailingTransition;
    private static bool _trailingChangeInProgress;


    // Start is called before the first frame update
    void Start()
    {
        _cameraTrns = transform;
        _targetPosition = Constants.CameraTrailingDistanceDefault;
        _cameraTrns.position = _playerTransform.position + Constants.CameraTrailingDistanceDefault;
        _trailingChangeInProgress = false;
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

}
