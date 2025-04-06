using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    protected RawImage _fadeEffectsPanel;
    public static bool ScreenFadeInProgress { get; protected set; }
    public static byte ScreenAlpha { get; protected set; }


    protected void Start()
    {
        ScreenFadeInProgress = false;
        ScreenAlpha = (byte) _fadeEffectsPanel.color.a;
    }

    public void StartScreenFade(byte minAlpha, byte maxAlpha, float speed)
    {
        StartCoroutine(ScreenFade(minAlpha, maxAlpha, speed));
    }

    protected IEnumerator ScreenFade(byte minAlpha, byte maxAlpha, float fadeSpeed)
    {
        if (ScreenFadeInProgress) yield break;
        ScreenFadeInProgress = true;
        byte alphaTarget = fadeSpeed > 0 ? maxAlpha : minAlpha;
        while (ScreenAlpha != alphaTarget)
        {
            ScreenAlpha = (byte)Math.Clamp(ScreenAlpha + fadeSpeed, minAlpha, maxAlpha);
            _fadeEffectsPanel.color = new Color32(Constants.EffectsPanelColourDefault.r,
                Constants.EffectsPanelColourDefault.g, Constants.EffectsPanelColourDefault.b, ScreenAlpha);
            yield return null;
        }
        ScreenFadeInProgress = false;
    }

}
