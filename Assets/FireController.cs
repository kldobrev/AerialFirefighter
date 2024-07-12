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
        if(intensity <= 0)
        {
            transform.GetComponent<ParticleSystem>().Stop();
            GameObject.Find("Player").GetComponent<PlayerController>().DecrementFiresCount();  // Temporary logic, should be moved when appropriate
            Destroy(gameObject);
        }
    }

}
