using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireController : MonoBehaviour
{
    [SerializeField]
    private float intensity = Constants.FireDefaultIntensity;
    [SerializeField]
    private ParticleSystem fireEffect;
    public UnityEvent<Transform> removeFire;

    private void OnParticleCollision(GameObject other)
    {
        intensity -= Constants.FireParticleDamage;
        transform.localScale -= Constants.FireScaleReduction;
        if(intensity == 0)
        {
            fireEffect.Stop();
            removeFire.Invoke(transform);
        }
    }

}
