using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FireMissionController : MissionController
{
    [SerializeField]
    private UnityEvent<int, int> updateFireCounter;
    [SerializeField]
    private UnityEvent hideExtinguishedSign;
    [SerializeField]
    private UnityEvent<int> updateScoreSign;
    [SerializeField]
    private UnityEvent<int> updateAddedScoreSign;
    [SerializeField]
    private UnityEvent hideAddToScoreSign;

    private int numberFiresLeft;
    private float comboTimer;
    private int firesExtinguishedInCombo;
    private int comboScore;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        numberFiresLeft = transform.childCount;
        updateFireCounter.Invoke(numberFiresLeft, 0);
        comboTimer = 0;
        firesExtinguishedInCombo = 0;
        comboScore = 0;
    }

    public void DecrementFiresCount()
    {
        numberFiresLeft--;
        comboTimer = 0;
        if(firesExtinguishedInCombo == 0)
        {
            StartCoroutine(StartComboTimer());
        }
        firesExtinguishedInCombo++;
        updateFireCounter.Invoke(numberFiresLeft, firesExtinguishedInCombo);
        comboScore += (firesExtinguishedInCombo * Constants.SingleExtinguishScore);
        updateAddedScoreSign.Invoke(comboScore);

        if (numberFiresLeft == 0)
        {
            missionPassed = true;
            EndMission();
        }
    }

    private IEnumerator StartComboTimer()
    {
        while (comboTimer < Constants.ExtinguishedComboTime)
        {
            comboTimer += Time.deltaTime;
            yield return null;
        }
        comboTimer = 0;
        firesExtinguishedInCombo = 0;
        score += comboScore;
        updateScoreSign.Invoke(score);
        comboScore = 0;
        hideExtinguishedSign.Invoke();
        hideAddToScoreSign.Invoke();
    }
}
