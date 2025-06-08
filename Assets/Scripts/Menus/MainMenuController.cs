using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenuController : MenuController
{
    [SerializeField]
    private Canvas screenCanvas;
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private float currentScaleFactor;
    private FrameRule checkResolutionRule;
    private float menuMoveSpeed;


    private new void Awake()
    {
        base.Awake();
        Opened = false;
        CalculateMenuProperties();
        checkResolutionRule = new FrameRule(15);
        transform.position = hiddenPosition;
    }


    private void Update()
    {
        if (checkResolutionRule.CheckFrameRule() && currentScaleFactor != screenCanvas.scaleFactor)
        {
            CalculateMenuProperties();
        }
        checkResolutionRule.AdvanceCounter();
    }


    public override void OpenMenu()
    {
       StartCoroutine(MoveMainMenuToScreen());
    }

    public override void CloseMenu()
    {
        StartCoroutine(MoveMainMenuOffscreen());
    }

    private void CalculateMenuProperties()
    {
        currentScaleFactor = screenCanvas.scaleFactor;
        menuMoveSpeed = Constants.MainMenuMoveSpeedBase * currentScaleFactor;
        shownPosition = Vector2.zero;
        hiddenPosition = shownPosition - (Vector2.right * MenuBkgRect.rect.width * currentScaleFactor);
    }

    private IEnumerator MoveMainMenuOffscreen()
    {
        yield return StartCoroutine(HelperMethods.MoveCanvasRectTransform(MenuBkgRect, hiddenPosition, 
            -menuMoveSpeed * Vector2.right));
        Opened = false;
    }

    private IEnumerator MoveMainMenuToScreen()
    {
        yield return StartCoroutine(HelperMethods.MoveCanvasRectTransform(MenuBkgRect, shownPosition, 
            menuMoveSpeed * Vector2.right));
        Opened = true;
    }

}
