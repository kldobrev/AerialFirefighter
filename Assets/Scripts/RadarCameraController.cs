using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadarCameraController : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<float> cameraEulerAngle;
    private Vector3 newPosition;
    private Transform cachedTrns;

    // Start is called before the first frame update
    void Start()
    {
        cachedTrns = transform;
        newPosition = cachedTrns.position;
    }

    public void UpdateRadarCameraTransform(Vector2 playerPosition, float horizontalRotation)
    {
        newPosition.x = playerPosition.x;
        newPosition.z = playerPosition.y;
        cachedTrns.SetPositionAndRotation(newPosition, Quaternion.Euler(90, horizontalRotation, 0f));
        cameraEulerAngle.Invoke(horizontalRotation);
    }
}
