using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPromptController : PopupMenuController
{
    public bool Confirmed { protected set; get; }
    public bool Responded { protected set; get; }


    // Start is called before the first frame update
    void Start()
    {
        Responded = false;
        Opened = false;
    }

    public override void OpenMenu()
    {
        Responded = false;
        StartCoroutine(FadeInMenu(Constants.ConfirmPromptTextTrigger, Constants.InGameMenuBkgAlphaMin, Constants.ConfirmPromptBkgAlphaMax, 
            Constants.InGameMenuBkgSizeChangeSpeed, Constants.InGameMenuTextFadeSpeedIn));
    }

    public override void CloseMenu()
    {
        StartCoroutine(FadeOutMenu(Constants.InGameMenuSizeTrigger, Constants.InGameMenuBkgAlphaMin, Constants.InGameMenuBkgAlphaMax, 
            Constants.InGameMenuBkgSizeChangeSpeed, Constants.InGameMenuTextFadeSpeedOut));
    }

    public virtual void GiveResponse()
    {
        Confirmed = CursorIndex.y == 0 ? true : false;
        Responded = true;
    }

}
