using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaterGaugeController : MonoBehaviour
{
    [SerializeField]
    private Image fill;

    private float capacity;
    private float quantity;
    private float capToQtityRatio;


    public void SetWaterCapacity(float cap)
    {
        capacity = cap > 0 ? cap : 0;
    }

    public void UpdateWaterQuantity(float qtity)
    {
        quantity = Mathf.Clamp(qtity, 0, capacity);
        capToQtityRatio = quantity / capacity;
        fill.fillAmount = capToQtityRatio;
    }

}
