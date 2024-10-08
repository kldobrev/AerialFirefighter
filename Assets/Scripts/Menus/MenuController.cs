using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    protected Transform optionsHolder;
    [SerializeField]
    protected RawImage selector;

    protected RectTransform menuBkgRect;
    protected TextMeshProUGUI[] optionsSigns;
    protected int optionsCount;
    protected RectTransform selectorTrns;
    protected Vector3 selectorPos;
    protected bool isOpened;
    public bool Visible => isOpened;


    protected void Awake()
    {
        menuBkgRect = transform.GetComponent<RectTransform>();
        optionsCount = optionsHolder.childCount;
        optionsSigns = new TextMeshProUGUI[optionsCount];
        for (int i = 0; i < optionsCount; i++)
        {
            optionsSigns[i] = optionsHolder.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
        selectorTrns = selector.transform.GetComponent<RectTransform>();
        selectorPos = selectorTrns.localPosition;
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
