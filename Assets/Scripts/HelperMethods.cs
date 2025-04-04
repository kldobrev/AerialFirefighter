using TMPro;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;


public static class HelperMethods
{
    public static float GetSignedAngleFromEuler(float eulerAngle)
    {
        return eulerAngle > 180 ? eulerAngle - 360 : eulerAngle;
    }

    public static IEnumerator FadeText(TextMeshProUGUI sign, float minAlpha, float maxAlpha, float fadeSpeed)
    {
        Color32 signColour = sign.color;
        float signTargetAlpha = fadeSpeed > 0 ? maxAlpha : minAlpha;
        while (signColour.a != signTargetAlpha)
        {
            signColour.a = (byte)Math.Clamp(signColour.a + fadeSpeed, minAlpha, maxAlpha);
            sign.color = signColour;
            yield return null;
        }
    }

    public static IEnumerator FadeImage(MaskableGraphic image, float minAlpha, float maxAlpha, float fadeSpeed)
    {
        Color32 imageColour = image.color;
        float signTargetAlpha = fadeSpeed > 0 ? maxAlpha : minAlpha;
        while (imageColour.a != signTargetAlpha)
        {
            imageColour.a = (byte)Math.Clamp(imageColour.a + fadeSpeed, minAlpha, maxAlpha);
            image.color = imageColour;
            yield return null;
        }
    }

    public static IEnumerator TransitionTextSize(TextMeshProUGUI sign, float signSizeMin, float signSizeMax,
        float speed)
    {
        float signTargetSize = speed > 0 ? signSizeMax : signSizeMin;
        while (sign.fontSize != signTargetSize)
        {
            sign.fontSize = Mathf.Clamp(sign.fontSize + speed, signSizeMin, signSizeMax);
            yield return null;
        }
    }

    public static IEnumerator TransitionHeightUI(RectTransform uiElem, float minSize, float maxSize, float speed)
    {
        float targetSize = speed > 0 ? maxSize : minSize;
        float newHeight = uiElem.rect.height;
        float startWidth = uiElem.sizeDelta.x;
        while (newHeight != targetSize)
        {
            newHeight = Mathf.Clamp(newHeight + speed, minSize, maxSize);
            uiElem.sizeDelta = new Vector2(startWidth, newHeight);
            yield return null;
        }
    }

    public static IEnumerator MoveCanvasRectTransform(RectTransform uiElem, Vector2 endPos, Vector3 speed, float maxDistAllowed = 0.1f)
    {
        while (Vector2.Distance(uiElem.position, endPos) > maxDistAllowed)
        {
            uiElem.localPosition += speed;
            yield return null;
        }
    }

}