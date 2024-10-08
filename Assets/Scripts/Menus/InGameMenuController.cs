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


    private new void Awake()
    {
        base.Awake();
        continueActive = true;
        continueSignGameOver = Constants.ContinueSignRegular;
        optionsSigns[0].text = Constants.ContinueSignRegular;
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
            selectorPos = optionsSigns[1].transform.localPosition;
            optionsSigns[1].text = Constants.RestartSignTutorial;
        }
        else if (mode == PlayMode.Generated)
        {
            continueActive = false;
            selectorPos = optionsSigns[1].transform.localPosition;
            optionsSigns[1].text = Constants.RestartSignStage;
        }
    }

    public IEnumerator TogglePauseMenu()
    {
        isOpened = !isOpened;
        if (isOpened)
        {
            yield return StartCoroutine(FadeInMenu(Constants.InGameMenuTextTrigger, Constants.InGameMenuBkgAlphaMin, 
                Constants.InGameMenuBkgAlphaMax, Constants.InGameMenuBkgSizeChangeSpeed, 
                Constants.InGameMenuTextFadeSpeedPauseIn));
            menuReady.Invoke();
        }
        else
        {
            yield return StartCoroutine(FadeOutMenu(Constants.InGameMenuSizeTrigger, Constants.InGameMenuBkgAlphaMin, 
                Constants.InGameMenuBkgAlphaMax, Constants.InGameMenuBkgSizeChangeSpeed, 
                Constants.InGameMenuTextFadeSpeedPauseOut));
            menuReady.Invoke();
        }
        yield return null;
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
        selectorTrns.localPosition = selectorPos;
    }

    public IEnumerator ShowGameOverMenu()
    {
        isOpened = true;
        yield return StartCoroutine(FadeInMenu(Constants.InGameMenuTextTrigger, Constants.InGameMenuBkgAlphaMin,
            Constants.InGameMenuBkgAlphaMax, Constants.InGameMenuBkgSizeChangeSpeed,
            Constants.InGameMenuTextFadeSpeedGameOver));
        menuReady.Invoke();
    }

}
