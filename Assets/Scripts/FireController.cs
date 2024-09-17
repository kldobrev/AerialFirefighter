using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireController : MonoBehaviour
{
    [SerializeField]
    private float intensity = Constants.FireDefaultIntensity;
    [SerializeField]
    private UnityEvent<int> removeFireGroupFromLocator;

    private void OnParticleCollision(GameObject other)
    {
        intensity -= Constants.FireParticleDamage;
        transform.localScale -= Constants.FireScaleReduction;

        Debug.Log("Intensity down to: " + intensity);

        if(intensity <= 0)
        {
            transform.GetComponent<ParticleSystem>().Stop();
            transform.parent.parent.GetComponent<FireMissionController>().DecrementFiresCount();

            if (transform.parent.childCount == 1)   // The only fire left in the fire group
            {
                removeFireGroupFromLocator.Invoke(transform.parent.GetSiblingIndex());
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }

}
