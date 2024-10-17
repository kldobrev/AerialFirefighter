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
    private Image _fill;

    private float _capacity;
    private float _quantity;
    private float _capToQuantityRatio;


    public void SetWater_capacity(float cap)
    {
        _capacity = cap > 0 ? cap : 0;
    }

    public void UpdateWater_quantity(float qtity)
    {
        _quantity = Mathf.Clamp(qtity, 0, _capacity);
        _capToQuantityRatio = _quantity / _capacity;
        _fill.fillAmount = _capToQuantityRatio;
    }

}
