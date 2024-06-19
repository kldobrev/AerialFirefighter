using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FuelGaugeController : MonoBehaviour
{
    [SerializeField]
    private RectTransform arrowTrns;
    [SerializeField]
    private RawImage background;

    private float capacity;
    private float quantity;
    private float capToQtityRatio;
    private byte [] currentColour;

    public void SetFuelCapacity(float cap)
    {
        capacity = cap > 0 ? cap : 0;
    }

    public void UpdateFuelQuantity(float qtity)
    {
        quantity = Mathf.Clamp(qtity, 0, capacity);
        capToQtityRatio = quantity / capacity;
        arrowTrns.rotation = Quaternion.Euler(0, 0, -180 * capToQtityRatio);

        if (capToQtityRatio < 0.25 && currentColour.IsUnityNull())
        {
            currentColour = new Byte[4];
            currentColour[0] = Constants.FuelGaugeColor.r;
            currentColour[1] = Constants.FuelGaugeColor.g;
            currentColour[2] = Constants.FuelGaugeColor.b;
            currentColour[3] = Constants.FuelGaugeColor.a;
            StartCoroutine(LowFuelAlertFlash());
        }
    }

    private IEnumerator LowFuelAlertFlash()
    {
        byte alphaModifier = 1;
        while (true)
        {
            currentColour[3] += alphaModifier;
            background.color = new Color32(currentColour[0], currentColour[1], currentColour[2], currentColour[3]);
            if (currentColour[3] == Constants.FuelGaugeAlertAlpha)
            {
                alphaModifier = 255;
            }
            else if (currentColour[3] == Constants.FuelGaugeNormalAlpha)
            {
                alphaModifier = 1;
            }
            yield return null;
        }
    }
}
