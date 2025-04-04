using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvasController : CanvasController
{
    [SerializeField]
    private RectTransform titleTransform;

    // Start is called before the first frame update
    void Start()
    {
        titleTransform.localPosition = Constants.TitlePositionReady;
    }

    public void ShowTitle()
    {
        StartCoroutine(HelperMethods.MoveCanvasRectTransform(titleTransform, Constants.TitlePositionReady, -Constants.TitleMoveSpeed));
    }

    public void HideTitle()
    {
        StartCoroutine(HelperMethods.MoveCanvasRectTransform(titleTransform, Constants.TitlePositionInitial, Constants.TitleMoveSpeed));
    }

}
