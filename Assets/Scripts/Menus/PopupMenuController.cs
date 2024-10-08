using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupMenuController : MenuController
{

    [SerializeField]
    protected TextMeshProUGUI stateSign;


    protected IEnumerator FadeInMenu(float menuBkgSizeTrigger, float minBkgSize, float maxBkgSize, float bkgSizeChangeSpeed,
        float textFadeSpeed)
    {
        StartCoroutine(HelperMethods.TransitionHeightUI(menuBkgRect, minBkgSize, maxBkgSize, bkgSizeChangeSpeed));
        while (menuBkgRect.sizeDelta.y < menuBkgSizeTrigger)
        {
            yield return null;
        }
        StartCoroutine(FadeOptions(Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, textFadeSpeed));
        yield return StartCoroutine(HelperMethods.FadeRawImage(selector, Constants.MenuSelectorAlphaMin, Constants.MenuSelectorAlphaMax,
            Constants.MenuSelectorFadeSpeed));
        StartCoroutine(HelperMethods.FadeText(stateSign, Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, textFadeSpeed));
    }

    protected IEnumerator FadeOutMenu(float selectorAlphaTrigger, float minBkgSize, float maxBkgSize, float bkgSizeChangeSpeed,
        float textFadeSpeed)
    {
        StartCoroutine(FadeOptions(Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, -textFadeSpeed));
        StartCoroutine(HelperMethods.FadeText(stateSign, Constants.MenuTextAlphaMin, Constants.MenuTextAlphaMax, -textFadeSpeed));
        StartCoroutine(HelperMethods.FadeRawImage(selector, Constants.MenuSelectorAlphaMin, Constants.MenuSelectorAlphaMax,
            -Constants.MenuSelectorFadeSpeed));
        while (selector.color.a > selectorAlphaTrigger)
        {
            yield return null;
        }
        StartCoroutine(HelperMethods.TransitionHeightUI(menuBkgRect, minBkgSize, maxBkgSize, -bkgSizeChangeSpeed));
    }

}
