using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Vector3 trailingDistance;
    [SerializeField]
    private Vector3 crashDistance;
    [SerializeField]
    private float smoothSpeed;
    [SerializeField]
    private float crashSmoothSpeed;

    private Vector3 crashPosition;
    private Vector3 crashTargetPosition;


    // Start is called before the first frame update
    void Start()
    {
        crashTargetPosition = Vector3.zero;
        crashPosition = Vector3.zero;
        transform.position = playerTransform.position + trailingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, playerTransform.TransformPoint(trailingDistance), smoothSpeed);
            transform.LookAt(playerTransform);
        }
    }

    public void StopFollowingPlayer(Vector3 crashLocation)
    {
        crashPosition = crashLocation;
        playerTransform = null;
        crashTargetPosition = transform.position + crashDistance;
        StartCoroutine(DistanceCamera());
    }

    private IEnumerator DistanceCamera()
    {
        while (transform.position != crashTargetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, crashTargetPosition, crashSmoothSpeed);
            transform.LookAt(crashPosition);
            yield return null;
        }
    }

}
