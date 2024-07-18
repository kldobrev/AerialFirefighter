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
    private TextMeshProUGUI ammoLeftDisplay;
    [SerializeField]
    private RawImage fadeEffectsPanel;
    [SerializeField]
    private TextMeshProUGUI firesLeftCounter;
    [SerializeField]
    private TextMeshProUGUI extinguishedSign;
    [SerializeField]
    private TextMeshProUGUI scoreSign;
    [SerializeField]
    private TextMeshProUGUI scoreToAddSign;

    private int speedDisplayed;
    private Image heightMeterBkg;
    private int weaponsAdded;
    private int currentWeaponIconIdx;
    private static int numberOfFires;
    private static byte[] fadePanelColour;
    private static byte[] extinguishSignColour;
    private static byte[] scoreToAddSignColour;
    private float extingSignTimer;


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

        extinguishSignColour = new byte[4];
        extinguishSignColour[0] = Constants.ExtinguishSignColour.r;
        extinguishSignColour[1] = Constants.ExtinguishSignColour.g;
        extinguishSignColour[2] = Constants.ExtinguishSignColour.b;
        extinguishSignColour[3] = Constants.ExtinguishSignColour.a;
        extinguishedSign.color = Constants.ExtinguishSignColour;

        scoreToAddSignColour = new byte[4];
        scoreToAddSignColour[0] = Constants.ScoreToAddSignColour.r;
        scoreToAddSignColour[1] = Constants.ScoreToAddSignColour.g;
        scoreToAddSignColour[2] = Constants.ScoreToAddSignColour.b;
        scoreToAddSignColour[3] = Constants.ScoreToAddSignColour.a;
        scoreToAddSign.color = Constants.ScoreToAddSignColour;
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

    public void ScreenFadeToBlack()
    {
        StartCoroutine(FadeToBlack());
    }

    public void ReverseScreenFadeToBlack()
    {
        StartCoroutine(ReverseFadeToBlack());
    }

    public void UpdateFireCount(int fireCount, int firesInCombo)
    {
        firesLeftCounter.text = fireCount.ToString();
        if (firesInCombo > 0)
        {
            ShowExtinguishedSign(firesInCombo);
        }
    }

    public static float GetScreenTransparency()
    {
        return fadePanelColour[3];
    }

    public void ShowExtinguishedSign(int numFires)
    {
        if(numFires == 1)
        {
            extinguishedSign.text = Constants.ExtinguishSignDefaultValue;
            StartCoroutine(FadeExtinguishSignIn());
        }
        else
        {
            extinguishedSign.text = Constants.ExtinguishSignDefaultValue  + " x" + numFires.ToString();
        }
    }

    public void HideFireExtinguishedSign()
    {
        StartCoroutine(FadeExtinguishSignOut());
    }

    public void UpdateScoreSign(int score)
    {
        scoreSign.text = "Score: " + score.ToString().PadLeft(7, '0');
    }

    public void UpdateScoreToAddSign(int score)
    {
        scoreToAddSign.text = "+" + score.ToString();
        if (scoreToAddSignColour[3] != Constants.UISignMaxAlpha)
        {
            StartCoroutine(FadeScoreSign(true));
        }
    }

    public void HideScoreToAddSign()
    {
        StartCoroutine(FadeScoreSign(false));
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

    private IEnumerator FadeExtinguishSignIn()
    {
        extinguishedSign.fontSize = Constants.UISignDefaultFontSize;
        while (extinguishSignColour[3] != Constants.UISignMaxAlpha)
        {
            extinguishSignColour[3] = (byte) Math.Clamp(extinguishSignColour[3] + Constants.UISignFadeSpeed,
                0, Constants.UISignMaxAlpha);
            extinguishedSign.color = new Color32(extinguishSignColour[0], extinguishSignColour[1],
                extinguishSignColour[2], extinguishSignColour[3]);
            yield return null;
        }
    }

    private IEnumerator FadeExtinguishSignOut()
    {
        while (extinguishSignColour[3] != 0 || extinguishedSign.fontSize != Constants.UISignMaxFontSize)
        {
            extinguishedSign.fontSize = Mathf.Clamp(extinguishedSign.fontSize + 1, Constants.UISignDefaultFontSize, 
                Constants.UISignMaxFontSize);
            extinguishSignColour[3] = (byte) Math.Clamp(extinguishSignColour[3] - Constants.UISignFadeSpeed, 
                0, Constants.UISignMaxAlpha);
            extinguishedSign.color = new Color32(extinguishSignColour[0], extinguishSignColour[1],
                extinguishSignColour[2], extinguishSignColour[3]);
            yield return null;
        }
    }

    private IEnumerator FadeScoreSign(bool fadeInDirection)
    {
        int fadeDirection = fadeInDirection ? 1 : -1;
        int borderAlphaValue = fadeInDirection ? Constants.UISignMaxAlpha : 0;
        while (scoreToAddSignColour[3] != borderAlphaValue)
        {
            scoreToAddSignColour[3] = (byte) Math.Clamp(scoreToAddSignColour[3] + (fadeDirection * Constants.UISignFadeSpeed),
                0, Constants.UISignMaxAlpha);
            scoreToAddSign.color = new Color32(scoreToAddSignColour[0], scoreToAddSignColour[1],
                scoreToAddSignColour[2], scoreToAddSignColour[3]);
            yield return null;
        }
    }

}
