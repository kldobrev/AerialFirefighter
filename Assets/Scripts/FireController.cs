using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireController : MonoBehaviour
{
    [SerializeField]
    private float _intensity;
    [SerializeField]
    private ParticleSystem _fireEffect;
    public UnityEvent<Transform> RemoveFire { get; set; }

    private float _startingIntensity;
    private float _checkpointIntensity;
    private Vector3 _checkpointScale;


    private void Start()
    {
        _startingIntensity = _intensity;    // Use to track fires for checkpoints
        _checkpointIntensity = _startingIntensity;
        _checkpointScale = transform.localScale;
    }

    private void OnParticleCollision(GameObject other)
    {
        _intensity -= Constants.FireParticleDamage;
        transform.localScale -= Vector3.forward * (Constants.FireParticleDamage / _startingIntensity);
        if (_intensity == 0)
        {
            _fireEffect.Stop();
            RemoveFire.Invoke(transform);
            gameObject.SetActive(false);
        }
    }

    public void UpdateCheckpointData()
    {
        _checkpointIntensity = _intensity;
        _checkpointScale = transform.localScale;
    }

    public void RestoreFire()
    {
        if (_intensity != _checkpointIntensity)  // Fire has not been put out, so restore the intensity to it's value when the check point was created
        {                                                                 
            _intensity = _checkpointIntensity;
            transform.localScale = _checkpointScale;
            if (!gameObject.activeSelf)    // Fire was put out
            {
                gameObject.SetActive(true);
                _fireEffect.Play();
            }
        }
    }

}
