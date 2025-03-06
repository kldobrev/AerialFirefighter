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
    protected Transform OptionsHolder { get; set; }
    [field: SerializeField]
    protected RawImage Cursor { get; set; }

    protected RectTransform MenuBkgRect { get; set; }
    protected TextMeshProUGUI[] OptionsSigns { get; set; }
    protected int OptionsCount { get; set; }
    protected RectTransform CursorTrns { get; set; }
    protected Vector3 StartingCursorPos { get; set; }
    public Vector2Int CursorIndex { get; protected set; }
    protected Transform[] OptionsTransforms { get; set; }
    protected int MenuStartIndexVert { get; set; }
    public bool Opened { get; protected set; }


    protected void Awake()
    {
        MenuBkgRect = transform.GetComponent<RectTransform>();
        OptionsCount = OptionsHolder.childCount;
        OptionsSigns = new TextMeshProUGUI[OptionsCount];
        OptionsTransforms = new Transform[OptionsCount];
        for (int i = 0; i < OptionsCount; i++)
        {
            OptionsSigns[i] = OptionsHolder.GetChild(i).GetComponent<TextMeshProUGUI>();
            OptionsTransforms[i] = OptionsSigns[i].transform;
        }
        CursorTrns = Cursor.transform.GetComponent<RectTransform>();
        CursorIndex = Vector2Int.zero;
        MenuStartIndexVert = 0;
    }

    public virtual void NavigateMenu(Vector2Int direction)
    {
        int nextIdx = CursorIndex.y + direction.y;
        if (nextIdx != (MenuStartIndexVert - 1) && nextIdx != OptionsCount) {
            CursorIndex = new Vector2Int(0, nextIdx);
            UpdateCursorPosition();
        }
    }

    public void ResetCursorPosition()
    {
        CursorIndex = new Vector2Int(0, MenuStartIndexVert);
    }

    public void UpdateCursorPosition()
    {
        CursorTrns.localPosition = OptionsHolder.localPosition + OptionsTransforms[CursorIndex.y].localPosition;
    }

    protected IEnumerator FadeOptions(float minAlpha, float maxAlpha, float speed)
    {
        for (int i = 0; i < OptionsCount; i++)
        {
            StartCoroutine(HelperMethods.FadeText(OptionsSigns[i], minAlpha, maxAlpha, speed));
        }
        yield return null;
    }

    
}
