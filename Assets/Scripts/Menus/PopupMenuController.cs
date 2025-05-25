using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class PopupMenuController : MenuController
{

    [field: SerializeField]
    protected TextMeshProUGUI StateSign { get; set; }


    public void SetPopupText(string text)
    {
        StateSign.text = text;
    }

    protected IEnumerator FadeInMenu(float menuBkgSizeTrigger, float minBkgSize, float maxBkgSize, float bkgSizeChangeSpeed,
        float textFadeSpeed)
    {
        StartCoroutine(HelperMethods.TransitionHeightUI(MenuBkgRect, minBkgSize, maxBkgSize, bkgSizeChangeSpeed));
        yield return new WaitUntil(() => MenuBkgRect.sizeDelta.y >= menuBkgSizeTrigger);
        StartCoroutine(FadeOptions(Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, textFadeSpeed));
        yield return new WaitUntil(() => MenuBkgRect.sizeDelta.y == maxBkgSize);
        UpdateCursorPosition();
        yield return StartCoroutine(HelperMethods.FadeImage(Cursor, Constants.MenuCursorAlphaMin, Constants.MenuCursorAlphaMax,
            Constants.MenuCursorFadeSpeedIn));
        yield return StartCoroutine(HelperMethods.FadeText(StateSign, Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, textFadeSpeed));
        Opened = true;
    }

    protected IEnumerator FadeOutMenu(float CursorAlphaTrigger, float minBkgSize, float maxBkgSize, float bkgSizeChangeSpeed,
        float textFadeSpeed)
    {
        StartCoroutine(FadeOptions(Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, -textFadeSpeed));
        StartCoroutine(HelperMethods.FadeText(StateSign, Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, -textFadeSpeed));
        StartCoroutine(HelperMethods.FadeImage(Cursor, Constants.MenuCursorAlphaMin, Constants.MenuCursorAlphaMax,
            -Constants.MenuCursorFadeSpeedOut));
        yield return new WaitUntil(() => GetOptionsAlpha() == Constants.MenuTextAlphaMin && StateSign.color.a == Constants.MenuTextAlphaMin 
            && Cursor.color.a <= CursorAlphaTrigger);
        yield return StartCoroutine(HelperMethods.TransitionHeightUI(MenuBkgRect, minBkgSize, maxBkgSize, -bkgSizeChangeSpeed));
        Opened = false;
    }

}
