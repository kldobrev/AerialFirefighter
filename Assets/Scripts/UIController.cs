using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI speedometer;
    [SerializeField]
    private TextMeshProUGUI autoSpeedIndicator;
    [SerializeField]
    private Slider heightMeter;
    [SerializeField]
    private TextMeshProUGUI heightNumeric;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject [] weaponIcons;
    [SerializeField]
    private TextMeshProUGUI ammoLeftDisplay;
    [SerializeField]
    private RawImage fadeEffectsPanel;
    [SerializeField]
    private TextMeshProUGUI firesLeftCounter;

    private int speedDisplayed;
    private Image heightMeterBkg;
    private int weaponsAdded;
    private int currentWeaponIconIdx;
    private static int numberOfFires;
    private static byte[] fadePanelColour;


    void Awake()
    {
        weaponsAdded = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        speedometer.text = Constants.DefaultSpeedValueUI;
        autoSpeedIndicator.text = Constants.DefaultAutoSpeedValueUI;
        speedDisplayed = 0;
        autoSpeedIndicator.color = Constants.AutoSpeedColourOff;
        heightMeter.value = 0f;
        heightMeter.minValue = Constants.HeightMeterValueMinUI;
        heightMeter.maxValue = Constants.HeightMeterValueMaxUI;
        heightMeterBkg = heightMeter.GetComponentInChildren<Image>();
        heightMeterBkg.color = Constants.HeightBelowAlertColour;
        currentWeaponIconIdx = 0;
        fadePanelColour = new byte[4];
        fadePanelColour[0] = Constants.FadePanelDefaultColour.r;
        fadePanelColour[1] = Constants.FadePanelDefaultColour.g;
        fadePanelColour[2] = Constants.FadePanelDefaultColour.b;
        fadePanelColour[3] = Constants.FadePanelDefaultColour.a;
        fadeEffectsPanel.color = Constants.FadePanelDefaultColour;
    }

    public void UpdateSpeedometer(int newSpeed)
    {
        if(newSpeed != speedDisplayed)
        {
            speedDisplayed = newSpeed;
            speedometer.text = "Speed: " + newSpeed.ToString().PadLeft(3, '0') + " km/h";
        }
    }

    public void ToggleAutoSpeed(bool isTurnedOn, int newAutoSpeed)
    {
        if(isTurnedOn)
        {
            autoSpeedIndicator.text = "Auto speed: " + newAutoSpeed.ToString().PadLeft(4, '0') + " km/h";
            autoSpeedIndicator.color = Constants.AutoSpeedColourOn;
        }
        else
        {
            autoSpeedIndicator.color = Constants.AutoSpeedColourOff;
        }
    }

    public void UpdateHeightMeter(float height)
    {
        if (height <= Constants.HeightMeterValueAlertUI && heightMeter.maxValue != Constants.HeightMeterValueAlertUI)
        {
            heightMeter.maxValue = Constants.HeightMeterValueAlertUI;
            heightMeter.minValue = 0f;
            heightMeterBkg.color = Constants.HeightBelowAlertColour;
            heightNumeric.color = Constants.HeightBelowAlertColour;
        }
        else if (height > Constants.HeightMeterValueAlertUI && heightMeter.maxValue != Constants.HeightMeterValueMaxUI)
        {
            heightMeter.maxValue = Constants.HeightMeterValueMaxUI;
            heightMeter.minValue = Constants.HeightMeterValueAlertUI;
            heightMeterBkg.color = Constants.HeightAboveAlertColour;
            heightNumeric.color = Constants.HeightAboveAlertColour;
        }
        heightMeter.value = height;
        heightNumeric.text = ((int) Mathf.Clamp(height, 0f, Constants.HeightMeterValueMaxUI)).ToString().PadLeft(4, '0') + " m";
    }

    public void SwitchCurrentWeapon(int weaponIdx, int ammoAmount)
    {
        weaponIcons[currentWeaponIconIdx].SetActive(!weaponIcons[currentWeaponIconIdx].activeSelf);
        weaponIcons[weaponIdx].SetActive(!weaponIcons[weaponIdx].activeSelf);
        currentWeaponIconIdx = weaponIdx;
        UpdateWeaponAmmo(ammoAmount);
    }

    public void UpdateWeaponAmmo(int ammo)
    {
        ammoLeftDisplay.text =  ammo.ToString();
        if(ammo == 0)
        {
            weaponIcons[currentWeaponIconIdx].GetComponent<RawImage>().color = Constants.WeaponEmptyIconColour;
        }
    }

    public void AddWeapon(string iconPath)
    {
        if(weaponsAdded != Constants.MaxNumWeapons)
        {
            weaponIcons[weaponsAdded].GetComponent<RawImage>().texture = Resources.Load<Texture2D>(iconPath);
            weaponsAdded++;
            if(weaponsAdded == Constants.MaxNumWeapons)
            {
                weaponIcons[0].SetActive(true);
                for(int i = 1; i != Constants.MaxNumWeapons; i++)
                {
                    weaponIcons[i].SetActive(false);
                }
            }
        }
        else
        {
            Debug.Log("Attempting to add more weapon icons than allowed.");
        }
    }

    public void ScreenFadeToBlack()
    {
        StartCoroutine(FadeToBlack());
    }

    public void ReverseScreenFadeToBlack()
    {
        StartCoroutine(ReverseFadeToBlack());
    }

    public void UpdateFireCount(int fireCount)
    {
        firesLeftCounter.text = fireCount.ToString();
    }

    public static float GetScreenTransparency()
    {
        return fadePanelColour[3];
    }

    private IEnumerator FadeToBlack()
    {
        while (fadePanelColour[3] != 255)
        {
            fadePanelColour[3] += 5;
            fadeEffectsPanel.color = new Color32(fadePanelColour[0],
                fadePanelColour[1], fadePanelColour[2], fadePanelColour[3]);
            yield return null;
        }
    }
    private IEnumerator ReverseFadeToBlack()
    {
        while (fadePanelColour[3] != 0)
        {
            fadePanelColour[3] -= 5;
            fadeEffectsPanel.color = new Color32(fadePanelColour[0],
                fadePanelColour[1], fadePanelColour[2], fadePanelColour[3]);
            yield return null;
        }
    }

}
