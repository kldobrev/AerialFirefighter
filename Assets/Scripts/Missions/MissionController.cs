using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionController : MonoBehaviour
{
    [field: SerializeField]
    protected GameObject landingGuide { get; set; }
    [field: SerializeField]
    protected GameObject goalSphere { get; set; }
    [field: SerializeField]
    protected GameObject landingField { get; set; }
    [field: SerializeField]
    protected UnityEvent<bool> updateMissionResults { get; set; }
    [field: SerializeField]
    protected UnityEvent startFadeOut { get; set; }

    protected float missionTimer { get; set; }
    protected int score { get; set; }
    protected IEnumerator timeTracker { get; set; }
    protected bool missionPassed { get; set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        missionTimer = 0;
        score = 0;
        timeTracker = TrackTime();
        StartCoroutine(timeTracker);
    }

    public void FailMission()
    {
        missionPassed = false;
    }

    public virtual void EndMission(bool landed)
    {
        StopCoroutine(timeTracker);
        updateMissionResults?.Invoke(missionPassed);
    }

    protected IEnumerator TrackTime()
    {
        WaitForSeconds waitPeriod = new WaitForSeconds(1);
        while (true)
        {
            missionTimer += 1;
            yield return waitPeriod;
        }
    }

}
