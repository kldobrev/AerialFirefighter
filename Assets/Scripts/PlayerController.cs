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
using static UnityEditor.VersionControl.Message;

public class PlayerController : MonoBehaviour, PlayerControls.IGameplayActions
{
    private PlayerControls controls;
    [SerializeField]
    private Transform bodyTransform;
    [SerializeField]
    private Rigidbody planeBody;
    [SerializeField]
    private Transform propeller;
    [SerializeField]
    private float throttleAcceleration;
    [SerializeField]
    private float pitchFactor;
    [SerializeField]
    private float yawFactor;
    [SerializeField]
    private float rollFactor;
    [SerializeField]
    private float liftForce = 3.1f;
    [SerializeField]
    private float pitchLiftFactor = 2;
    [SerializeField]
    private ParticleSystem dropWaterEffect;
    public static AmmunitionUITracker UIAmmoTracker;


    [SerializeField]
    private UnityEvent<int> setSpeedometerSpeed;
    [SerializeField]
    private UnityEvent<bool, int> updateAutoSpeedIndicator;
    [SerializeField]
    private UnityEvent<float> updateHeightMeter;
    [SerializeField]
    private UnityEvent<Vector2, float> updateRadarCamera;
    [SerializeField]
    private UnityEvent toggleUITracker;
    [SerializeField]
    private UnityEvent<bool, float, float> sendWeaponDataToTracker;
    [SerializeField]
    private UnityEvent<float> setFuelGaugeCap;
    [SerializeField]
    private UnityEvent<float> updateFuelGaugeQtity;
    [SerializeField]
    private UnityEvent startFadeOut;
    [SerializeField]
    private UnityEvent startFadeIn;
    [SerializeField]
    private UnityEvent<float> setWaterGaugeCap;
    [SerializeField]
    private UnityEvent<float> updateWaterGaugeQtity;


    private float accelerateValue;
    private float throttleInput;
    private float brakeInput;
    private Vector2 pitchRollInput;
    private float yawInput;
    private float autoSpeed;
    private bool isAirbourne;
    private bool isAutoSpeedOn;
    private float airbourneThresholdY;
    private float planeDrag;
    private float planeAngularDrag;
    private int planeMagnitudeRounded;
    private FrameRule sendHeightRule;
    private FrameRule sendCoordsRule;
    private FrameRule sendSpeedRule;
    private FrameRule spinPropellerRule;
    public static Transform PlayerBodyTransform;
    private float signedEulerPitch;
    private float planeSpeed;
    private static bool engineStarted;
    private float propellerSpeed;
    private float fuelQuantity;
    private bool outsideFieldBounds;
    private bool lockedControls;
    private bool nullifyingAngleEnabled;
    private bool waterTankOpened;
    private float waterQuantity;
    private float bankAngle;


    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.startstopengine.canceled += OnStartstopengine;
        controls.gameplay.pitchroll.performed += OnPitchroll;
        controls.gameplay.pitchroll.canceled += OnCancelPitchroll;
        controls.gameplay.yaw.performed += OnYaw;
        controls.gameplay.yaw.canceled += OnCancelYaw;
        controls.gameplay.accelerate.performed += OnAccelerate;
        controls.gameplay.accelerate.canceled += context => throttleInput = 0f;
        controls.gameplay.brake.performed += OnBrake;
        controls.gameplay.brake.canceled += OnCancelBrake;
        controls.gameplay.dropwater.canceled += OnDropwater;
        controls.gameplay.toggleautospeed.performed += OnToggleautospeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        isAirbourne = false;
        isAutoSpeedOn = false;
        airbourneThresholdY = transform.position.y + 1;
        autoSpeed = 0;
        planeMagnitudeRounded = 0;
        planeDrag = Constants.PlDefaultDrag;
        planeAngularDrag = Constants.PlDefaultAngularDrag;
        sendHeightRule = new FrameRule(Constants.SendHeightFramerule);
        sendCoordsRule = new FrameRule(Constants.SendCoordsFramerule);
        sendSpeedRule = new FrameRule(Constants.SendSpeedFramerule);
        spinPropellerRule = new FrameRule(Constants.SpinPropellerFramerule);
        UIAmmoTracker = transform.GetComponent<AmmunitionUITracker>();
        PlayerBodyTransform = bodyTransform;
        propellerSpeed = 0;
        engineStarted = false;
        setFuelGaugeCap.Invoke(Constants.FuelCapacity);
        setWaterGaugeCap.Invoke(Constants.WaterCapacity);
        fuelQuantity = 10000;    // Will be set from game settings
        waterQuantity = 1000;
        planeBody.mass = Mathf.Clamp(Constants.WeightPlaneNoLoad + (waterQuantity * Constants.WaterQuantityToWeightRatio),
            Constants.WeightPlaneNoLoad, Constants.MaxWeightPlaneFullyLoaded);   // Should be executed only when attempting firefighter missions
        outsideFieldBounds = false;
        lockedControls = false;
        nullifyingAngleEnabled = false;
        waterTankOpened = false;
        updateFuelGaugeQtity.Invoke(fuelQuantity);

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
        if (!isAirbourne && transform.position.y > airbourneThresholdY)
        {
            Debug.Log("Lift off: " + planeMagnitudeRounded);
            isAirbourne = true;
        }

