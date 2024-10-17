using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupMenuController : MenuController
{

    [field: SerializeField]
    protected TextMeshProUGUI stateSign { get; set; }


    protected IEnumerator FadeInMenu(float menuBkgSizeTrigger, float minBkgSize, float maxBkgSize, float bkgSizeChangeSpeed,
        float textFadeSpeed)
    {
        StartCoroutine(HelperMethods.TransitionHeightUI(menuBkgRect, minBkgSize, maxBkgSize, bkgSizeChangeSpeed));
        yield return new WaitUntil(() => menuBkgRect.sizeDelta.y >= menuBkgSizeTrigger);
        StartCoroutine(FadeOptions(Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, textFadeSpeed));
        yield return new WaitUntil(() => menuBkgRect.sizeDelta.y == maxBkgSize);
        CursorIndex = new Vector2Int(0, menuStartIndexVert);
        UpdateCursorPosition();
        yield return StartCoroutine(HelperMethods.FadeRawImage(cursor, Constants.MenuCursorAlphaMin, Constants.MenuCursorAlphaMax,
            Constants.MenuCursorFadeSpeed));
        StartCoroutine(HelperMethods.FadeText(stateSign, Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, textFadeSpeed));
    }

    protected IEnumerator FadeOutMenu(float cursorAlphaTrigger, float minBkgSize, float maxBkgSize, float bkgSizeChangeSpeed,
        float textFadeSpeed)
    {
        StartCoroutine(FadeOptions(Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, -textFadeSpeed));
        StartCoroutine(HelperMethods.FadeText(stateSign, Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, -textFadeSpeed));
        StartCoroutine(HelperMethods.FadeRawImage(cursor, Constants.MenuCursorAlphaMin, Constants.MenuCursorAlphaMax,
            -Constants.MenuCursorFadeSpeed));
        while (cursor.color.a > cursorAlphaTrigger)
        {
            yield return null;
        }
        StartCoroutine(HelperMethods.TransitionHeightUI(menuBkgRect, minBkgSize, maxBkgSize, -bkgSizeChangeSpeed));
    }

}
