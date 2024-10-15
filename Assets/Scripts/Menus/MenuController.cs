using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    protected Transform optionsHolder;
    [SerializeField]
    protected RawImage cursor;
    [SerializeField]
    protected UnityEvent<float> screenFadeEffect;

    protected RectTransform menuBkgRect;
    protected TextMeshProUGUI[] optionsSigns;
    protected int optionsCount;
    protected RectTransform cursorTrns;
    protected Vector3 startingCursorPos;
    protected bool isOpened;
    protected Vector2Int cursorIdx;
    protected Transform[] optionsTransforms;
    protected int menuStartIndexVert;
    public bool Visible => isOpened;
    public Vector2Int CursorIndex => cursorIdx;


    protected void Awake()
    {
        menuBkgRect = transform.GetComponent<RectTransform>();
        optionsCount = optionsHolder.childCount;
        optionsSigns = new TextMeshProUGUI[optionsCount];
        optionsTransforms = new Transform[optionsCount];
        for (int i = 0; i < optionsCount; i++)
        {
            optionsSigns[i] = optionsHolder.GetChild(i).GetComponent<TextMeshProUGUI>();
            optionsTransforms[i] = optionsSigns[i].transform;
        }
        cursorTrns = cursor.transform.GetComponent<RectTransform>();
        cursorIdx = Vector2Int.zero;
        menuStartIndexVert = 0;
    }

    public virtual void NavigateMenu(Vector2Int direction)
    {
        int nextIdx = cursorIdx.y + direction.y;
        if (nextIdx != (menuStartIndexVert - 1) && nextIdx != optionsCount) {
            cursorIdx.y = nextIdx;
            UpdateCursorPosition();
        }
    }

    public void UpdateCursorPosition()
    {
        cursorTrns.localPosition = optionsHolder.localPosition + optionsTransforms[cursorIdx.y].localPosition;
    }

    protected IEnumerator FadeOptions(float minAlpha, float maxAlpha, float speed)
    {
        for (int i = 0; i < optionsCount; i++)
        {
            StartCoroutine(HelperMethods.FadeText(optionsSigns[i], minAlpha, maxAlpha, speed));
        }
        yield return null;
    }

    
}
