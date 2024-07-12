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
    [SerializeField]
    private RawImage arrow;
    [SerializeField]
    private RawImage measurementsPanel;

    private float capacity;
    private float quantity;
    private float capToQtityRatio;
    private byte [] currentBkgColour;
    private IEnumerator fuelAlert;


    public void SetFuelCapacity(float cap)
    {
        capacity = cap > 0 ? cap : 0;
    }

    public void UpdateFuelQuantity(float qtity)
    {
        quantity = Mathf.Clamp(qtity, 0, capacity);
        capToQtityRatio = quantity / capacity;
        arrowTrns.rotation = Quaternion.Euler(0, 0, -180 * capToQtityRatio);

        if(capToQtityRatio == 0)
        {
            StopCoroutine(fuelAlert);
            background.color = new Color32(0, 0, 0, 0);
            measurementsPanel.color = Constants.FuelGaugeEmptyColour;
            arrow.color = Constants.FuelGaugeEmptyColour;
        }
        else if (capToQtityRatio < 0.25 && currentBkgColour.IsUnityNull())
        {
            currentBkgColour = new Byte[4];
            currentBkgColour[0] = Constants.FuelGaugeColor.r;
            currentBkgColour[1] = Constants.FuelGaugeColor.g;
            currentBkgColour[2] = Constants.FuelGaugeColor.b;
            currentBkgColour[3] = Constants.FuelGaugeColor.a;
            fuelAlert = LowFuelAlertFlash();
            StartCoroutine(fuelAlert);
        }
    }

    private IEnumerator LowFuelAlertFlash()
    {
        byte alphaModifier = 1;
        while (true)
        {
            currentBkgColour[3] += alphaModifier;
            background.color = new Color32(currentBkgColour[0], currentBkgColour[1], currentBkgColour[2], currentBkgColour[3]);
            if (currentBkgColour[3] == Constants.FuelGaugeAlertAlpha)
            {
                alphaModifier = 255;
            }
            else if (currentBkgColour[3] == Constants.FuelGaugeNormalAlpha)
            {
                alphaModifier = 1;
            }
            yield return null;
        }
    }

}
