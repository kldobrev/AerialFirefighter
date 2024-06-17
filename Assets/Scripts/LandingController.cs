using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTrns;
    private float playerEulerPitch;
    private float landingTimer;

    // Start is called before the first frame update
    void Start()
    {
        playerEulerPitch = 0;
        landingTimer = 0;
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        playerEulerPitch = HelperMethods.GetSignedAngleFromEuler(playerTrns.rotation.eulerAngles.x);
        Debug.Log("Landing: " + other.gameObject.name);
        if (-5 <= playerEulerPitch && playerEulerPitch <= 3)
        {
            Debug.Log("Landing angle rule met.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        landingTimer = !PlayerController.EngineStarted() ? landingTimer + Time.fixedDeltaTime : 0;
        if (landingTimer >= 3)
        {
            Debug.Log("Landing successfull!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        landingTimer = 0;
    }
}
