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
    private GameObject exactPointer;
    [SerializeField]
    private RawImage trackerImage;
    [SerializeField]
    private TextMeshProUGUI trackerDistanceSign;
    [SerializeField]
    private Transform playerPoint;

    private Vector2 targetTrackerPosition;
    private GameObject targetTrackerObject;
    private Vector3 objectViewPosition;
    private int radarObjectsLayer;
    private bool trackerActivated;
    private Transform targetPoint;
    private Image trackerMissileBarFill;
    private bool missileLockingEnabled;
    private RaycastHit [] targetsHits;
    private Ray targetsRay;
    private float missileBarFillStep;


    // Start is called before the first frame update
    void Start()
    {
        targetTrackerObject = targetTracker.gameObject;
        trackerDistanceSign.text = "999999";
        trackerActivated = true;
        radarObjectsLayer = LayerMask.GetMask(Constants.RadarPointLayerName);
        targetsHits = new RaycastHit[2];
        trackerImage.color = Constants.AllyColour;
        trackerDistanceSign.color = Constants.AllyColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (trackerActivated)
        {
            if (targetPoint.IsUnityNull())
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
            if (activate) obj.GetComponent<RawImage>().color = Constants.AllyColour;
            obj.SetActive(activate);
        }
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
                if(!targetTrackerObject.activeSelf)
                {
                    targetTrackerObject.SetActive(true);
                }
                else
                {
                    targetTrackerPosition = mainCamera.ViewportToScreenPoint(objectViewPosition);
                    targetTracker.position = targetTrackerPosition;
                    trackerDistanceSign.text = Mathf.Round(objectViewPosition.z).ToString();
                }
            }
        }
        else if(targetTrackerObject.activeSelf)
        {
            SetActiveAndColour(targetTrackerObject, false);
        }

    }

    public void StopTracking()
    {
        foreach (GameObject arrow in trackerArrows)
        {
            arrow.SetActive(false);
        }
        targetTrackerObject.SetActive(false);
        trackerActivated = false;
    }

    public void ChangeTrackedTarget(Transform tgt)
    {
        targetPoint = tgt;
    }

    public void ToggleTracker()
    {
        if(trackerActivated)
        {
            StopTracking();
        }
        else
        {
            trackerActivated = true;
        }
    }

}
