using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Vector3 crashDistance;
    [SerializeField]
    private float smoothSpeed;

    private Vector3 targetPosition;


    // Start is called before the first frame update
    void Start()
    {
        targetPosition = Constants.CameraTrailingDistanceDefault;
        transform.position = playerTransform.position + Constants.CameraTrailingDistanceDefault;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, playerTransform.TransformPoint(targetPosition), 
                smoothSpeed);
            transform.LookAt(playerTransform);
        }
    }

    public void StopFollowingPlayer(Vector3 crashLocation)
    {
        playerTransform = null;
        targetPosition = transform.position + Constants.CameraCrashDistance;
        StartCoroutine(TransitionCamera(crashLocation));
    }

    public void ChangeCameraPosition(Vector3 newDistance)
    {
        StartCoroutine(TransitionTrailingDistance(newDistance));
    }

    private IEnumerator TransitionTrailingDistance(Vector3 newDistance)
    {
        while (targetPosition != newDistance)
        {
            targetPosition = Vector3.Lerp(targetPosition, newDistance, 
                Constants.CameraTransitionSpeed);
            yield return null;
        }
    }

    private IEnumerator TransitionCamera(Vector3 followPosition)
    {
        while (transform.position != targetPosition && followPosition != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition,
                Constants.CameraTransitionSpeed);
            transform.LookAt(followPosition);
            yield return null;
        }
        targetPosition = Constants.CameraTrailingDistanceDefault;
    }

}
