using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FireMissionController : MissionController
{
    [SerializeField]
    private GameObject landingGuide;
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
    [SerializeField]
    private UnityEvent showAirportIcon;

    private int numberFiresLeft;
    private int firesExtinguishedInCombo;
    private int comboScore;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < transform.childCount; i++)
        {
            numberFiresLeft += transform.GetChild(i).childCount;
        }

        updateFireCounter.Invoke(numberFiresLeft, 0);
        firesExtinguishedInCombo = 0;
        comboScore = 0;
    }

    public void DecrementFiresCount()
    {
        numberFiresLeft--;
        if (firesExtinguishedInCombo == 0)
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
            EnableRunway();
        }
    }

    private void EnableRunway()
    {
        landingGuide.SetActive(true);
        showAirportIcon.Invoke();
    }

    private IEnumerator StartComboTimer()
    {
        yield return new WaitForSeconds(Constants.ExtinguishedComboTime);
        firesExtinguishedInCombo = 0;
        score += comboScore;
        updateScoreSign.Invoke(score);
        comboScore = 0;
        hideExtinguishedSign.Invoke();
        hideAddToScoreSign.Invoke();
    }
}
