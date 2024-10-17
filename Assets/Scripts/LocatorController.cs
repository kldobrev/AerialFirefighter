using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class LocatorController : MonoBehaviour
{
    [SerializeField]
    private RectTransform _locatorRectTransform;
    [SerializeField]
    private Transform _playerTransform;
    [SerializeField]
    private Transform _firesTransforms;
    [SerializeField]
    private Transform _airportTransform;
    [SerializeField]
    private Transform _goalSphereTransform;
    [SerializeField]
    private RectTransform _barRectTransform;
    [SerializeField]
    private GameObject _airportIconPrefab;
    [SerializeField]
    private GameObject _flagIconPrefab;
    [SerializeField]
    private GameObject _fireIconPrefab;
    [SerializeField]
    private Transform _iconsHolder;

    private float _playerAngleY;
    private float _locatorAngle;
    private float _startBarPositionX;
    private float _startTargetIconPositionX;
    private float _playerTargetAngle;
    private float _locatorHalfWidth;
    private Vector3 _nextBarPosition;
    private Vector3 _nextTargetIconPosition;
    private Vector3 _direction;
    private Vector3 _playerForward;

    private static List<Transform> _targets;
    private static List<Transform> _icons;
    private static int _removeIdx;

    // Start is called before the first frame update
    void Start()
    {
        _playerAngleY = 0;
        _locatorAngle = 0;
        _removeIdx = -1;
        _locatorHalfWidth = _locatorRectTransform.rect.size.x / 2;
        _startBarPositionX = _barRectTransform.localPosition.x;
        _nextBarPosition = _barRectTransform.localPosition;
        _startTargetIconPositionX = Constants.DefaultLocatorIconPosition.x;
        _nextTargetIconPosition = Constants.DefaultLocatorIconPosition;

        _targets = new();
        _icons = new();
        for (int i = 0; i < _firesTransforms.childCount; i++)
        {
            Transform icon = Instantiate(_fireIconPrefab, _iconsHolder).transform;
            _icons.Add(icon);
            _targets.Add(_firesTransforms.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTransform != null)
        {
            _locatorAngle = HelperMethods.GetSignedAngleFromEuler(_playerAngleY);
            _nextBarPosition.x = _startBarPositionX - _locatorAngle;
            if (_locatorAngle < -Constants.BarResetBorder || _locatorAngle > Constants.BarResetBorder)  // Resetting bar image position
            {
                _nextBarPosition.x += (_locatorAngle > Constants.BarResetBorder ? -Constants.BarResetBorder : Constants.BarResetBorder);
            }
            _barRectTransform.localPosition = _nextBarPosition;

            if (_targets.Count != 0)
            {
                for (int i = 0; i < _targets.Count; i++)
                {
                    if (i == _removeIdx) continue;   // Current element is being removed
                    Transform tracked = _targets[i];
                    if (!tracked.IsUnityNull())
                    {
                        MoveIcon(tracked, i);
                    }
                }
            }
            _playerAngleY = _playerTransform.rotation.eulerAngles.y;
        }
    }

    private void MoveIcon(Transform target, int iconIdx)
    {
        _direction = target.position - _playerTransform.position;
        _playerForward = _playerTransform.forward;
        _direction.y = _playerForward.y = 0;  // Excluding vertical coordinates from calculations
        _playerTargetAngle = Vector3.SignedAngle(_direction, _playerForward, Vector3.up);
        _nextTargetIconPosition.x = _startTargetIconPositionX - ((_playerTargetAngle / 180) * _locatorHalfWidth);
        _nextTargetIconPosition.x = Mathf.Clamp(_nextTargetIconPosition.x, _locatorRectTransform.rect.xMin, _locatorRectTransform.rect.xMax);
        try
        {
            _icons[iconIdx].localPosition = _nextTargetIconPosition;
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.Log("Locator controller trying to access an element that was just deleted. Lookup index: " + iconIdx);
        }
    }

    public void AddGoalIcons()
    {
        Transform airportIcon = Instantiate(_airportIconPrefab, _iconsHolder).transform;
        _icons.Add(airportIcon);
        _targets.Add(_airportTransform);
        Transform flagIcon = Instantiate(_flagIconPrefab, _iconsHolder).transform;
        _icons.Add(flagIcon);
        _targets.Add(_goalSphereTransform);
    }

    public void RemoveIcon(int iconIdx)
    {
        StartCoroutine(RemoveIconCoroutine(iconIdx));
    }

    public void RemoveGoalIcons()
    {
        StartCoroutine(RemoveGoalIconsCoroutine());
    }

    private IEnumerator RemoveGoalIconsCoroutine()
    {
        yield return StartCoroutine(RemoveIconCoroutine(1));
        yield return StartCoroutine(RemoveIconCoroutine(0));
    }

    private IEnumerator RemoveIconCoroutine(int idx)
    {
        _removeIdx = idx;
        Destroy(_icons.ElementAt(idx).gameObject);
        _icons.RemoveAt(idx);
        _targets.RemoveAt(idx);
        _removeIdx = -1;
        yield return null;
    }

}
