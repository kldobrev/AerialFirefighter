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
    [SerializeField]
    private ParticleSystem waterSplashEffect;
    [SerializeField]
    private ParticleSystem crashEffect;
    [SerializeField]
    private ParticleSystem crashInWaterEffect;



    [SerializeField]
    private UnityEvent<bool> setSpeedometerActive;
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
    [SerializeField]
    private UnityEvent<Vector3> detachCamera;
    [SerializeField]
    private UnityEvent<Vector3> changeCameraDistance;
    [SerializeField]
    private UnityEvent<bool> stageEndTrigger;



    public UnityEvent<GameOverType> signalGameOver;
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
    private float liftValue;
    private bool throttleAllowed;
    private float cameraTransitionTimer;
    private Transform cachedTrns;
    private float landingTimerCounter;
    private IEnumerator landingCountCoroutine;


    private void Awake()
    {
        cachedTrns = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        isAirbourne = false;
        isAutoSpeedOn = false;
        airbourneThresholdY = cachedTrns.position.y + 1;
        autoSpeed = 0;
        planeMagnitudeRounded = 0;
        planeDrag = Constants.PlDefaultDrag;
        planeAngularDrag = Constants.PlDefaultAngularDrag;
        sendHeightRule = new FrameRule(Constants.SendHeightFramerule);
        sendCoordsRule = new FrameRule(Constants.SendCoordsFramerule);
        sendSpeedRule = new FrameRule(Constants.SendSpeedFramerule);
        spinPropellerRule = new FrameRule(Constants.SpinPropellerFramerule);
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
        throttleAllowed = false;
        cameraTransitionTimer = 0;
        landingTimerCounter = 0;
        landingCountCoroutine = LandingCountdown();
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
        if (!isAirbourne && cachedTrns.position.y > airbourneThresholdY)
        {
            isAirbourne = true;
        }

        // Updating UI elements
        if (sendHeightRule.CheckFrameRule()) updateHeightMeter.Invoke(cachedTrns.position.y);
        sendHeightRule.AdvanceCounter();

        if (sendCoordsRule.CheckFrameRule()) updateRadarCamera.Invoke(new Vector2(cachedTrns.position.x, 
            cachedTrns.position.z), cachedTrns.rotation.eulerAngles.y);
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
            if (fuelQuantity <= 0)
            {
                engineStarted = false;
                if (!isAirbourne)
                {
                    signalGameOver.Invoke(GameOverType.FuelDepleted);
                }
            }
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
            waterQuantity = Mathf.Clamp(waterQuantity - Constants.WaterWasteRate, 0, Constants.WaterCapacity);
            planeBody.mass = Mathf.Clamp(planeBody.mass - (Constants.WaterWasteRate * Constants.WaterQuantityToWeightRatio),
                Constants.WeightPlaneNoLoad, Constants.MaxWeightPlaneFullyLoaded);
            updateWaterGaugeQtity.Invoke(waterQuantity);
        }

        // Propeller spinning
        if (engineStarted || propellerSpeed > 0)
        {
            propeller.Rotate(propellerSpeed, 0, 0);
            if (propellerSpeed < Constants.MaxIdlePropellerSpeed || !engineStarted)
            {
                if (spinPropellerRule.CheckFrameRule()) propellerSpeed += (engineStarted ? 2 : -2);
                AllowThrottle(false);
                spinPropellerRule.AdvanceCounter();
            }
            else if (propellerSpeed >= Constants.MaxIdlePropellerSpeed)
            {
                propellerSpeed = Constants.MaxIdlePropellerSpeed + (0.1f * planeSpeed);
                AllowThrottle(true);
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
                cachedTrns.Rotate(new Vector3(2 * cachedTrns.rotation.eulerAngles.x, 180, 0));
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

        if (throttleAllowed)
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

        liftValue = (planeSpeed - (planeSpeed * signedEulerPitch * pitchLiftFactor)) * liftForce;
        planeBody.AddRelativeForce(Vector3.up * liftValue, ForceMode.Impulse);

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

    private void AllowThrottle(bool allowVal)
    {
        if (throttleAllowed != allowVal) throttleAllowed = allowVal;
        setSpeedometerActive.Invoke(allowVal);
    }

    private void EndPlaySession(ParticleSystem effect, GameOverType gameOverReason)
    {
        effect.transform.position = planeBody.position;
        effect.Play();
        detachCamera.Invoke(effect.transform.position);
        Destroy(gameObject);
        signalGameOver.Invoke(gameOverReason);
    }

    // Collisions/Triggers

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constants.TerrainTagName) && isAirbourne)
        {
            isAirbourne = false;
            float signedEulerBank = HelperMethods.GetSignedAngleFromEuler(cachedTrns.rotation.eulerAngles.z);
            ContactPoint collisionPoint = collision.GetContact(0);
            if (!(collisionPoint.normal == Vector3.up && collisionPoint.impulse.y == 0 &&
                Constants.LandingPitchMin <= signedEulerPitch && signedEulerPitch <= Constants.LandingPitchMax &&
                Constants.LandingBankMin < signedEulerBank && signedEulerBank < Constants.LandingBankMax))
            {
                EndPlaySession(crashEffect, GameOverType.GroundCrash);
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
        else if (other.gameObject.CompareTag(Constants.WaterSurfaceTagName))
        {
            if (waterQuantity < Constants.WaterCapacity)
            {
                waterQuantity = Mathf.Clamp(waterQuantity + Constants.WaterScoopRate, 0, Constants.WaterCapacity);
                planeBody.mass = Mathf.Clamp(planeBody.mass + (Constants.WaterScoopRate * Constants.WaterQuantityToWeightRatio),
                    Constants.WeightPlaneNoLoad, Constants.MaxWeightPlaneFullyLoaded);
                updateWaterGaugeQtity.Invoke(waterQuantity);
            }
        }
        else if (other.gameObject.CompareTag(Constants.WaterDepthsTagName))
        {
            cameraTransitionTimer = 0;
            waterSplashEffect.Stop();
            EndPlaySession(crashInWaterEffect, GameOverType.WaterCrash);
        }
        else if (other.gameObject.CompareTag(Constants.GoalSphereTag))
        {
            stageEndTrigger.Invoke(false);
        }
        else if (other.gameObject.CompareTag(Constants.LandingZoneTagName))
        {
            StartCoroutine(landingCountCoroutine);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.WaterSurfaceTagName))
        {
            float signedEulerBank = HelperMethods.GetSignedAngleFromEuler(cachedTrns.rotation.eulerAngles.z);
            if (Constants.ScoopPitchMin <= signedEulerPitch && signedEulerPitch <= Constants.ScoopPitchMax &&
                Constants.ScoopBankMin < signedEulerBank && signedEulerBank < Constants.ScoopBankMax)
            {
                // Trying to keep plane afloat while scooping water
                planeBody.AddRelativeForce(Vector3.up * Constants.WaterFloatForceUp, ForceMode.VelocityChange);
                waterSplashEffect.Play();
                if (cameraTransitionTimer == 0)
                {
                    changeCameraDistance.Invoke(Constants.CameraTrailingDistanceWater);
                    StartCoroutine(CameraTimerCountdown());
                }
                cameraTransitionTimer = Constants.CameraTimeLimit;
            }
        }
        else if(other.gameObject.CompareTag(Constants.LandingZoneTagName) && (engineStarted || planeMagnitudeRounded != 0))
        {
            landingTimerCounter = Constants.LandingTimer;   // Reset timer if player moves
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.StageBoundsTagName) && !outsideFieldBounds)
            outsideFieldBounds = true;
        else if (other.gameObject.CompareTag(Constants.WaterSurfaceTagName))
        {
            planeBody.AddRelativeForce(Vector3.down * Constants.WaterFloatForceDown, ForceMode.VelocityChange);
            waterSplashEffect.Stop();
        }
        else if (other.gameObject.CompareTag(Constants.LandingZoneTagName))
        {
            StopCoroutine(landingCountCoroutine);
        }
    }


    public static bool EngineStarted()
    {
        return engineStarted;
    }

    public void StartPlaneInAir(float initSpeed)
    {
        isAirbourne = true;
        engineStarted = true;
        AllowThrottle(true);
        propellerSpeed = Constants.MaxIdlePropellerSpeed;
        planeBody.velocity = planeBody.transform.forward * initSpeed;
        planeMagnitudeRounded = Mathf.RoundToInt(planeBody.velocity.magnitude);
        ToggleAutoSpeed();
    }

    private IEnumerator NullifyAngularSpeed()
    {
        nullifyingAngleEnabled = true;
        yield return new WaitForSeconds(0.8f);
        if(pitchRollInput == Vector2.zero && yawInput == 0) planeBody.angularVelocity = Vector3.zero;
        nullifyingAngleEnabled = false;
    }

    private IEnumerator CameraTimerCountdown()
    {
        cameraTransitionTimer = Constants.CameraTimeLimit;
        while (cameraTransitionTimer != 0)
        {
            cameraTransitionTimer = Mathf.Clamp(cameraTransitionTimer - Time.deltaTime, 0, Constants.CameraTimeLimit);
            yield return null;
        }
        changeCameraDistance.Invoke(Constants.CameraTrailingDistanceDefault);
    }

    private IEnumerator LandingCountdown() 
    {
        landingTimerCounter = Constants.LandingTimer;
        while (landingTimerCounter != 0)
        {
            landingTimerCounter = Mathf.Clamp(landingTimerCounter - Time.deltaTime, 0, Constants.LandingTimer);
            yield return null;
        }
        stageEndTrigger.Invoke(true);
    }


    // Controls section

    public void ToggleEngine()
    {
        if (!lockedControls && fuelQuantity > 0)
        {
            engineStarted = !engineStarted;
            if (!engineStarted && isAutoSpeedOn) ToggleAutoSpeed();
        }
    }

    public void PitchRoll(Vector2 direction)
    {
        if (!lockedControls)
        {
            pitchRollInput = direction;
            planeAngularDrag = Constants.PlTurnAngularDrag;
        }
    }

    public void CancelPitchroll()
    {
        pitchRollInput = Vector2.zero;
        if (yawInput == 0 && !nullifyingAngleEnabled)
        {
            planeAngularDrag = Constants.PlDefaultAngularDrag;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void Yaw(float input)
    {
        if (!lockedControls)
        {
            yawInput = input;
            planeAngularDrag = Constants.PlTurnAngularDrag;
        }
    }

    public void CancelYaw()
    {
        yawInput = 0f;
        if (pitchRollInput == Vector2.zero && !nullifyingAngleEnabled)
        {
            planeAngularDrag = Constants.PlDefaultAngularDrag;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void Accelerate(float input)
    {
        if (!lockedControls) throttleInput = input;
    }

    public void CancelAccelerate()
    {
        throttleInput = 0;
    }

    public void Brake(float input)
    {
        if (!lockedControls) brakeInput = input;
    }

    public void CancelBrake()
    {
        brakeInput = 0;
    }

    public void ToggleDropwater()
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

    public void ToggleAutoSpeed()
    {
        if (!lockedControls && engineStarted)
        {
            isAutoSpeedOn = !isAutoSpeedOn;
            if (isAutoSpeedOn) autoSpeed = (float)planeMagnitudeRounded;
            updateAutoSpeedIndicator.Invoke(isAutoSpeedOn, planeMagnitudeRounded);
        }
    }

}
