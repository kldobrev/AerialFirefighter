using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField]
    private TextMeshProUGUI clearSign;
    [SerializeField]
    private InGameMenuController inGameMenu;

    public UnityEvent crashComplete;
    private int speedDisplayed;
    private Image heightMeterBkg;
    private int currentWeaponIconIdx;
    private static byte screenAlpha;
    private static byte extinguishSignAlpha;
    private static byte scoreToAddSignAlpha;
    private static byte clearSignAlpha;
    private static int numberOfFires;
    private float extingSignTimer;




    // Start is called before the first frame update
    void Start()
    {
        speedometer.text = Constants.DefaultSpeedValueUI;
        autoSpeedIndicator.text = Constants.DefaultAutoSpeedValueUI;
        speedDisplayed = 0;
        speedometer.color = Constants.SpeedColourInactive;
        autoSpeedIndicator.color = Constants.SpeedColourInactive;
        heightMeter.value = 0f;
        heightMeter.minValue = Constants.MinHeightAllowed;
        heightMeter.maxValue = Constants.MaxHeightAllowed;
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
        clearSignAlpha = Constants.ClearSignColour.a;
        clearSign.color = Constants.ClearSignColour;
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
            autoSpeedIndicator.text = "Auto speed: " + newAutoSpeed.ToString().PadLeft(3, '0') + " km/h";
            autoSpeedIndicator.color = Constants.AutoSpeedColourOn;
        }
        else
        {
            autoSpeedIndicator.color = Constants.SpeedColourInactive;
        }
    }

    public void UpdateHeightMeter(float height)
    {
        if (height <= Constants.AlertHeightUI && heightNumeric.color != Constants.HeightBelowAlertColour)
        {
            heightMeterBkg.color = Constants.HeightBelowAlertColour;
            heightNumeric.color = Constants.HeightBelowAlertColour;
        }
        else if (height > Constants.AlertHeightUI && heightNumeric.color != Constants.HeightAboveAlertColour)
        {
            heightMeterBkg.color = Constants.HeightAboveAlertColour;
            heightNumeric.color = Constants.HeightAboveAlertColour;
        }
        heightMeter.value = Mathf.Clamp(height, Constants.MinHeightAllowed, Constants.MaxHeightAllowed);
        heightNumeric.text = ((int) height).ToString().PadLeft(4, '0') + " m";
    }

    public void ScreenFadeToBlack()
    {
        StartCoroutine(ScreenFade(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax,
            Constants.FadeScreenSpeed));
    }

    public void ReverseScreenFadeToBlack()
    {
        StartCoroutine(ScreenFade(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax,
            -Constants.FadeScreenSpeed));
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
        extinguishedSign.fontSize = Constants.UISignDefaultFontSize;
        if (numFiresLeft == 0)
        {
            extinguishedSign.text = Constants.ExtinguishSignAllExtinguishedText;
            extinguishedSign.color = Constants.ExtinguishSignColourAll;
            StartCoroutine(HelperMethods.FadeText(extinguishedSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
        else if(numFiresCombo == 1)
        {
            extinguishedSign.text = Constants.ExtinguishSignDefaultText;
            StartCoroutine(HelperMethods.FadeText(extinguishedSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
        else
        {
            extinguishedSign.text = Constants.ExtinguishSignDefaultText + " x" + numFiresCombo.ToString();
        }
    }

    public void HideFireExtinguishedSign()
    {
        StartCoroutine(HelperMethods.FadeText(extinguishedSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
        StartCoroutine(HelperMethods.TransitionTextSize(extinguishedSign, Constants.UISignDefaultFontSize, 
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
            StartCoroutine(HelperMethods.FadeText(scoreToAddSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
    }

    public void PlayClearSequence(string signText)
    {
        clearSign.text = signText;
        StartCoroutine(ClearCoroutine());
    }

    public void CrashSequence(GameOverType type)
    {
        if (type == GameOverType.GroundCrash)
        {
            crashSign.text = Constants.CrashSignTextCrash;
            crashSign.color = Constants.CrashSignColourGround;
        }
        else if (type == GameOverType.WaterCrash)
        {
            crashSign.text = Constants.CrashSignTextCrash;
            crashSign.color = Constants.CrashSignColourWater;
        }
        else if (type == GameOverType.FuelDepleted)
        {
            crashSign.text = Constants.CrashSignTextEmpty;
            crashSign.color = Constants.CrashSignColourFuel;
        }

        inGameMenu.SetGameOverMenu(type);
        StartCoroutine(CrashCoroutine());
    }

    public void ScreenFadeInGame(float speed)
    {
        StartCoroutine(ScreenFade(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaPause, speed));
    }

    private IEnumerator ClearCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(HelperMethods.FadeText(clearSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(ScreenFade(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax,
            Constants.FadeScreenSpeed));
    }

    private IEnumerator CrashCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(HelperMethods.FadeText(crashSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(HelperMethods.FadeText(crashSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
        yield return ScreenFade(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaPause,
                Constants.FadeAlphaSpeedPause);
        crashComplete.Invoke();
    }

    public void HideScoreToAddSign()
    {
        StartCoroutine(HelperMethods.FadeText(scoreToAddSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
    }

    private IEnumerator ScreenFade(byte minAlpha, byte maxAlpha, float fadeSpeed)
    {
        byte alphaTarget = fadeSpeed > 0 ? maxAlpha : minAlpha;
        while (screenAlpha != alphaTarget)
        {
            screenAlpha = (byte) Math.Clamp(screenAlpha + fadeSpeed, minAlpha, maxAlpha);
            fadeEffectsPanel.color = new Color32(Constants.FadePanelDefaultColour.r,
                Constants.FadePanelDefaultColour.g, Constants.FadePanelDefaultColour.b, screenAlpha);
            yield return null;
        }
    }

}
