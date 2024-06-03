using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;


public class LocatorController : MonoBehaviour
{
    [SerializeField]
    private RectTransform locatorRectTrns;
    [SerializeField]
    private Transform playerTrns;
    [SerializeField]
    private RectTransform barRectTrns;
    [SerializeField]
    private RectTransform targetIcon;

    private Transform targetTrns;
    private float playerAngleY;
    private float locatorAngle;
    private float startBarPositionX;
    private float startTargetIconPositionX;
    private float playerTargetAngle;
    private float angleDelta;
    private Vector3 nextBarPosition;
    private Vector3 nextTargetIconPosition;
    private Vector3 direction;
    private Vector3 playerForward;


    // Start is called before the first frame update
    void Start()
    {
        playerAngleY = 0;
        locatorAngle = 0;
        startBarPositionX = barRectTrns.localPosition.x;
        nextBarPosition = barRectTrns.localPosition;
        startTargetIconPositionX = targetIcon.localPosition.x;
        nextTargetIconPosition = targetIcon.localPosition;
        angleDelta = 0;
    }

    // Update is called once per frame
    void Update()
    {
        playerAngleY = playerTrns.rotation.eulerAngles.y;
        if (locatorAngle != playerAngleY)
        {
            locatorAngle = playerAngleY > 180 ? playerAngleY - 360 : playerAngleY;
            angleDelta = locatorAngle - angleDelta;
            nextBarPosition.x = startBarPositionX - locatorAngle;
            if (locatorAngle < -Constants.BarResetBorder || locatorAngle > Constants.BarResetBorder)  // Resetting bar image position
            {
                nextBarPosition.x += (locatorAngle > Constants.BarResetBorder ? -Constants.BarResetBorder : Constants.BarResetBorder);
            }
            barRectTrns.localPosition = nextBarPosition;

            direction = targetTrns.position - playerTrns.position;
            playerForward = playerTrns.forward;
            direction.y = playerForward.y = 0;  // Excluding vertical coordinates from calculations
            playerTargetAngle = Vector3.SignedAngle(direction, playerForward, Vector3.up);
            nextTargetIconPosition.x = startTargetIconPositionX - (playerTargetAngle * Constants.TargetIconMovementMult);
            nextTargetIconPosition.x = Mathf.Clamp(nextTargetIconPosition.x, locatorRectTrns.rect.xMin, locatorRectTrns.rect.xMax);
            targetIcon.localPosition = nextTargetIconPosition;
        }
    }

    public void UpdateLocatorTarget(Transform trns)
    {
        targetTrns = trns;
    }
}
