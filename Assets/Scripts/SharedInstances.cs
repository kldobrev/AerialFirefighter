using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedInstances : MonoBehaviour
{
    public static Transform ProjectilesParent;

    private void Start()
    {
        //ProjectilesParent = GameObject.Find("Projectiles").transform;
    }

}
