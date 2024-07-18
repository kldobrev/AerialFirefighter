using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField]
    private float intensity = Constants.FireDefaultIntensity;

    private void OnParticleCollision(GameObject other)
    {
        intensity -= Constants.FireParticleDamage;
        transform.localScale -= Constants.FireScaleReduction;

        Debug.Log("Intensity down to: " + intensity);

        if(intensity <= 0)
        {
            transform.GetComponent<ParticleSystem>().Stop();
            transform.parent.GetComponent<FireMissionController>().DecrementFiresCount();
            Destroy(gameObject);
        }
    }

}
