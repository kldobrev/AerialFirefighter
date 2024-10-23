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
    private TextMeshProUGUI _speedometer;
    [SerializeField]
    private TextMeshProUGUI _autoSpeedIndicator;
    [SerializeField]
    private Slider _heightMeter;
    [SerializeField]
    private TextMeshProUGUI _heightNumeric;
    [SerializeField]
    private RawImage _fadeEffectsPanel;
    [SerializeField]
    private TextMeshProUGUI _firesLeftCounter;
    [SerializeField]
    private TextMeshProUGUI _extinguishedSign;
    [SerializeField]
    private TextMeshProUGUI _scoreSign;
    [SerializeField]
    private TextMeshProUGUI _scoreToAddSign;
    [SerializeField]
    private TextMeshProUGUI _crashSign;
    [SerializeField]
    private TextMeshProUGUI _clearSign;
    [SerializeField]
    private InGameMenuController _inGameMenu;

    public UnityEvent CrashComplete { get; set; }
    public static byte ScreenAlpha { get; private set; }
    private int _speedDisplayed;
    private Image _heightMeterBkg;
    private static byte _scoreToAddSignAlpha;


    void Awake()
    {
        CrashComplete = new();
    }

    // Start is called before the first frame update
    void Start()
    {
        _speedometer.text = Constants.DefaultSpeedValueUI;
        _autoSpeedIndicator.text = Constants.DefaultAutoSpeedValueUI;
        _speedDisplayed = 0;
        _speedometer.color = Constants.SpeedColourInactive;
        _autoSpeedIndicator.color = Constants.SpeedColourInactive;
        _heightMeter.value = 0f;
        _heightMeter.minValue = Constants.MinHeightAllowed;
        _heightMeter.maxValue = Constants.MaxHeightAllowed;
        _heightMeterBkg = _heightMeter.GetComponentInChildren<Image>();
        _heightMeterBkg.color = Constants.HeightBelowAlertColour;
        _fadeEffectsPanel.color = Constants.FadePanelDefaultColour;
        ScreenAlpha = Constants.FadePanelDefaultColour.a;
        _extinguishedSign.color = Constants.ExtinguishSignColour;
        _extinguishedSign.fontSize = Constants.UISignDefaultFontSize;
        _scoreToAddSignAlpha = Constants.ScoreToAddSignColour.a;
        _scoreToAddSign.color = Constants.ScoreToAddSignColour;
        _clearSign.color = Constants.ClearSignColour;
        _extinguishedSign.transform.parent.gameObject.SetActive(true);
    }

    public void SetSpeedometerColour(bool active)
    {
        _speedometer.color = active ? Constants.SpeedColourIndicatorOn : Constants.SpeedColourInactive;
    }

    public void UpdateSpeedometer(int newSpeed)
    {
        if(newSpeed != _speedDisplayed)
        {
            _speedDisplayed = newSpeed;
            _speedometer.text = "Speed: " + newSpeed.ToString().PadLeft(3, '0') + " km/h";
        }
    }

    public void ToggleAutoSpeed(bool isTurnedOn, int newAutoSpeed)
    {
        if(isTurnedOn)
        {
            _autoSpeedIndicator.text = "Auto speed: " + newAutoSpeed.ToString().PadLeft(3, '0') + " km/h";
            _autoSpeedIndicator.color = Constants.AutoSpeedColourOn;
        }
        else
        {
            _autoSpeedIndicator.color = Constants.SpeedColourInactive;
        }
    }

    public void UpdateHeightMeter(float height)
    {
        if (height <= Constants.AlertHeightUI && _heightNumeric.color != Constants.HeightBelowAlertColour)
        {
            _heightMeterBkg.color = Constants.HeightBelowAlertColour;
            _heightNumeric.color = Constants.HeightBelowAlertColour;
        }
        else if (height > Constants.AlertHeightUI && _heightNumeric.color != Constants.HeightAboveAlertColour)
        {
            _heightMeterBkg.color = Constants.HeightAboveAlertColour;
            _heightNumeric.color = Constants.HeightAboveAlertColour;
        }
        _heightMeter.value = Mathf.Clamp(height, Constants.MinHeightAllowed, Constants.MaxHeightAllowed);
        _heightNumeric.text = ((int) height).ToString().PadLeft(4, '0') + " m";
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
        _firesLeftCounter.text = fireCount.ToString();
        ShowExtinguishedSign(fireCount, firesInCombo);
    }

    public void ShowExtinguishedSign(int numFiresLeft, int numFiresCombo)
    {
        _extinguishedSign.fontSize = Constants.UISignDefaultFontSize;
        if (numFiresLeft == 0)
        {
            _extinguishedSign.text = Constants.ExtinguishSignAllExtinguishedText;
            _extinguishedSign.color = Constants.ExtinguishSignColourAll;
            StartCoroutine(HelperMethods.FadeText(_extinguishedSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
        else if(numFiresCombo == 1)
        {
            _extinguishedSign.text = Constants.ExtinguishSignDefaultText;
            StartCoroutine(HelperMethods.FadeText(_extinguishedSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
        else
        {
            _extinguishedSign.text = Constants.ExtinguishSignDefaultText + " x" + numFiresCombo.ToString();
        }
    }

    public void HideFireExtinguishedSign()
    {
        StartCoroutine(HelperMethods.FadeText(_extinguishedSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
        StartCoroutine(HelperMethods.TransitionTextSize(_extinguishedSign, Constants.UISignDefaultFontSize, 
            Constants.UISignMaxFontSize, 1));
    }

    public void UpdateScoreSign(int score)
    {
        _scoreSign.text = "Score: " + score.ToString().PadLeft(7, '0');
    }

    public void UpdateScoreToAddSign(int score)
    {
        _scoreToAddSign.text = "+" + score.ToString();
        if (_scoreToAddSignAlpha != Constants.UISignMaxAlpha)
        {
            StartCoroutine(HelperMethods.FadeText(_scoreToAddSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        }
    }

    public void PlayClearSequence(string signText)
    {
        _clearSign.text = signText;
        StartCoroutine(ClearCoroutine());
    }

    public void ScreenFadeInGame(byte minAlpha, byte maxAlpha, float speed)
    {
        StartCoroutine(ScreenFade(minAlpha, maxAlpha, speed));
    }

    public void HideScoreToAddSign()
    {
        StartCoroutine(HelperMethods.FadeText(_scoreToAddSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
    }

    public IEnumerator CrashSequence(GameOverType type)
    {
        if (type == GameOverType.GroundCrash)
        {
            _crashSign.text = Constants.CrashSignTextCrash;
            _crashSign.color = Constants.CrashSignColourGround;
        }
        else if (type == GameOverType.WaterCrash)
        {
            _crashSign.text = Constants.CrashSignTextCrash;
            _crashSign.color = Constants.CrashSignColourWater;
        }
        else if (type == GameOverType.FuelDepleted)
        {
            _crashSign.text = Constants.CrashSignTextEmpty;
            _crashSign.color = Constants.CrashSignColourFuel;
        }

        _inGameMenu.SetGameOverMenu(type);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(HelperMethods.FadeText(_crashSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(HelperMethods.FadeText(_crashSign, 0, Constants.UISignMaxAlpha, -Constants.UISignFadeSpeed));
        yield return ScreenFade(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaPause,
                Constants.FadeAlphaSpeedPause);
        CrashComplete.Invoke();
    }

    private IEnumerator ClearCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(HelperMethods.FadeText(_clearSign, 0, Constants.UISignMaxAlpha, Constants.UISignFadeSpeed));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(ScreenFade(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax,
            Constants.FadeScreenSpeed));
    }

    private IEnumerator ScreenFade(byte minAlpha, byte maxAlpha, float fadeSpeed)
    {
        byte alphaTarget = fadeSpeed > 0 ? maxAlpha : minAlpha;
        while (ScreenAlpha != alphaTarget)
        {
            ScreenAlpha = (byte) Math.Clamp(ScreenAlpha + fadeSpeed, minAlpha, maxAlpha);
            _fadeEffectsPanel.color = new Color32(Constants.FadePanelDefaultColour.r,
                Constants.FadePanelDefaultColour.g, Constants.FadePanelDefaultColour.b, ScreenAlpha);
            yield return null;
        }
    }

}
