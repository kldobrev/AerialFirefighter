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
    [SerializeField]
    private TextMeshProUGUI crashSign;

    private int speedDisplayed;
    private Image heightMeterBkg;
    private int weaponsAdded;
    private int currentWeaponIconIdx;
    private static byte screenAlpha;
    private static byte extinguishSignAlpha;
    private static byte scoreToAddSignAlpha;
    private static int numberOfFires;
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
        speedometer.color = Constants.SpeedColourInactive;
        autoSpeedIndicator.color = Constants.SpeedColourInactive;
        heightMeter.value = 0f;
        heightMeter.minValue = Constants.HeightMeterValueMinUI;
        heightMeter.maxValue = Constants.HeightMeterValueMaxUI;
        heightMeterBkg = heightMeter.GetComponentInChildren<Image>();
        heightMeterBkg.color = Constants.HeightBelowAlertColour;
        currentWeaponIconIdx = 0;
        fadeEffectsPanel.color = Constants.FadePanelDefaultColour;
        screenAlpha = Constants.FadePanelDefaultColour.a;
        extinguishSignAlpha = Constants.ExtinguishSignColour.a;
        extinguishedSign.color = Constants.ExtinguishSignColour;
        extinguishedSign.fontSize = Constants.UISignDefaultFontSize;
        scoreToAddSignAlpha = Constants.ScoreToAddSignColour.a;
        scoreToAddSign.color = Constants.ScoreToAddSignColour;
        extinguishedSign.transform.parent.gameObject.SetActive(true);
    }

    public void SetSpeedometerColour(bool active)
    {
        speedometer.color = active ? Constants.SpeedColourIndicatorOn : Constants.SpeedColourInactive;
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
            autoSpeedIndicator.color = Constants.SpeedColourInactive;
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
        StartCoroutine(ScreenFade(Constants.FadeScreenSpeed));
    }

    public void ReverseScreenFadeToBlack()
    {
        StartCoroutine(ScreenFade(-Constants.FadeScreenSpeed));
    }

    public void UpdateFireCount(int fireCount, int firesInCombo)
    {
        firesLeftCounter.text = fireCount.ToString();
        ShowExtinguishedSign(fireCount, firesInCombo);
    }

    public static float GetScreenTransparency()
    {
        return screenAlpha;
    }

    public void ShowExtinguishedSign(int numFiresLeft, int numFiresCombo)
    {
        if (numFiresLeft == 0)
        {
            extinguishedSign.text = Constants.ExtinguishSignAllExtinguishedText;
            extinguishedSign.color = Constants.ExtinguishSignColourAll;
            StartCoroutine(FadeText(extinguishedSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
        else if(numFiresCombo == 1)
        {
            extinguishedSign.text = Constants.ExtinguishSignDefaultText;
            StartCoroutine(FadeText(extinguishedSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
        else
        {
            extinguishedSign.text = Constants.ExtinguishSignDefaultText + " x" + numFiresCombo.ToString();
        }
    }

    public void HideFireExtinguishedSign()
    {
        StartCoroutine(FadeText(extinguishedSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
        StartCoroutine(TransitionTextSize(extinguishedSign, Constants.UISignDefaultFontSize, 
            Constants.UISignMaxFontSize, 1));
    }

    public void UpdateScoreSign(int score)
    {
        scoreSign.text = "Score: " + score.ToString().PadLeft(7, '0');
    }

    public void UpdateScoreToAddSign(int score)
    {
        scoreToAddSign.text = "+" + score.ToString();
        if (scoreToAddSignAlpha != Constants.UISignMaxAlpha)
        {
            StartCoroutine(FadeText(scoreToAddSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
    }

    public void PlayCrashSequence(string signText, Color32 signColour)
    {
        crashSign.text = signText;
        crashSign.color = signColour;
        StartCoroutine(CrashCoroutine());
    }

    private IEnumerator CrashCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeText(crashSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(ScreenFade(Constants.FadeScreenSpeed));
    }

    public void HideScoreToAddSign()
    {
        StartCoroutine(FadeText(scoreToAddSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
    }

    private IEnumerator ScreenFade(float fadeSpeed)
    {
        float alphaTarget = fadeSpeed > 0 ? 255 : 0;
        while (screenAlpha != alphaTarget)
        {
            screenAlpha = (byte) Math.Clamp(screenAlpha + fadeSpeed, 0, 255);
            fadeEffectsPanel.color = new Color32(Constants.FadePanelDefaultColour.r,
                Constants.FadePanelDefaultColour.g, Constants.FadePanelDefaultColour.b, screenAlpha);
            yield return null;
        }
    }

    private IEnumerator FadeText(TextMeshProUGUI sign, float minAlpha, float maxAlpha, float fadeSpeed)
    {
        Color32 signColour = sign.color;
        float signTargetAlpha = fadeSpeed > 0 ? maxAlpha : minAlpha;
        while (signColour.a != signTargetAlpha)
        {
            signColour.a = (byte) Math.Clamp(signColour.a + fadeSpeed, minAlpha, maxAlpha);
            sign.color = signColour;
            yield return null;
        }
    }

    private IEnumerator TransitionTextSize(TextMeshProUGUI sign, float signSizeMin, float signSizeMax,
        float speed)
    {
        float signTargetSize = speed > 0 ? signSizeMax : signSizeMin;
        while (sign.fontSize != signTargetSize)
        {
            extinguishedSign.fontSize = Mathf.Clamp(extinguishedSign.fontSize + speed, signSizeMin, signSizeMax);
            yield return null;
        }
    }

}
