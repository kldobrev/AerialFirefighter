using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTrns;
    private float playerEulerPitch;
    private float playerEulerBank;
    private float landingTimer;

    // Start is called before the first frame update
    void Start()
    {
        playerEulerPitch = 0;
        landingTimer = 0;
    }

    private void OnCollisionStay(Collision collision)
    {
        landingTimer = !PlayerController.EngineStarted() ? landingTimer + Time.fixedDeltaTime : 0;
        /*if (landingTimer >= 3 && !PlayerController.EngineStarted())
        {
            Debug.Log("Landing successfull!");
        }*/
    }

    private void OnCollisionExit(Collision collision)
    {
        landingTimer = 0;
    }

}
