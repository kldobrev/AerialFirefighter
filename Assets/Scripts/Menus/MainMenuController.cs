using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MenuController
{
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;

    private new void Awake()
    {
        base.Awake();
        Opened = false;
        shownPosition = transform.position;
        hiddenPosition = shownPosition - (Vector2.right * MenuBkgRect.sizeDelta.x);
        transform.position = hiddenPosition;
    }

    public override void OpenMenu()
    {
        StartCoroutine(MoveMainMenuToScreen());
    }

    public override void CloseMenu()
    {
        StartCoroutine(MoveMainMenuOffscreen());
    }

    private IEnumerator MoveMainMenuOffscreen()
    {
        yield return StartCoroutine(HelperMethods.MoveCanvasRectTransform(MenuBkgRect, hiddenPosition, 
            -Constants.MainMenuMoveSpeed * Vector2.right));
        Opened = false;
    }

    private IEnumerator MoveMainMenuToScreen()
    {
        yield return StartCoroutine(HelperMethods.MoveCanvasRectTransform(MenuBkgRect, shownPosition, 
            Constants.MainMenuMoveSpeed * Vector2.right));
        Opened = true;
    }

}
