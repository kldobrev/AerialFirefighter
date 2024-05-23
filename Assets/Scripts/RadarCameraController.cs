using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadarCameraController : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<float> cameraEulerAngle;
    private Vector3 newPosition;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
    }

    public void UpdateRadarCameraTransform(Vector2 playerPosition, float horizontalRotation)
    {
        newPosition.x = playerPosition.x;
        newPosition.z = playerPosition.y;
        transform.SetPositionAndRotation(newPosition, Quaternion.Euler(90, horizontalRotation, 0f));
        cameraEulerAngle.Invoke(horizontalRotation);
    }
}
