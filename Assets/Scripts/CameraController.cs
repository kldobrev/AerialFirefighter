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
    private float smoothSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerTransform.position + trailingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, playerTransform.TransformPoint(trailingDistance), smoothSpeed);
            transform.LookAt(playerTransform);
        }
    }

}
