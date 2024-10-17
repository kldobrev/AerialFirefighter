using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarRendererController : MonoBehaviour
{
    [SerializeField]
    private Transform _directionsTransform;

    public void UpdateRadarRotation(float horizontalRotation)
    {
        _directionsTransform.rotation = Quaternion.Euler(0, 0, horizontalRotation);
    }
}
