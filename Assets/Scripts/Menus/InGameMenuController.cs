using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameMenuController : PopupMenuController
{
    [SerializeField]
    protected UnityEvent menuReady;

    protected string continueSignGameOver;
    protected Color32 continueSignGameOverColour;
    protected bool continueActive;
    private int gameOverStartIdx;

    private new void Awake()
    {
        base.Awake();
        continueActive = true;
        continueSignGameOver = Constants.ContinueSignRegular;
        optionsSigns[0].text = Constants.ContinueSignRegular;
        gameOverStartIdx = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        isOpened = false;
        stateSign.color = Constants.InGameMenuStateColorPause;
        stateSign.text = Constants.InGameMenuStateSignPause;
        continueSignGameOverColour = Constants.InGameMenuOptionColourDefault;
    }

    public void SetInGameMenuForMode(PlayMode mode)
    {
        if (mode == PlayMode.FireMission)
        {
            continueSignGameOver = Constants.ContinueSignCheckpoint;
            optionsSigns[1].text = Constants.RestartSignStage;
        }
        else if (mode == PlayMode.Tutorial)
        {
            continueActive = false;
            startingcursorPos = optionsHolder.localPosition + optionsTransforms[1].localPosition;
            optionsSigns[1].text = Constants.RestartSignTutorial;
            gameOverStartIdx = 1;
        }
        else if (mode == PlayMode.Generated)
        {
            continueActive = false;
            startingcursorPos = optionsHolder.localPosition + optionsTransforms[1].localPosition;
            optionsSigns[1].text = Constants.RestartSignStage;
            gameOverStartIdx = 1;
        }
    }

    public void ShowPuseMenu()
    {
        StartCoroutine(ToggleInGameMenu(true, Constants.InGameMenuTextFadeSpeedPauseIn));
    }

    public void HidePuseMenu()
    {
        StartCoroutine(ToggleInGameMenu(false, Constants.InGameMenuTextFadeSpeedPauseOut));
    }

    public void ShowGameOverMenu()
    {
        StartCoroutine(ToggleInGameMenu(true, Constants.InGameMenuTextFadeSpeedGameOver));
    }

    public void SetGameOverMenu(GameOverType type)
    {
        if (type == GameOverType.GroundCrash)
        {
            stateSign.color = Constants.CrashSignColourGround;
        }
        else if (type == GameOverType.WaterCrash)
        {
            stateSign.color = Constants.CrashSignColourWater;
        }
        else if (type == GameOverType.FuelDepleted)
        {
            stateSign.color = Constants.CrashSignColourFuel;
        }

        stateSign.text = Constants.InGameMenuStateSignGameOver;
        optionsSigns[0].text = continueSignGameOver;
        optionsSigns[0].gameObject.SetActive(continueActive);
        cursorTrns.localPosition = startingcursorPos;
        menuStartIndexVert = gameOverStartIdx;
    }

    public IEnumerator ToggleInGameMenu(bool activate, float textFadeSpeed)
    {
        if (activate)
        {
            screenFadeEffect.Invoke(Constants.FadeScreenAlphaPause);
            yield return StartCoroutine(FadeInMenu(Constants.InGameMenuTextTrigger, Constants.InGameMenuBkgAlphaMin, 
                Constants.InGameMenuBkgAlphaMax, Constants.InGameMenuBkgSizeChangeSpeed, textFadeSpeed));
            menuReady.Invoke();
        }
        else
        {
            yield return StartCoroutine(FadeOutMenu(Constants.InGameMenuSizeTrigger, Constants.InGameMenuBkgAlphaMin, 
                Constants.InGameMenuBkgAlphaMax, Constants.InGameMenuBkgSizeChangeSpeed, textFadeSpeed));
            menuReady.Invoke();
            screenFadeEffect.Invoke(-Constants.FadeScreenAlphaPause);
        }
        isOpened = activate;
        yield return null;
    }


}
