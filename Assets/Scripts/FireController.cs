using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireController : MonoBehaviour
{
    [SerializeField]
    private float _intensity = Constants.FireDefaultIntensity;
    [SerializeField]
    private ParticleSystem _fireEffect;
    public UnityEvent<Transform> RemoveFire { get; set; }

    private void OnParticleCollision(GameObject other)
    {
        _intensity -= Constants.FireParticleDamage;
        transform.localScale -= Constants.FireScaleReduction;
        if(_intensity == 0)
        {
            _fireEffect.Stop();
            RemoveFire.Invoke(transform);
        }
    }

}
