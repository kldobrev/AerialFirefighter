using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FireMissionController : MissionController
{
    [SerializeField]
    private UnityEvent<int, int> _updateFireCounter;
    [SerializeField]
    private UnityEvent _hideExtinguishedSign;
    [SerializeField]
    private UnityEvent<int> _updateScoreSign;
    [SerializeField]
    private UnityEvent<int> _updateAddedScoreSign;
    [SerializeField]
    private UnityEvent _hideAddToScoreSign;
    [SerializeField]
    private UnityEvent _showGoalIcons;
    [SerializeField]
    private UnityEvent _removeGoalIcons;
    [SerializeField]
    private UnityEvent<string> _initStageClear;
    [SerializeField]
    private UnityEvent<int> _removeGroupFormLocator;

    private int _numberFiresLeft;
    private int _firesExtinguishedInCombo;
    private int _comboScore;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform group = transform.GetChild(i);
            _numberFiresLeft += group.childCount;
            for (int j = 0; j < group.childCount; j++)
            {
                UnityEvent<Transform> removeFire = new();
                removeFire.AddListener(RemoveActiveFire);
                group.GetChild(j).GetComponent<FireController>().RemoveFire = removeFire;
            }

        }

        _updateFireCounter.Invoke(_numberFiresLeft, 0);
        _firesExtinguishedInCombo = 0;
        _comboScore = 0;
        landingGuide.SetActive(false);
        landingField.SetActive(false);
        goalSphere.SetActive(false);
    }

    private void RemoveActiveFire(Transform fire)
    {
        _firesExtinguishedInCombo++;
        AddScore(_firesExtinguishedInCombo * Constants.SingleExtinguishScore);
        _numberFiresLeft--;
        _updateFireCounter.Invoke(_numberFiresLeft, _firesExtinguishedInCombo);

        Transform fireGroup = fire.parent;
        if(fireGroup.childCount == 1)   // The only fire left in the fire group
        {
            _removeGroupFormLocator.Invoke(fireGroup.GetSiblingIndex());
            Destroy(fireGroup.gameObject);
        }
        else
        {
            Destroy(fire.gameObject);
        }

        if (_numberFiresLeft == 0)
        {
            missionPassed = true;
            EnableGoalTargets();
        }
    }

    public override void EndMission(bool landed)
    {
        AddScore(landed ? Constants.LandingScore : Constants.ClearSphereScore);
        _removeGoalIcons.Invoke();
        Destroy(goalSphere);
        _initStageClear.Invoke(Constants.StageClearSign);
    }

    private void EnableGoalTargets()
    {
        landingGuide.SetActive(true);
        landingField.SetActive(true);
        goalSphere.SetActive(true);
        _showGoalIcons.Invoke();
    }

    private void AddScore(int amount)
    {
        if (_firesExtinguishedInCombo < 2)
        {
            StartCoroutine(StartComboTimer());
        }
        _comboScore += amount;
        _updateAddedScoreSign.Invoke(_comboScore);
    }

    private IEnumerator StartComboTimer()
    {
        yield return new WaitForSeconds(Constants.ExtinguishedComboTime);
        _firesExtinguishedInCombo = 0;
        score += _comboScore;
        _updateScoreSign.Invoke(score);
        _comboScore = 0;
        _hideExtinguishedSign.Invoke();
        _hideAddToScoreSign.Invoke();
    }

}
