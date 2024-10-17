using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadarCameraController : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<float> _cameraEulerAngle;
    private Vector3 _newPosition;
    private Transform _cachedTransform;

    // Start is called before the first frame update
    void Start()
    {
        _cachedTransform = transform;
        _newPosition = _cachedTransform.position;
    }

    public void UpdateRadarCameraTransform(Vector2 playerPosition, float horizontalRotation)
    {
        _newPosition.x = playerPosition.x;
        _newPosition.z = playerPosition.y;
        _cachedTransform.SetPositionAndRotation(_newPosition, Quaternion.Euler(90, horizontalRotation, 0f));
        _cameraEulerAngle.Invoke(horizontalRotation);
    }
}
