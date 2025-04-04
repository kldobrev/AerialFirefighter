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
    private bool _continueActive;
    private int _gameOverStartIdx;
    private bool _checkpointContinueAllowed;

    private new void Awake()
    {
        base.Awake();
        _continueActive = true;
        _gameOverStartIdx = 1;
        Opened = false;
        _checkpointContinueAllowed = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _continueSignGameOver = Constants.ContinueSignRegular;
        ActivatePauseMenu();
    }

    public void SetInGameMenuForMode(PlayMode mode)
    {
        if (mode == PlayMode.FireMission)
        {
            _continueSignGameOver = Constants.ContinueSignCheckpoint;
            OptionsSigns[1].text = Constants.RestartSignStage;
        }
        else if (mode == PlayMode.Tutorial)
        {
            _continueActive = false;
            StartingCursorPos = OptionsHolder.localPosition + OptionsTransforms[1].localPosition;
            OptionsSigns[1].text = Constants.RestartSignTutorial;
        }
        else if (mode == PlayMode.Generated)
        {
            _continueActive = false;
            StartingCursorPos = OptionsHolder.localPosition + OptionsTransforms[1].localPosition;
            OptionsSigns[1].text = Constants.RestartSignStage;
        }
    }

    public override void OpenMenu()
    {
        StartCoroutine(ToggleInGameMenu(true, Constants.InGameMenuTextFadeSpeedIn));
    }

    public override void CloseMenu()
    {
        StartCoroutine(ToggleInGameMenu(false, Constants.InGameMenuTextFadeSpeedOut));
    }

    public void ActivatePauseMenu()
    {
        StateSign.color = Constants.InGameMenuStateColorPause;  // Pause sign colour is still red
        StateSign.text = Constants.InGameMenuStateSignPause;
        OptionsSigns[0].text = Constants.ContinueSignRegular;
    }

    public void ActivateGameOverMenu(GameOverType type)
    {
        if (type == GameOverType.GroundCrash)
        {
            StateSign.color = Constants.CrashSignColourGround;
        }
        else if (type == GameOverType.WaterCrash)
        {
            StateSign.color = Constants.CrashSignColourWater;
        }
        else if (type == GameOverType.FuelDepleted)
        {
            StateSign.color = Constants.CrashSignColourFuel;
        }

        StateSign.text = Constants.InGameMenuStateSignGameOver;
        OptionsSigns[0].text = _continueSignGameOver;
        OptionsSigns[0].gameObject.SetActive(_continueActive);
        OptionsSigns[0].color = _checkpointContinueAllowed ? Constants.InGameMenuOptionColourDefault 
            : Constants.InGameMenuOptionColourDisabled;
        MenuStartIndexVert = _gameOverStartIdx;
        StartingCursorPos = OptionsHolder.localPosition + OptionsTransforms[MenuStartIndexVert].localPosition;
        ResetCursorPosition();
    }

    public void AllowCheckpointContinue()
    {
        _checkpointContinueAllowed = true;
        _gameOverStartIdx = 0;
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
    }

}
