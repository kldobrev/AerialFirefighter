using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AttitudeMeterController : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTrns;
    [SerializeField]
    private RectTransform _bankPitchContainer;
    [SerializeField]
    private UnityEngine.UI.Slider _pitchSlider;

    private float bankPitchRotation;
    private float pitch;

    // Start is called before the first frame update
    void Start()
    {
        bankPitchRotation = 0;
        pitch = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTrns != null)
        {
            if (bankPitchRotation != _playerTrns.rotation.eulerAngles.z)
            {
                bankPitchRotation = -_playerTrns.rotation.eulerAngles.z;
                _bankPitchContainer.rotation = Quaternion.Euler(0, 0, bankPitchRotation);
            }

            pitch = HelperMethods.GetSignedAngleFromEuler(_playerTrns.rotation.eulerAngles.x);
            _pitchSlider.value = Mathf.Clamp(pitch, -Constants.AttitudeMeterMaxPitchShown, Constants.AttitudeMeterMaxPitchShown);
        }
    }

}
