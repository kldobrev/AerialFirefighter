using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [field: SerializeField]
    protected Transform optionsHolder { get; set; }
    [field: SerializeField]
    protected RawImage cursor { get; set; }
    [field: SerializeField]
    protected UnityEvent<float> screenFadeEffect { get; set; }

    protected RectTransform menuBkgRect { get; set; }
    protected TextMeshProUGUI[] optionsSigns { get; set; }
    protected int optionsCount { get; set; }
    protected RectTransform cursorTrns { get; set; }
    protected Vector3 startingCursorPos { get; set; }
    protected bool isOpened { get; set; }
    public Vector2Int CursorIndex { get; protected set; }
    protected Transform[] optionsTransforms { get; set; }
    protected int menuStartIndexVert { get; set; }


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
        CursorIndex = Vector2Int.zero;
        menuStartIndexVert = 0;
    }

    public virtual void NavigateMenu(Vector2Int direction)
    {
        int nextIdx = CursorIndex.y + direction.y;
        if (nextIdx != (menuStartIndexVert - 1) && nextIdx != optionsCount) {
            CursorIndex = new Vector2Int(0, nextIdx);
            UpdateCursorPosition();
        }
    }

    public void UpdateCursorPosition()
    {
        cursorTrns.localPosition = optionsHolder.localPosition + optionsTransforms[CursorIndex.y].localPosition;
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