        // Updating UI elements
        if (sendHeightRule.CheckFrameRule()) updateHeightMeter.Invoke(transform.position.y);
        sendHeightRule.AdvanceCounter();

        if (sendCoordsRule.CheckFrameRule()) updateRadarCamera.Invoke(new Vector2(transform.position.x, 
            transform.position.z), transform.rotation.eulerAngles.y);
        sendCoordsRule.AdvanceCounter();

        if (sendSpeedRule.CheckFrameRule())
        {
            planeMagnitudeRounded = Mathf.RoundToInt(planeSpeed);
            setSpeedometerSpeed.Invoke(planeMagnitudeRounded);
        }
        sendSpeedRule.AdvanceCounter();

        // Fuel management
        if (engineStarted)
        {
            fuelQuantity -= (Constants.EngineRunningFuelWaste + (0.001f * planeSpeed));
            if (fuelQuantity <= 0) engineStarted = false;
            updateFuelGaugeQtity.Invoke(fuelQuantity);
        }

        // Water management
        if (waterTankOpened)
        {
            bankAngle = HelperMethods.GetSignedAngleFromEuler(planeBody.rotation.eulerAngles.z);
            if (waterQuantity <= 0 || bankAngle < -Constants.PourWaterBankAngleMinMax || 
                bankAngle > Constants.PourWaterBankAngleMinMax)
            {
                waterTankOpened = false;
                dropWaterEffect.Stop();
            }
            waterQuantity -= Constants.WaterWasteRate;
            planeBody.mass -= (Constants.WaterWasteRate * Constants.WaterQuantityToWeightRatio);
            updateWaterGaugeQtity.Invoke(waterQuantity);
        }

        // Propeller spinning
        if (engineStarted || propellerSpeed > 0)
        {
            propeller.Rotate(propellerSpeed, 0, 0);
            if (propellerSpeed < Constants.MaxIdlePropellerSpeed || !engineStarted)
            {
                if (spinPropellerRule.CheckFrameRule()) propellerSpeed += (engineStarted ? 2 : -2);
                spinPropellerRule.AdvanceCounter();
            }
            else if (propellerSpeed >= Constants.MaxIdlePropellerSpeed)
            {
                propellerSpeed = Constants.MaxIdlePropellerSpeed + (0.1f * planeSpeed);
            }
        }

