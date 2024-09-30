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
    private Transform cameraTrns;
    private IEnumerator trailingTransition;
    private static bool trailingChangeInProgress;


    // Start is called before the first frame update
    void Start()
    {
        cameraTrns = transform;
        targetPosition = Constants.CameraTrailingDistanceDefault;
        cameraTrns.position = playerTransform.position + Constants.CameraTrailingDistanceDefault;
        trailingChangeInProgress = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            cameraTrns.position = Vector3.Lerp(cameraTrns.position, playerTransform.TransformPoint(targetPosition), 
                smoothSpeed);
            cameraTrns.LookAt(playerTransform);
        }
    }

    public void StopFollowingPlayer(Vector3 crashLocation)
    {
        playerTransform = null;
        if (trailingChangeInProgress)
        {
            StopCoroutine(trailingTransition);
            trailingChangeInProgress = false;
        }
        targetPosition = cameraTrns.position + Constants.CameraCrashDistance;
        StartCoroutine(TransitionCamera(crashLocation));
    }

    public void ChangeCameraPosition(Vector3 newDistance)
    {
        trailingTransition = TransitionTrailingDistance(newDistance);
        StartCoroutine(trailingTransition);
    }

    private IEnumerator TransitionTrailingDistance(Vector3 newDistance)
    {
        trailingChangeInProgress = true;
        while (targetPosition != newDistance)
        {
            targetPosition = Vector3.Lerp(targetPosition, newDistance, 
                Constants.CameraTransitionSpeed);
            yield return null;
        }
        trailingChangeInProgress = false;
    }

    private IEnumerator TransitionCamera(Vector3 followPosition)
    {
        while (cameraTrns.position != targetPosition && followPosition != null)
        {
            cameraTrns.position = Vector3.Lerp(cameraTrns.position, targetPosition,
                Constants.CameraTransitionSpeed);
            cameraTrns.LookAt(followPosition);
            yield return null;
        }
        targetPosition = Constants.CameraTrailingDistanceDefault;
    }

}
