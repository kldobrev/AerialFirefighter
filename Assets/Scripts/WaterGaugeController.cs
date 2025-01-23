using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class WaterGaugeController : MonoBehaviour
{
    [SerializeField]
    private Image _fill;

    private float _capacity;
    private float _quantity;
    private float _capToQuantityRatio;
    private IEnumerator waterGaugeFade;


    public void SetWaterCapacity(float cap)
    {
        _capacity = cap > 0 ? cap : 0;
    }

    public void UpdateWaterQuantity(float qtity)
    {
        _quantity = Mathf.Clamp(qtity, 0, _capacity);
        _capToQuantityRatio = _quantity / _capacity;
        _fill.fillAmount = _capToQuantityRatio;
    }

    public void ChangeWaterGaugeColour(bool enabled)
    {
        if (!waterGaugeFade.IsUnityNull()) StopCoroutine(waterGaugeFade);
        waterGaugeFade = HelperMethods.FadeImage(_fill, Constants.WaterGaugeAlphaPouring, Constants.WaterGaugeAlphaDefault, 
            (enabled ? -1 : 1) * Constants.WaterGaugeAlphaChangeSpeed);
        StartCoroutine(waterGaugeFade);
    }

}
