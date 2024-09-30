using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField]
    private UnityEvent showGoalIcons;
    [SerializeField]
    private UnityEvent removeGoalIcons;
    [SerializeField]
    private UnityEvent<string> initStageClear;
    [SerializeField]
    private UnityEvent<int> removeGroupFormLocator;

    private int numberFiresLeft;
    private int firesExtinguishedInCombo;
    private int comboScore;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform group = transform.GetChild(i);
            numberFiresLeft += group.childCount;
            for (int j = 0; j < group.childCount; j++)
            {
                group.GetChild(j).GetComponent<FireController>().removeFire.AddListener(RemoveActiveFire);
            }

        }

        updateFireCounter.Invoke(numberFiresLeft, 0);
        firesExtinguishedInCombo = 0;
        comboScore = 0;
        landingGuide.SetActive(false);
        landingField.SetActive(false);
        goalSphere.SetActive(false);
    }

    private void RemoveActiveFire(Transform fire)
    {
        firesExtinguishedInCombo++;
        AddScore(firesExtinguishedInCombo * Constants.SingleExtinguishScore);
        numberFiresLeft--;
        updateFireCounter.Invoke(numberFiresLeft, firesExtinguishedInCombo);

        Transform fireGroup = fire.parent;
        if(fireGroup.childCount == 1)   // The only fire left in the fire group
        {
            removeGroupFormLocator.Invoke(fireGroup.GetSiblingIndex());
            Destroy(fireGroup.gameObject);
        }
        else
        {
            Destroy(fire.gameObject);
        }

        if (numberFiresLeft == 0)
        {
            missionPassed = true;
            EnableGoalTargets();
        }
    }

    public override void EndMission(bool landed)
    {
        AddScore(landed ? Constants.LandingScore : Constants.ClearSphereScore);
        removeGoalIcons.Invoke();
        Destroy(goalSphere);
        initStageClear.Invoke(Constants.StageClearSign);
    }

    private void EnableGoalTargets()
    {
        landingGuide.SetActive(true);
        landingField.SetActive(true);
        goalSphere.SetActive(true);
        showGoalIcons.Invoke();
    }

    private void AddScore(int amount)
    {
        if (firesExtinguishedInCombo < 2)
        {
            StartCoroutine(StartComboTimer());
        }
        comboScore += amount;
        updateAddedScoreSign.Invoke(comboScore);
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
