using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionController : MonoBehaviour
{
    [SerializeField]
    protected UnityEvent<bool> updateMissionResults;
    [SerializeField]
    protected UnityEvent startFadeOut;

    protected float missionTimer;
    protected int score;
    protected IEnumerator timeTracker;
    protected bool missionPassed;

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
        EndMission();
    }

    protected void EndMission()
    {
        StopCoroutine(timeTracker);
        updateMissionResults.Invoke(missionPassed);
        startFadeOut.Invoke();
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
