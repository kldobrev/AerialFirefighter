using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrackerController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject[] trackerArrows;
    [SerializeField]
    private Transform targetTracker;
    [SerializeField]
    private GameObject missileLockedBar;
    [SerializeField]
    private GameObject missileLockedSign;
    [SerializeField]
    private GameObject exactPointer;
    [SerializeField]
    private RawImage trackerImage;
    [SerializeField]
    private TextMeshProUGUI trackerDistanceSign;
    [SerializeField]
    private Transform playerPoint;
    [SerializeField]
    private Transform crshPositionTransform;
    [SerializeField]
    private UnityEvent<Transform> sendMissileTarget;

    private Vector2 targetTrackerPosition;
    private GameObject targetTrackerObject;
    private Vector3 objectViewPosition;
    private float trackableDistanceMax;
    private float trackedDistance;
    private int radarObjectsLayer;
    private Color32 activeColour;
    private bool trackerActivated;
    private bool isTargetEnemy;
    private Transform targetPoint;
    private Image trackerMissileBarFill;
    private float missileBarFillAmmount;
    private bool missileLockingEnabled;
    private RaycastHit [] targetsHits;
    private Ray targetsRay;
    private float missileBarFillStep;


    // Start is called before the first frame update
    void Start()
    {
        targetTrackerObject = targetTracker.gameObject;
        trackerDistanceSign.text = "99999";
        trackableDistanceMax = Constants.CoveredDistance / 2;
        trackerActivated = false;
        isTargetEnemy = false;
        radarObjectsLayer = LayerMask.GetMask(Constants.RadarPointLayerName);
        targetsHits = new RaycastHit[2];
        trackerMissileBarFill = missileLockedBar.GetComponentInChildren<Image>();
        trackerMissileBarFill.fillAmount = 0;
        missileBarFillAmmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (trackerActivated)
        {
            if (targetPoint.IsUnityNull() || !IsInPlayerRange())
            {
                StopTracking();
            }
            else
            {
                DisplayPointOnTracker();
            }
        }
    }

    private void SetActiveAndColour(GameObject obj, bool activate)
    {
        if (obj.activeSelf != activate)
        {
            if (activate) obj.GetComponent<RawImage>().color = isTargetEnemy ? Constants.EnemyColour : Constants.AllyColour;
            obj.SetActive(activate);
        }
    }

    private bool IsInPlayerRange()
    {   
        return objectViewPosition.z < Constants.CoveredDistance;
    }

    private void DisplayPointOnTracker()
    {

        objectViewPosition = mainCamera.WorldToViewportPoint(targetPoint.position);
        if (objectViewPosition.y > 1)    // Object is to the top of the screen
        {
            SetActiveAndColour(trackerArrows[0], true);
            SetActiveAndColour(trackerArrows[1], false);
        }
        else if (objectViewPosition.y < -1)    // Object is to the bottom of the screen
        {
            SetActiveAndColour(trackerArrows[1], true);
            SetActiveAndColour(trackerArrows[0], false);
        }
        else
        {
            SetActiveAndColour(trackerArrows[0], false);
            SetActiveAndColour(trackerArrows[1], false);
        }

        if (objectViewPosition.x > 1)    // Object is to the right of the screen
        {
            SetActiveAndColour(trackerArrows[3], true);
            SetActiveAndColour(trackerArrows[2], false);
        }
        else if (objectViewPosition.x < -1)    // Object is to the left of the screen
        {
            SetActiveAndColour(trackerArrows[2], true);
            SetActiveAndColour(trackerArrows[3], false);
        }
        else
        {
            SetActiveAndColour(trackerArrows[2], false);
            SetActiveAndColour(trackerArrows[3], false);
        }

        if (objectViewPosition.y > 0 && objectViewPosition.y < 1 && objectViewPosition.x > 0 && objectViewPosition.x < 1)
        {
            if (objectViewPosition.z > 0)    // Object is in viewing range
            {
                if (objectViewPosition.z <= trackedDistance && trackerImage.color != activeColour)
                {
                    trackerImage.color = activeColour;
                } 
                else if (objectViewPosition.z > trackedDistance && trackerImage.color != Constants.OutOfRangeColour)
                {
                    trackerImage.color = Constants.OutOfRangeColour;
                }

                if(!targetTrackerObject.activeSelf)
                {
                    trackerDistanceSign.color = activeColour;
                    targetTrackerObject.SetActive(true);
                }
                else
                {
                    targetTrackerPosition = mainCamera.ViewportToScreenPoint(objectViewPosition);
                    targetTracker.position = targetTrackerPosition;
                    trackerDistanceSign.text = Mathf.Round(objectViewPosition.z).ToString();
                    if (missileLockingEnabled && objectViewPosition.z <= trackedDistance) UpdateMissileLockBar();
                }
            }
        }
        else if(targetTrackerObject.activeSelf)
        {
            SetActiveAndColour(targetTrackerObject, false);
            missileLockedBar.SetActive(false);
            missileBarFillAmmount = 0;
            sendMissileTarget.Invoke(null);
        }

    }

    private void UpdateMissileLockBar()
    {
        if (objectViewPosition.y > Constants.MissileBarUpdBoundLow && objectViewPosition.y < Constants.MissileBarUpdBoundHigh && 
            objectViewPosition.x > Constants.MissileBarUpdBoundLow && objectViewPosition.x < Constants.MissileBarUpdBoundHigh)
        {
            if (missileBarFillAmmount == 1 && !missileLockedSign.activeSelf)
            {
                sendMissileTarget.Invoke(targetPoint);
                missileLockedBar.SetActive(false);
                missileLockedSign.SetActive(true);
            } 
            else if(missileBarFillAmmount == 0)
            {
                missileLockedBar.SetActive(true);
            }
            missileBarFillAmmount = Mathf.Clamp01(missileBarFillAmmount + missileBarFillStep);
        }
        else
        {
            if (missileBarFillAmmount == 0 && missileLockedBar.activeSelf)  missileLockedBar.SetActive(false);
            else if (missileBarFillAmmount == 1)
            {
                sendMissileTarget.Invoke(null);
                missileLockedSign.SetActive(false);
                missileLockedBar.SetActive(true);
            }
            missileBarFillAmmount = Mathf.Clamp01(missileBarFillAmmount - missileBarFillStep);
        }
        trackerMissileBarFill.fillAmount = missileBarFillAmmount;
    }

    public void TrackTarget()
    {
        targetsRay = new Ray(playerPoint.position, playerPoint.TransformDirection(Vector3.forward));
        int numDetected = Physics.RaycastNonAlloc(targetsRay, targetsHits, trackableDistanceMax, radarObjectsLayer);
        if (numDetected != 0)
        {
            for (int i = 0; i != targetsHits.Count(); i++)
            {
                if (!trackerActivated  || (targetsHits[i].collider != null && 
                    !GameObject.ReferenceEquals(targetPoint, targetsHits[i].collider.transform)))
                {
                    targetPoint = targetsHits[i].collider.transform;
                    trackerActivated = true;
                    isTargetEnemy = targetPoint.CompareTag(Constants.EnemyPointTagName);
                    activeColour = isTargetEnemy ? Constants.EnemyColour: Constants.AllyColour;
                    break;
                }
            }
        }
    }

    public void StopTracking()
    {
        foreach (GameObject arrow in trackerArrows)
        {
            arrow.SetActive(false);
        }
        if(missileLockedSign.activeSelf) missileLockedSign.SetActive(false);
        targetTrackerObject.SetActive(false);
        trackerActivated = false;
        missileBarFillAmmount = 0;
    }

    public void SyncWithWeapon(bool isMissile, float dist, float lockStep)
    {
        missileLockingEnabled = isMissile;
        trackedDistance = dist;
        missileBarFillAmmount = 0;
        missileBarFillStep = lockStep;
        if(!isMissile)
        {
            if (missileLockedSign.activeSelf)
            {
                missileLockedSign.SetActive(false);
            }
            else if (missileLockedBar.activeSelf)
            {
                missileLockedBar.SetActive(false);
            }
        }
    }

}
