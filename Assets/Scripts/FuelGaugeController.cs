using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FuelGaugeController : MonoBehaviour
{
    [SerializeField]
    private RectTransform _arrowTrns;
    [SerializeField]
    private RawImage _background;
    [SerializeField]
    private RawImage _arrow;
    [SerializeField]
    private RawImage _measurementsPanel;

    private float _capacity;
    private float _quantity;
    private float _capToQtityRatio;
    private byte _currentBkgAlpha;
    private IEnumerator _fuelAlert;
    private bool _alertActivated;


    public void SetFuelCapacity(float cap)
    {
        _capacity = cap > 0 ? cap : 0;
        _alertActivated = false;
    }

    public void UpdateFuelQuantity(float qtity)
    {
        _quantity = Mathf.Clamp(qtity, 0, _capacity);
        _capToQtityRatio = _quantity / _capacity;
        _arrowTrns.rotation = Quaternion.Euler(0, 0, -180 * _capToQtityRatio);

        if(_capToQtityRatio == 0)
        {
            StopCoroutine(_fuelAlert);
            _background.color = new Color32(0, 0, 0, 0);
            _measurementsPanel.color = Constants.FuelGaugeEmptyColour;
            _arrow.color = Constants.FuelGaugeEmptyColour;
        }
        else if (_capToQtityRatio < 0.25 && !_alertActivated)
        {
            _currentBkgAlpha = Constants.FuelGaugeColor.a;
            _fuelAlert = LowFuelAlertFlash();
            _alertActivated = true;
            StartCoroutine(_fuelAlert);
        }
    }

    private IEnumerator LowFuelAlertFlash()
    {
        byte alphaModifier = 1;
        while (true)
        {
            _currentBkgAlpha += alphaModifier;
            _background.color = new Color32(Constants.FuelGaugeColor.r, Constants.FuelGaugeColor.g, 
                Constants.FuelGaugeColor.b, _currentBkgAlpha);
            if (_currentBkgAlpha == Constants.FuelGaugeAlertAlpha)
            {
                alphaModifier = 255;
            }
            else if (_currentBkgAlpha == Constants.FuelGaugeNormalAlpha)
            {
                alphaModifier = 1;
            }
            yield return null;
        }
    }

}
