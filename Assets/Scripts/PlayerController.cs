using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using static UnityEditor.VersionControl.Message;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform _bodyTransform;
    [SerializeField]
    private Rigidbody _planeBody;
    [SerializeField]
    private Transform _propeller;
    [SerializeField]
    private float _throttleAcceleration;
    [SerializeField]
    private float _pitchFactor;
    [SerializeField]
    private float _yawFactor;
    [SerializeField]
    private float _rollFactor;
    [SerializeField]
    private float _liftForce = 3.1f;
    [SerializeField]
    private float _pitchLiftFactor = 2;
    [SerializeField]
    private ParticleSystem _dropWaterEffect;
    [SerializeField]
    private ParticleSystem _waterSplashEffect;
    [SerializeField]
    private ParticleSystem _crashEffect;
    [SerializeField]
    private ParticleSystem _crashInWaterEffect;



    [SerializeField]
    private UnityEvent<bool> _setSpeedometerActive;
    [SerializeField]
    private UnityEvent<int> _setSpeedometerSpeed;
    [SerializeField]
    private UnityEvent<bool, int> _updateAutoSpeedIndicator;
    [SerializeField]
    private UnityEvent<float> _updateHeightMeter;
    [SerializeField]
    private UnityEvent<Vector2, float> _updateRadarCamera;
    [SerializeField]
    private UnityEvent<float> _setFuelGaugeCap;
    [SerializeField]
    private UnityEvent<float> _updateFuelGaugeQuantity;
    [SerializeField]
    private UnityEvent _startFadeOut;
    [SerializeField]
    private UnityEvent _startFadeIn;
    [SerializeField]
    private UnityEvent<float> _setWaterGaugeCap;
    [SerializeField]
    private UnityEvent<float> _updateWaterGaugeQuantity;
    [SerializeField]
    private UnityEvent<Vector3> _detachCamera;
    [SerializeField]
    private UnityEvent<Vector3> _changeCameraDistance;
    [SerializeField]
    private UnityEvent<bool> _stageEndTrigger;



    public UnityEvent<GameOverType> SignalGameOver { get; set; }
    private float _accelerateValue;
    private float _throttleInput;
    private float _brakeInput;
    private Vector2 _pitchRollInput;
    private float _yawInput;
    private float _autoSpeed;
    private bool _isAirbourne;
    private bool _isAutoSpeedOn;
    private float _airbourneThresholdY;
    private float _planeDrag;
    private float _planeAngularDrag;
    private int _planeMagnitudeRounded;
    private FrameRule _sendHeightRule;
    private FrameRule _sendCoordsRule;
    private FrameRule _sendSpeedRule;
    private FrameRule _spinPropellerRule;
    public static Transform PlayerBodyTransform { get; set; }
    private float _signedEulerPitch;
    private float _planeSpeed;
    private static bool _engineStarted;
    private float _propellerSpeed;
    private float _fuelQuantity;
    private bool _outsideFieldBounds;
    private bool _lockedControls;
    private bool _nullifyingAngleEnabled;
    private bool _waterTankOpened;
    private float _waterQuantity;
    private float _bankAngle;
    private float _liftValue;
    private bool _throttleAllowed;
    private float _cameraTransitionTimer;
    private Transform _cachedTransform;
    private float _landingTimerCounter;
    private IEnumerator _landingCountCoroutine;


    private void Awake()
    {
        _cachedTransform = transform;
        SignalGameOver = new();
    }

    // Start is called before the first frame update
    void Start()
    {
        _isAirbourne = false;
        _isAutoSpeedOn = false;
        _airbourneThresholdY = _cachedTransform.position.y + 1;
        _autoSpeed = 0;
        _planeMagnitudeRounded = 0;
        _planeDrag = Constants.PlDefaultDrag;
        _planeAngularDrag = Constants.PlDefaultAngularDrag;
        _sendHeightRule = new FrameRule(Constants.SendHeightFramerule);
        _sendCoordsRule = new FrameRule(Constants.SendCoordsFramerule);
        _sendSpeedRule = new FrameRule(Constants.SendSpeedFramerule);
        _spinPropellerRule = new FrameRule(Constants.SpinPropellerFramerule);
        PlayerBodyTransform = _bodyTransform;
        _propellerSpeed = 0;
        _engineStarted = false;
        _setFuelGaugeCap.Invoke(Constants.FuelCapacity);
        _setWaterGaugeCap.Invoke(Constants.WaterCapacity);
        _fuelQuantity = 10000;    // Will be set from game settings
        _waterQuantity = 1000;
        _planeBody.mass = Mathf.Clamp(Constants.WeightPlaneNoLoad + (_waterQuantity * Constants.WaterQuantityToWeightRatio),
            Constants.WeightPlaneNoLoad, Constants.MaxWeightPlaneFullyLoaded);   // Should be executed only when attempting firefighter missions
        _outsideFieldBounds = false;
        _lockedControls = false;
        _nullifyingAngleEnabled = false;
        _waterTankOpened = false;
        _throttleAllowed = false;
        _cameraTransitionTimer = 0;
        _landingTimerCounter = 0;
        _landingCountCoroutine = LandingCountdown();
        _updateFuelGaugeQuantity.Invoke(_fuelQuantity);

        // Move to game manager script the logic below
        Transform ground = GameObject.Find("Ground").transform;
        for (int i = 0; i < ground.childCount; i++)
        {
            ground.GetChild(i).tag = Constants.TerrainTagName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isAirbourne && _cachedTransform.position.y > _airbourneThresholdY)
        {
            _isAirbourne = true;
        }

        // Updating UI elements
        if (_sendHeightRule.CheckFrameRule()) _updateHeightMeter.Invoke(_cachedTransform.position.y);
        _sendHeightRule.AdvanceCounter();

        if (_sendCoordsRule.CheckFrameRule()) _updateRadarCamera.Invoke(new Vector2(_cachedTransform.position.x, 
            _cachedTransform.position.z), _cachedTransform.rotation.eulerAngles.y);
        _sendCoordsRule.AdvanceCounter();

        if (_sendSpeedRule.CheckFrameRule())
        {
            _planeMagnitudeRounded = Mathf.RoundToInt(_planeSpeed);
            _setSpeedometerSpeed.Invoke(_planeMagnitudeRounded);
        }
        _sendSpeedRule.AdvanceCounter();

        // Fuel management
        if (_engineStarted)
        {
            _fuelQuantity -= (Constants.EngineRunningFuelWaste + (0.001f * _planeSpeed));
            if (_fuelQuantity <= 0)
            {
                _engineStarted = false;
                if (!_isAirbourne)
                {
                    SignalGameOver.Invoke(GameOverType.FuelDepleted);
                }
            }
            _updateFuelGaugeQuantity.Invoke(_fuelQuantity);
        }

        // Water management
        if (_waterTankOpened)
        {
            _bankAngle = HelperMethods.GetSignedAngleFromEuler(_planeBody.rotation.eulerAngles.z);
            if (_waterQuantity <= 0 || _bankAngle < -Constants.PourWaterBankAngleMinMax || 
                _bankAngle > Constants.PourWaterBankAngleMinMax)
            {
                _waterTankOpened = false;
                _dropWaterEffect.Stop();
            }
            _waterQuantity = Mathf.Clamp(_waterQuantity - Constants.WaterWasteRate, 0, Constants.WaterCapacity);
            _planeBody.mass = Mathf.Clamp(_planeBody.mass - (Constants.WaterWasteRate * Constants.WaterQuantityToWeightRatio),
                Constants.WeightPlaneNoLoad, Constants.MaxWeightPlaneFullyLoaded);
            _updateWaterGaugeQuantity.Invoke(_waterQuantity);
        }

        // _propeller spinning
        if (_engineStarted || _propellerSpeed > 0)
        {
            _propeller.Rotate(_propellerSpeed, 0, 0);
            if (_propellerSpeed < Constants.MaxIdlePropellerSpeed || !_engineStarted)
            {
                if (_spinPropellerRule.CheckFrameRule()) _propellerSpeed += (_engineStarted ? 2 : -2);
                AllowThrottle(false);
                _spinPropellerRule.AdvanceCounter();
            }
            else if (_propellerSpeed >= Constants.MaxIdlePropellerSpeed)
            {
                _propellerSpeed = Constants.MaxIdlePropellerSpeed + (0.1f * _planeSpeed);
                AllowThrottle(true);
            }
        }

        if (_outsideFieldBounds)
        {
            if (UIController.ScreenAlpha == Constants.FadeScreenAlphaMin)
            {
                _lockedControls = true;
                _startFadeOut.Invoke();
            }
            else if (UIController.ScreenAlpha == Constants.FadeScreenAlphaMax && _lockedControls)
            {
                // Reversing plane direction and speed after passing stage boundary
                _cachedTransform.Rotate(new Vector3(2 * _cachedTransform.rotation.eulerAngles.x, 180, 0));
                Vector3 currentDirection = -_planeBody.velocity.normalized;
                _planeBody.velocity = currentDirection * _planeBody.velocity.magnitude;
                Physics.SyncTransforms();
                _lockedControls = false;
            }
        }

    }

    void FixedUpdate()
    {
        _accelerateValue = 0;
        _planeSpeed = _planeBody.velocity.magnitude;
        _planeDrag = Constants.PlDefaultDrag;

        if (_throttleAllowed)
        {
            if (_throttleInput != 0)  // Accelerate using player input ignoring auto speed value
            {
                _accelerateValue = _throttleInput * _throttleAcceleration;
            }
            else
            {
                if (_isAutoSpeedOn) // Maintain constant speed if enabled
                    _accelerateValue = _planeSpeed < _autoSpeed ? _throttleAcceleration : 0;
            }

            if (_planeSpeed > Constants.PlaneMaxSpeed) _planeDrag += (Constants.HighSpeedDrag * _planeSpeed);
            _signedEulerPitch = HelperMethods.GetSignedAngleFromEuler(_planeBody.rotation.eulerAngles.x);
        }

        _liftValue = (_planeSpeed - (_planeSpeed * _signedEulerPitch * _pitchLiftFactor)) * _liftForce;
        _planeBody.AddRelativeForce(Vector3.up * _liftValue, ForceMode.Impulse);

        if (_brakeInput != 0)    // Brake engaged
        {
            _planeDrag += Constants.PlBrakeDrag;
        }
        else if (_signedEulerPitch >= -Constants.PitchDragAngle && _planeBody.position.y < Constants.MaxHeightAllowed)
        {
            _planeBody.AddRelativeForce(Vector3.forward * _accelerateValue, ForceMode.Acceleration);
        }
        else
        {
            _planeDrag += (Constants.HighPitchDrag * (-_signedEulerPitch));
        }

        if (_planeBody.position.y >= Constants.MaxHeightAllowed)    // Height ceiling check
            _planeDrag += Constants.HeightDrag;

        if (_pitchRollInput != Vector2.zero && _planeSpeed > 1)
        {
            if (_pitchRollInput.y != 0 && _isAirbourne) _planeDrag += (Constants.PlTurnDrag * _planeSpeed);
            _planeBody.AddRelativeTorque(_pitchRollInput.y * _pitchFactor * Vector3.right, ForceMode.Acceleration);
            _planeBody.AddRelativeTorque(_pitchRollInput.x * _rollFactor * Vector3.forward, ForceMode.Acceleration);
        }

        if (_isAirbourne && _yawInput != 0f)
        {
            if (_throttleInput == 0) _planeDrag += Constants.PlTurnDrag; 
            _planeBody.AddRelativeTorque(_yawInput * _yawFactor * Vector3.up, ForceMode.Acceleration);
        }

        if (_planeBody.drag != _planeDrag) _planeBody.drag = _planeDrag;
        if(_planeBody.angularDrag != _planeAngularDrag) _planeBody.angularDrag = _planeAngularDrag;
    }

    private void AllowThrottle(bool allowVal)
    {
        if (_throttleAllowed != allowVal) _throttleAllowed = allowVal;
        _setSpeedometerActive.Invoke(allowVal);
    }

    private void EndPlaySession(ParticleSystem effect, GameOverType gameOverReason)
    {
        effect.transform.position = _planeBody.position;
        effect.Play();
        _detachCamera.Invoke(effect.transform.position);
        Destroy(gameObject);
        SignalGameOver.Invoke(gameOverReason);
    }

    // Collisions/Triggers

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constants.TerrainTagName) && _isAirbourne)
        {
            _isAirbourne = false;
            float signedEulerBank = HelperMethods.GetSignedAngleFromEuler(_cachedTransform.rotation.eulerAngles.z);
            ContactPoint collisionPoint = collision.GetContact(0);
            if (!(collisionPoint.normal == Vector3.up && collisionPoint.impulse.y == 0 &&
                Constants.LandingPitchMin <= _signedEulerPitch && _signedEulerPitch <= Constants.LandingPitchMax &&
                Constants.LandingBankMin < signedEulerBank && signedEulerBank < Constants.LandingBankMax))
            {
                EndPlaySession(_crashEffect, GameOverType.GroundCrash);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("StageBounds") && _outsideFieldBounds)
        {
            _outsideFieldBounds = false;
            _startFadeIn.Invoke();
        }
        else if (other.gameObject.CompareTag(Constants.WaterSurfaceTagName))
        {
            if (_waterQuantity < Constants.WaterCapacity)
            {
                _waterQuantity = Mathf.Clamp(_waterQuantity + Constants.WaterScoopRate, 0, Constants.WaterCapacity);
                _planeBody.mass = Mathf.Clamp(_planeBody.mass + (Constants.WaterScoopRate * Constants.WaterQuantityToWeightRatio),
                    Constants.WeightPlaneNoLoad, Constants.MaxWeightPlaneFullyLoaded);
                _updateWaterGaugeQuantity.Invoke(_waterQuantity);
            }
        }
        else if (other.gameObject.CompareTag(Constants.WaterDepthsTagName))
        {
            _cameraTransitionTimer = 0;
            _waterSplashEffect.Stop();
            EndPlaySession(_crashInWaterEffect, GameOverType.WaterCrash);
        }
        else if (other.gameObject.CompareTag(Constants.GoalSphereTag))
        {
            _stageEndTrigger.Invoke(false);
        }
        else if (other.gameObject.CompareTag(Constants.LandingZoneTagName))
        {
            StartCoroutine(_landingCountCoroutine);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.WaterSurfaceTagName))
        {
            float signedEulerBank = HelperMethods.GetSignedAngleFromEuler(_cachedTransform.rotation.eulerAngles.z);
            if (Constants.ScoopPitchMin <= _signedEulerPitch && _signedEulerPitch <= Constants.ScoopPitchMax &&
                Constants.ScoopBankMin < signedEulerBank && signedEulerBank < Constants.ScoopBankMax)
            {
                // Trying to keep plane afloat while scooping water
                _planeBody.AddRelativeForce(Vector3.up * Constants.WaterFloatForceUp, ForceMode.VelocityChange);
                _waterSplashEffect.Play();
                if (_cameraTransitionTimer == 0)
                {
                    _changeCameraDistance.Invoke(Constants.CameraTrailingDistanceWater);
                    StartCoroutine(CameraTimerCountdown());
                }
                _cameraTransitionTimer = Constants.CameraTimeLimit;
            }
        }
        else if(other.gameObject.CompareTag(Constants.LandingZoneTagName) && (_engineStarted || _planeMagnitudeRounded != 0))
        {
            _landingTimerCounter = Constants.LandingTimer;   // Reset timer if player moves
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.StageBoundsTagName) && !_outsideFieldBounds)
            _outsideFieldBounds = true;
        else if (other.gameObject.CompareTag(Constants.WaterSurfaceTagName))
        {
            _planeBody.AddRelativeForce(Vector3.down * Constants.WaterFloatForceDown, ForceMode.VelocityChange);
            _waterSplashEffect.Stop();
        }
        else if (other.gameObject.CompareTag(Constants.LandingZoneTagName))
        {
            StopCoroutine(_landingCountCoroutine);
        }
    }

    public void StartPlaneInAir(float initSpeed)
    {
        _isAirbourne = true;
        _engineStarted = true;
        AllowThrottle(true);
        _propellerSpeed = Constants.MaxIdlePropellerSpeed;
        _planeBody.velocity = _planeBody.transform.forward * initSpeed;
        _planeMagnitudeRounded = Mathf.RoundToInt(_planeBody.velocity.magnitude);
        ToggleAutoSpeed();
    }

    private IEnumerator NullifyAngularSpeed()
    {
        _nullifyingAngleEnabled = true;
        yield return new WaitForSeconds(0.8f);
        if(_pitchRollInput == Vector2.zero && _yawInput == 0) _planeBody.angularVelocity = Vector3.zero;
        _nullifyingAngleEnabled = false;
    }

    private IEnumerator CameraTimerCountdown()
    {
        _cameraTransitionTimer = Constants.CameraTimeLimit;
        while (_cameraTransitionTimer != 0)
        {
            _cameraTransitionTimer = Mathf.Clamp(_cameraTransitionTimer - Time.deltaTime, 0, Constants.CameraTimeLimit);
            yield return null;
        }
        _changeCameraDistance.Invoke(Constants.CameraTrailingDistanceDefault);
    }

    private IEnumerator LandingCountdown() 
    {
        _landingTimerCounter = Constants.LandingTimer;
        while (_landingTimerCounter != 0)
        {
            _landingTimerCounter = Mathf.Clamp(_landingTimerCounter - Time.deltaTime, 0, Constants.LandingTimer);
            yield return null;
        }
        _stageEndTrigger.Invoke(true);
    }


    // Controls section

    public void ToggleEngine()
    {
        if (!_lockedControls && _fuelQuantity > 0)
        {
            _engineStarted = !_engineStarted;
            if (!_engineStarted && _isAutoSpeedOn) ToggleAutoSpeed();
        }
    }

    public void PitchRoll(Vector2 direction)
    {
        if (!_lockedControls)
        {
            _pitchRollInput = direction;
            _planeAngularDrag = Constants.PlTurnAngularDrag;
        }
    }

    public void CancelPitchroll()
    {
        _pitchRollInput = Vector2.zero;
        if (_yawInput == 0 && !_nullifyingAngleEnabled)
        {
            _planeAngularDrag = Constants.PlDefaultAngularDrag;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void Yaw(float input)
    {
        if (!_lockedControls)
        {
            _yawInput = input;
            _planeAngularDrag = Constants.PlTurnAngularDrag;
        }
    }

    public void CancelYaw()
    {
        _yawInput = 0f;
        if (_pitchRollInput == Vector2.zero && !_nullifyingAngleEnabled)
        {
            _planeAngularDrag = Constants.PlDefaultAngularDrag;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void Accelerate(float input)
    {
        if (!_lockedControls) _throttleInput = input;
    }

    public void CancelAccelerate()
    {
        _throttleInput = 0;
    }

    public void Brake(float input)
    {
        if (!_lockedControls) _brakeInput = input;
    }

    public void CancelBrake()
    {
        _brakeInput = 0;
    }

    public void ToggleDropwater()
    {
        if (_isAirbourne && _waterQuantity > 0)
        {
            _waterTankOpened = !_waterTankOpened;
            if (_waterTankOpened)
            {
                _dropWaterEffect.Play();
            }
            else
            {
                _dropWaterEffect.Stop();
            }
        }

    }

    public void ToggleAutoSpeed()
    {
        if (!_lockedControls && _engineStarted)
        {
            _isAutoSpeedOn = !_isAutoSpeedOn;
            if (_isAutoSpeedOn) _autoSpeed = (float)_planeMagnitudeRounded;
            _updateAutoSpeedIndicator.Invoke(_isAutoSpeedOn, _planeMagnitudeRounded);
        }
    }

}
