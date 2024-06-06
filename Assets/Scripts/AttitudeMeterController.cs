using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AttitudeMeterController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTrns;
    [SerializeField]
    private RectTransform bankPitchContainerTrns;
    [SerializeField]
    private UnityEngine.UI.Slider pitchSlider;

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
        if(bankPitchRotation != playerTrns.rotation.eulerAngles.z)
        {
            bankPitchRotation = -playerTrns.rotation.eulerAngles.z;
            bankPitchContainerTrns.rotation = Quaternion.Euler(0, 0, bankPitchRotation);
        }

        pitch = HelperMethods.GetSignedAngleFromEuler(playerTrns.rotation.eulerAngles.x);
        pitchSlider.value = Mathf.Clamp(pitch, -Constants.AttitudeMeterMaxPitchShown, Constants.AttitudeMeterMaxPitchShown);
    }

}
