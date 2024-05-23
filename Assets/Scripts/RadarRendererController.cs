using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarRendererController : MonoBehaviour
{
    [SerializeField]
    private Transform directionsTransform;

    public void UpdateRadarRotation(float horizontalRotation)
    {
        directionsTransform.rotation = Quaternion.Euler(0, 0, horizontalRotation);
    }
}
