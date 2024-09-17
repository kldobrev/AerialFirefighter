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
    private RectTransform barRectTrns;
    [SerializeField]
    private GameObject runwayIconPrefab;
    [SerializeField]
    private GameObject fireIconPrefab;
    [SerializeField]
    private Transform fireIconsHolder;

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

    private static List<GameObject> targets;



    // Start is called before the first frame update
    void Start()
    {
        playerAngleY = 0;
        locatorAngle = 0;
        locatorHalfWidth = locatorRectTrns.rect.size.x / 2;
        startBarPositionX = barRectTrns.localPosition.x;
        nextBarPosition = barRectTrns.localPosition;
        startTargetIconPositionX = Constants.DefaultLocatorIconPosition.x;
        nextTargetIconPosition = Constants.DefaultLocatorIconPosition;

        targets = new();
        for (int i = 0; i < firesTrns.childCount; i++)
        {
            GameObject icon = Instantiate(fireIconPrefab, fireIconsHolder);
            targets.Add(icon);
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

            for (int i = 0; i < firesTrns.childCount; i++)
            {
                Transform targetTrns = firesTrns.GetChild(i);
                direction = targetTrns.position - playerTrns.position;
                playerForward = playerTrns.forward;
                direction.y = playerForward.y = 0;  // Excluding vertical coordinates from calculations
                playerTargetAngle = Vector3.SignedAngle(direction, playerForward, Vector3.up);
                nextTargetIconPosition.x = startTargetIconPositionX - ((playerTargetAngle / 180) * locatorHalfWidth);
                nextTargetIconPosition.x = Mathf.Clamp(nextTargetIconPosition.x, locatorRectTrns.rect.xMin, locatorRectTrns.rect.xMax);
                try
                {
                    targets[i].transform.localPosition = nextTargetIconPosition;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.Log("Locator controller trying to access an element that was just deleted.");
                }
            }
            playerAngleY = playerTrns.rotation.eulerAngles.y;
        }
    }

    public void RemoveFireIcon(int groupIdx)
    {
        Destroy(targets.ElementAt(groupIdx));
        targets.RemoveAt(groupIdx);
    }

}