        if (outsideFieldBounds)
        {
            if (UIController.GetScreenTransparency() == 0)
            {
                lockedControls = true;
                startFadeOut.Invoke();
            }
            else if (UIController.GetScreenTransparency() == 255 && lockedControls)
            {
                // Reversing plane direction and speed after passing stage boundary
                transform.Rotate(new Vector3(2 * transform.rotation.eulerAngles.x, 180, 0));
                Vector3 currentDirection = -planeBody.velocity.normalized;
                planeBody.velocity = currentDirection * planeBody.velocity.magnitude;
                Physics.SyncTransforms();
                lockedControls = false;
            }
        }

    }

    void FixedUpdate()
    {
        accelerateValue = 0;
        planeSpeed = planeBody.velocity.magnitude;
        planeDrag = Constants.PlDefaultDrag;

        if (propellerSpeed >= Constants.MaxIdlePropellerSpeed)
        {
            if (throttleInput != 0)  // Accelerate using player input ignoring auto speed value
            {
                accelerateValue = throttleInput * throttleAcceleration;
            }
            else
            {
                if (isAutoSpeedOn) // Maintain constant speed if enabled
                    accelerateValue = planeSpeed < autoSpeed ? throttleAcceleration : 0;
            }

            if (planeSpeed > Constants.PlaneMaxSpeed) planeDrag += (Constants.HighSpeedDrag * planeSpeed);
            signedEulerPitch = HelperMethods.GetSignedAngleFromEuler(planeBody.rotation.eulerAngles.x);
        }
        planeBody.AddRelativeForce(Vector3.up * ((planeSpeed - (planeSpeed * signedEulerPitch * pitchLiftFactor))
            * liftForce), ForceMode.Impulse);

        if (brakeInput != 0)    // Brake engaged
        {
            planeDrag += Constants.PlBrakeDrag;
        }
        else if (signedEulerPitch >= -Constants.PitchDragAngle && planeBody.position.y < Constants.MaxHeightAllowed)
        {
            planeBody.AddRelativeForce(Vector3.forward * accelerateValue, ForceMode.Acceleration);
        }
        else
        {
            planeDrag += (Constants.HighPitchDrag * (-signedEulerPitch));
        }

        if (planeBody.position.y >= Constants.MaxHeightAllowed)    // Height ceiling check
            planeDrag += Constants.HeightDrag;

        if (pitchRollInput != Vector2.zero && planeSpeed > 1)
        {
            if (pitchRollInput.y != 0 && isAirbourne) planeDrag += (Constants.PlTurnDrag * planeSpeed);
            planeBody.AddRelativeTorque(pitchRollInput.y * pitchFactor * Vector3.right, ForceMode.Acceleration);
            planeBody.AddRelativeTorque(pitchRollInput.x * rollFactor * Vector3.forward, ForceMode.Acceleration);
        }

        if (isAirbourne && yawInput != 0f)
        {
            if (throttleInput == 0) planeDrag += Constants.PlTurnDrag; 
            planeBody.AddRelativeTorque(yawInput * yawFactor * Vector3.up, ForceMode.Acceleration);
        }

        if (planeBody.drag != planeDrag) planeBody.drag = planeDrag;
        if(planeBody.angularDrag != planeAngularDrag) planeBody.angularDrag = planeAngularDrag;
    }

    private void ToggleAutoSpeed()
    {
        isAutoSpeedOn = !isAutoSpeedOn;
        if(isAutoSpeedOn) autoSpeed = (float)planeMagnitudeRounded;
        updateAutoSpeedIndicator.Invoke(isAutoSpeedOn, planeMagnitudeRounded);
    }

    private void OnEnable()
    {
        controls.gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.gameplay.Disable();
    }

    public static bool EngineStarted()
    {
        return engineStarted;
    }

    // Controls section

    public void OnStartstopengine(InputAction.CallbackContext context)
    {
        if (!lockedControls && fuelQuantity > 0)
        {
            engineStarted = !engineStarted;
            if (!engineStarted && isAutoSpeedOn) ToggleAutoSpeed();
        }
    }

    public void OnPitchroll(InputAction.CallbackContext context)
    {
        if (!lockedControls)
        {
            pitchRollInput = context.ReadValue<Vector2>();
            planeAngularDrag = Constants.PlTurnAngularDrag;
        }
    }

    private void OnCancelPitchroll(InputAction.CallbackContext context)
    {
        pitchRollInput = Vector2.zero;
        if (yawInput == 0 && !nullifyingAngleEnabled)
        {
            planeAngularDrag = Constants.PlDefaultAngularDrag;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void OnYaw(InputAction.CallbackContext context)
    {
        if (!lockedControls)
        {
            yawInput = context.ReadValue<float>();
            planeAngularDrag = Constants.PlTurnAngularDrag;
        }
    }

    private void OnCancelYaw(InputAction.CallbackContext context)
    {
        yawInput = 0f;
        if (pitchRollInput == Vector2.zero && !nullifyingAngleEnabled)
        {
            planeAngularDrag = Constants.PlDefaultAngularDrag;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (!lockedControls) throttleInput = context.ReadValue<float>();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (!lockedControls) brakeInput = context.ReadValue<float>();
    }

    public void OnCancelBrake(InputAction.CallbackContext context)
    {
        brakeInput = 0;
    }

    public void OnDropwater(InputAction.CallbackContext context)
    {
        if (isAirbourne && waterQuantity > 0)
        {
            waterTankOpened = !waterTankOpened;
            if (waterTankOpened)
            {
                dropWaterEffect.Play();
            }
            else
            {
                dropWaterEffect.Stop();
            }
        }

    }

    public void OnStopFiringWeapon(InputAction.CallbackContext context)
    {
        // In progress
    }

    public void OnToggleautospeed(InputAction.CallbackContext context)
    {
        if (!lockedControls && engineStarted) ToggleAutoSpeed();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constants.TerrainTagName) && isAirbourne)
        {
            isAirbourne = false;
            float signedEulerBank = HelperMethods.GetSignedAngleFromEuler(transform.rotation.eulerAngles.z);
            Debug.Log("Pitch: " + signedEulerPitch + ", Bank: " + signedEulerBank);
            if (-5 <= signedEulerPitch && signedEulerPitch <= 3 && -10 < signedEulerBank && signedEulerBank < 10)
            {
                Debug.Log("Landing angle rule met.");
            }
            else
            {
                startFadeOut.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("StageBounds") && outsideFieldBounds)
        {
            outsideFieldBounds = false;
            startFadeIn.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("StageBounds") && !outsideFieldBounds)
            outsideFieldBounds = true;
    }

    private IEnumerator NullifyAngularSpeed()
    {
        nullifyingAngleEnabled = true;
        yield return new WaitForSeconds(0.8f);
        if(pitchRollInput == Vector2.zero && yawInput == 0) planeBody.angularVelocity = Vector3.zero;
        nullifyingAngleEnabled = false;
    }

}
