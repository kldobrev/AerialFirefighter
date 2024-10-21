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
    private string _continueSignGameOver;
    private Color32 _continueSignGameOverColour;
    private bool _continueActive;
    private int _gameOverStartIdx;

    private new void Awake()
    {
        base.Awake();
        _continueActive = true;
        _continueSignGameOver = Constants.ContinueSignRegular;
        optionsSigns[0].text = Constants.ContinueSignRegular;
        _gameOverStartIdx = 0;
        Opened = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        stateSign.color = Constants.InGameMenuStateColorPause;
        stateSign.text = Constants.InGameMenuStateSignPause;
        _continueSignGameOverColour = Constants.InGameMenuOptionColourDefault;
    }

    public void SetInGameMenuForMode(PlayMode mode)
    {
        if (mode == PlayMode.FireMission)
        {
            _continueSignGameOver = Constants.ContinueSignCheckpoint;
            optionsSigns[1].text = Constants.RestartSignStage;
        }
        else if (mode == PlayMode.Tutorial)
        {
            _continueActive = false;
            startingCursorPos = optionsHolder.localPosition + optionsTransforms[1].localPosition;
            optionsSigns[1].text = Constants.RestartSignTutorial;
            _gameOverStartIdx = 1;
        }
        else if (mode == PlayMode.Generated)
        {
            _continueActive = false;
            startingCursorPos = optionsHolder.localPosition + optionsTransforms[1].localPosition;
            optionsSigns[1].text = Constants.RestartSignStage;
            _gameOverStartIdx = 1;
        }
    }

    public void OpenPauseMenu()
    {
        StartCoroutine(ToggleInGameMenu(true, Constants.InGameMenuTextFadeSpeedPauseIn));
    }

    public void ClosePauseMenu()
    {
        StartCoroutine(ToggleInGameMenu(false, Constants.InGameMenuTextFadeSpeedPauseOut));
    }

    public void OpenGameOverMenu()
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
        optionsSigns[0].text = _continueSignGameOver;
        optionsSigns[0].gameObject.SetActive(_continueActive);
        cursorTrns.localPosition = startingCursorPos;
        menuStartIndexVert = _gameOverStartIdx;
    }

    private IEnumerator ToggleInGameMenu(bool activate, float textFadeSpeed)
    {
        if (activate)
        {
            yield return StartCoroutine(FadeInMenu(Constants.InGameMenuTextTrigger, Constants.InGameMenuBkgAlphaMin, 
                Constants.InGameMenuBkgAlphaMax, Constants.InGameMenuBkgSizeChangeSpeed, textFadeSpeed));
        }
        else
        {
            yield return StartCoroutine(FadeOutMenu(Constants.InGameMenuSizeTrigger, Constants.InGameMenuBkgAlphaMin, 
                Constants.InGameMenuBkgAlphaMax, Constants.InGameMenuBkgSizeChangeSpeed, textFadeSpeed));
        }
        Opened = activate;
    }


}
