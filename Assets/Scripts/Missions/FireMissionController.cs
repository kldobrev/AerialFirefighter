using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FireMissionController : MissionController
{
    [SerializeField]
    private UnityEvent<int> _updateFireCounter;
    [SerializeField]
    private UnityEvent<int, int> _showExtinguishedSign;
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
    private UnityEvent<int> _removeAreaFormLocator;
    [SerializeField]
    private UnityEvent _activateMenuCheckpoints;
    [SerializeField]
    private UnityEvent _signalCheckpointCreation;

    private int _numberFireAreasTotal;
    private int _numberFireAreasLeft;
    private int _numberFiresLeft;
    private int _numberFiresCheckpoint;
    private int _firesExtinguishedInCombo;
    private int _comboScore;
    private int _checkpointScore;
    private bool _updatingCheckpoint;
    private bool _comboInProgress;
    private bool _removingFire;
    private IDictionary<Transform, int> _firesInAreasCounts;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _checkpointScore = 0;
        _numberFireAreasTotal = transform.childCount;
        _numberFireAreasLeft = _numberFireAreasTotal;
        _firesInAreasCounts = new Dictionary<Transform, int>();
        for (int i = 0; i < _numberFireAreasTotal; i++)
        {
            Transform area = transform.GetChild(i);
            _firesInAreasCounts.Add(area, area.childCount);
            _numberFiresLeft += _firesInAreasCounts[area];
            for (int j = 0; j < _firesInAreasCounts[area]; j++)
            {
                UnityEvent<Transform> removeFire = new();
                removeFire.AddListener(RemoveActiveFire);
                Transform fire = area.GetChild(j);
                fire.GetComponent<FireController>().RemoveFire = removeFire;
            }
            _numberFiresCheckpoint = _numberFiresLeft;
        }

        _updateFireCounter.Invoke(_numberFiresLeft);
        _firesExtinguishedInCombo = 0;
        _comboScore = 0;
        _updatingCheckpoint = false;
        _comboInProgress = false;
        _removingFire = false;
        landingGuide.SetActive(false);
        landingField.SetActive(false);
        goalSphere.SetActive(false);
    }

    private void RemoveActiveFire(Transform fire)
    {
        Transform fireArea = fire.parent;
        _firesInAreasCounts[fireArea]--;
        _firesExtinguishedInCombo++;
        AddScore(_firesExtinguishedInCombo * Constants.SingleExtinguishScore);
        _numberFiresLeft--;
        _updateFireCounter.Invoke(_numberFiresLeft);
        _showExtinguishedSign.Invoke(_numberFiresLeft, _firesExtinguishedInCombo);

        if (_firesInAreasCounts[fireArea] == 0)   // The only fire left in the fire area, change this to include inactive fires scenario
        {
            _removeAreaFormLocator.Invoke(fireArea.GetSiblingIndex());
            _firesInAreasCounts.Remove(fireArea);
            Destroy(fireArea.gameObject);
            _numberFireAreasLeft--;
            // Update fire data
            _checkpointScore = score;
            _numberFiresCheckpoint = _numberFiresLeft;
            _signalCheckpointCreation.Invoke();
            StartCoroutine(UpdateFiresCheckpointData());
            if (_numberFireAreasLeft == _numberFireAreasTotal - 1)    // Fire area is removed for the first time
            {
                _activateMenuCheckpoints.Invoke();
            }
        }

        if (_numberFireAreasLeft == 0)
        {
            if (_numberFiresLeft != 0)  // A quick bug fix
            {
                _numberFiresLeft = 0;
                _updateFireCounter.Invoke(_numberFiresLeft);
            }
            missionPassed = true;
            EnableGoalTargets();
        }
    }

    public void RestoreFiresFromCheckpoint()
    {
        for (int i = 0; i < _numberFireAreasLeft; i++)	// Change to a for each loop
        {
            Transform area = transform.GetChild(i);
            for (int j = 0; j < area.childCount; j++)
            {
                Transform fire = area.GetChild(j);
                fire.GetComponent<FireController>().RestoreFire();
                _numberFiresLeft++;
            }
        }
        _numberFiresLeft = _numberFiresCheckpoint;
        _updateFireCounter.Invoke(_numberFiresLeft);
        score = _checkpointScore;
        _updateScoreSign.Invoke(score);
        _firesExtinguishedInCombo = 0;
        _comboInProgress = false;
        _updatingCheckpoint = false;
    }

    public bool IsReadyForCheckpointReload()
    {
        return !_updatingCheckpoint && !_comboInProgress && !_removingFire;
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

    private IEnumerator UpdateFiresCheckpointData()
    {
        _updatingCheckpoint = true;
        yield return null;  // Waiting a frame for hierarchy changes to take effect
        for (int i = 0; i < _numberFireAreasLeft; i++)
        {
            Transform area = transform.GetChild(i);
            for (int j = 0; j < area.childCount; j++)
            {
                Transform fire = area.GetChild(j);
                fire.GetComponent<FireController>().UpdateCheckpointData();
                yield return null;
            }
        }
        _updatingCheckpoint = false;
    }

    private IEnumerator StartComboTimer()
    {
        _comboInProgress = true;
        yield return new WaitForSeconds(Constants.ExtinguishedComboTime);
        _firesExtinguishedInCombo = 0;
        score += _comboScore;
        _updateScoreSign.Invoke(score);
        _comboScore = 0;
        _hideExtinguishedSign.Invoke();
        _hideAddToScoreSign.Invoke();
        _comboInProgress = false;
    }

}
