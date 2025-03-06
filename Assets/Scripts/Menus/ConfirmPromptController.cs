using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPromptController : PopupMenuController
{
    public bool Confirmed { private set; get; }
    public bool Responded { private set; get; }

    // Start is called before the first frame update
    void Start()
    {
        Responded = false;
        Opened = false;
        StateSign.text = Constants.LeaveStagePromptText;
    }

    public void Open()
    {
        Responded = false;
        StartCoroutine(FadeInMenu(Constants.ConfirmPromptTextTrigger, Constants.InGameMenuBkgAlphaMin, Constants.ConfirmPromptBkgAlphaMax, 
            Constants.InGameMenuBkgSizeChangeSpeed, Constants.InGameMenuTextFadeSpeedPauseIn));
    }

    public void Close()
    {
        StartCoroutine(FadeOutMenu(Constants.InGameMenuSizeTrigger, Constants.InGameMenuBkgAlphaMin, Constants.InGameMenuBkgAlphaMax, 
            Constants.InGameMenuBkgSizeChangeSpeed, Constants.InGameMenuTextFadeSpeedPauseOut));
    }

    public void GiveResponse()
    {
        Confirmed = CursorIndex.y == 0 ? true : false;
        Responded = true;
    }

}
