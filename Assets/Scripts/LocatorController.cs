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
    private RectTransform locatorRectTrns;
    [SerializeField]
    private Transform playerTrns;
    [SerializeField]
    private Transform firesTrns;
    [SerializeField]
    private Transform airportTrns;
    [SerializeField]
    private Transform goalSphereTrns;
    [SerializeField]
    private RectTransform barRectTrns;
    [SerializeField]
    private GameObject airportIconPrefab;
    [SerializeField]
    private GameObject flagIconPrefab;
    [SerializeField]
    private GameObject fireIconPrefab;
    [SerializeField]
    private Transform iconsHolder;

    private float playerAngleY;
    private float locatorAngle;
    private float startBarPositionX;
    private float startTargetIconPositionX;
    private float playerTargetAngle;
    private float locatorHalfWidth;
    private Vector3 nextBarPosition;
    private Vector3 nextTargetIconPosition;
    private Vector3 direction;
    private Vector3 playerForward;

    private static List<Transform> targets;
    private static List<Transform> icons;
    private static int removeIdx;

    // Start is called before the first frame update
    void Start()
    {
        playerAngleY = 0;
        locatorAngle = 0;
        removeIdx = -1;
        locatorHalfWidth = locatorRectTrns.rect.size.x / 2;
        startBarPositionX = barRectTrns.localPosition.x;
        nextBarPosition = barRectTrns.localPosition;
        startTargetIconPositionX = Constants.DefaultLocatorIconPosition.x;
        nextTargetIconPosition = Constants.DefaultLocatorIconPosition;

        targets = new();
        icons = new();
        for (int i = 0; i < firesTrns.childCount; i++)
        {
            Transform icon = Instantiate(fireIconPrefab, iconsHolder).transform;
            icons.Add(icon);
            targets.Add(firesTrns.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTrns != null)
        {
            locatorAngle = HelperMethods.GetSignedAngleFromEuler(playerAngleY);
            nextBarPosition.x = startBarPositionX - locatorAngle;
            if (locatorAngle < -Constants.BarResetBorder || locatorAngle > Constants.BarResetBorder)  // Resetting bar image position
            {
                nextBarPosition.x += (locatorAngle > Constants.BarResetBorder ? -Constants.BarResetBorder : Constants.BarResetBorder);
            }
            barRectTrns.localPosition = nextBarPosition;

            if (targets.Count != 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (i == removeIdx) continue;   // Current element is being removed
                    Transform tracked = targets[i];
                    if (!tracked.IsUnityNull())
                    {
                        MoveIcon(tracked, i);
                    }
                }
            }
            /*else
            {
                MoveIcon(airportTrns, 0);
                MoveIcon(goalSphereTrns, 1);
            }*/
            playerAngleY = playerTrns.rotation.eulerAngles.y;
        }
    }

    private void MoveIcon(Transform target, int iconIdx)
    {
        direction = target.position - playerTrns.position;
        playerForward = playerTrns.forward;
        direction.y = playerForward.y = 0;  // Excluding vertical coordinates from calculations
        playerTargetAngle = Vector3.SignedAngle(direction, playerForward, Vector3.up);
        nextTargetIconPosition.x = startTargetIconPositionX - ((playerTargetAngle / 180) * locatorHalfWidth);
        nextTargetIconPosition.x = Mathf.Clamp(nextTargetIconPosition.x, locatorRectTrns.rect.xMin, locatorRectTrns.rect.xMax);
        try
        {
            icons[iconIdx].localPosition = nextTargetIconPosition;
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.Log("Locator controller trying to access an element that was just deleted. Lookup index: " + iconIdx);
        }
    }

    public void AddGoalIcons()
    {
        Transform airportIcon = Instantiate(airportIconPrefab, iconsHolder).transform;
        icons.Add(airportIcon);
        targets.Add(airportTrns);
        Transform flagIcon = Instantiate(flagIconPrefab, iconsHolder).transform;
        icons.Add(flagIcon);
        targets.Add(goalSphereTrns);
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
        removeIdx = idx;
        Destroy(icons.ElementAt(idx).gameObject);
        icons.RemoveAt(idx);
        targets.RemoveAt(idx);
        removeIdx = -1;
        yield return null;
    }

}
