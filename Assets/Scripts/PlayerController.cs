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
    private Rigidbody rafaleBody;
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
    //[SerializeField]
    //private Transform objective;
    [SerializeField]
    private float pitchLiftFactor = 2;
    //[SerializeField]
    //private WeaponContainer[] weapons;
    public static AmmunitionUITracker UIAmmoTracker;


    [SerializeField]
    private UnityEvent<int> setSpeedometerSpeed;
    [SerializeField]
    private UnityEvent<bool, int> updateAutoSpeedIndicator;
    [SerializeField]
    private UnityEvent<float> updateHeightMeter;
    [SerializeField]
    private UnityEvent<Vector2, float> updateRadarCamera;
    //[SerializeField]
    //private UnityEvent<Transform> updateLocator;
    [SerializeField]
    private UnityEvent toggleUITracker;
    [SerializeField]
    private UnityEvent<bool, float, float> sendWeaponDataToTracker;


    public float yawDrag = 1;
    public float pitchDrag = 1;
    public float rollDrag = 1;



    private float accelerateValue;
    private float throttleInput;
    private float brakeInput;
    private Vector2 pitchRollInput;
    private float yawInput;
    private Animator playerAnimator;
    private float autoSpeed;
    private bool isAirbourne;
    private bool isAutoSpeedOn;
    private float airbourneThresholdY;
    private float planeDrag;
    private float planeAngularDrag;
    private Transform rigBodyTransform;
    private int planeMagnitudeRounded;
    private FrameRule sendHeightRule;
    private FrameRule sendCoordsRule;
    private FrameRule sendSpeedRule;
    private FrameRule spinPropellerRule;
    private int selectedWeaponIdx;
    public static Transform PlayerBodyTransform;
    private float signedEulerPitch;
    private float planeSpeed;
    private static bool engineStarted;
    private bool landingComplete;
    private float propellerSpeed;


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
        controls.gameplay.fireweapon.performed += OnFireweapon;
        controls.gameplay.fireweapon.canceled += OnStopFiringWeapon;
        controls.gameplay.toggleautospeed.performed += OnToggleautospeed;
        controls.gameplay.tracktarget.performed += OnTracktarget;
        controls.gameplay.toggletracker.performed += OnToggletracker;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = transform.GetComponent<Animator>();
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
        selectedWeaponIdx = 0;
        UIAmmoTracker = transform.GetComponent<AmmunitionUITracker>();
        rigBodyTransform = rafaleBody.transform;
        //weapons[0].SetWeapon(Constants.HeatseekerMissile);   // Will be set by player
        //weapons[1].SetWeapon(Constants.BulletCannon);   // Will be set by player
        //UIAmmoTracker.UpdateWeaponAmmoInUI(weapons[selectedWeaponIdx].Ammunition);
        //sendWeaponDataToTracker.Invoke(true, weapons[selectedWeaponIdx].Range, weapons[selectedWeaponIdx].LockingStep);
        PlayerBodyTransform = bodyTransform;
        propellerSpeed = 0;
        landingComplete = false;
        engineStarted = false;
        //updateLocator.Invoke(objective);

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
            Debug.Log(planeMagnitudeRounded);
            isAirbourne = true;
        }

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

        if (engineStarted || propellerSpeed > 0)
        {
            propeller.Rotate(propellerSpeed, 0, 0);
            if (propellerSpeed < Constants.MaxPropellerSpeed || !engineStarted)
            {
                if (spinPropellerRule.CheckFrameRule()) propellerSpeed += (engineStarted ? 1 : -1);
                spinPropellerRule.AdvanceCounter();
            }
        }
    }

    void FixedUpdate()
    {
        accelerateValue = 0;
        planeSpeed = rafaleBody.velocity.magnitude;
        planeDrag = Constants.PlDefaultDrag;

        if (propellerSpeed >= Constants.MaxPropellerSpeed)
        {
            if (throttleInput != 0)  // Accelerate using player input ignoring auto speed value
            {
                accelerateValue = throttleInput * throttleAcceleration;
            }
            else
            {
                if (isAirbourne && isAutoSpeedOn) // Maintain constant speed if enabled
                {
                    accelerateValue = planeSpeed < autoSpeed ? throttleAcceleration : 0;
                }
            }

            if (planeSpeed > 180) planeDrag += (Constants.HighSpeedDrag * planeSpeed);
            signedEulerPitch = HelperMethods.GetSignedAngleFromEuler(rafaleBody.rotation.eulerAngles.x);
        }
        rafaleBody.AddRelativeForce(Vector3.up * ((planeSpeed - (planeSpeed * signedEulerPitch * pitchLiftFactor))
            * liftForce), ForceMode.Impulse);

        if (brakeInput != 0)    // Brake engaged
        {
            planeDrag += Constants.PlBrakeDrag;
        }
        else if (signedEulerPitch >= -Constants.PitchDragAngle)
        {
            rafaleBody.AddRelativeForce(Vector3.forward * accelerateValue, ForceMode.Acceleration);
        }
        else
        {
            planeDrag += (Constants.HighPitchDrag * (-signedEulerPitch));
        }

        if (rafaleBody.position.y > Constants.HeightTreshold)    // Height ceiling check
        {
            rafaleBody.AddRelativeForce(Vector3.down * Constants.HeightDrag, ForceMode.Acceleration);
            rafaleBody.AddRelativeTorque(Vector3.down * Constants.HeightDragTurn, ForceMode.VelocityChange);
        }

        if (pitchRollInput != Vector2.zero && planeSpeed > 1)
        {
            if (throttleInput == 0)
            {
                planeDrag += Constants.PlTurnDrag;
            }
            rafaleBody.AddRelativeTorque(pitchRollInput.y * pitchFactor * Vector3.right, ForceMode.Acceleration);
            rafaleBody.AddRelativeTorque(pitchRollInput.x * rollFactor * Vector3.forward, ForceMode.Acceleration);
        }

        if (isAirbourne && yawInput != 0f)
        {
            if (throttleInput == 0)
            {
                planeDrag += Constants.PlTurnDrag;
            }
            rafaleBody.AddRelativeTorque(yawInput * yawFactor * Vector3.up, ForceMode.Acceleration);
        }
        
        if(rafaleBody.drag != planeDrag) rafaleBody.drag = planeDrag;
        if(rafaleBody.angularDrag != planeAngularDrag) rafaleBody.angularDrag = planeAngularDrag;
    }

    private void SetAutoSpeed()
    {
        isAutoSpeedOn = !isAutoSpeedOn;
        if(isAutoSpeedOn)
        {
            autoSpeed = (float)planeMagnitudeRounded;
        }
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
        engineStarted = !engineStarted;
        Debug.Log("Engine: " + engineStarted);
    }

    public void OnPitchroll(InputAction.CallbackContext context)
    {
        pitchRollInput = context.ReadValue<Vector2>();
        planeAngularDrag = 0.05f;
    }

    private void OnCancelPitchroll(InputAction.CallbackContext context)
    {
        pitchRollInput = Vector2.zero;
        //if (!controls.gameplay.roll.IsPressed()) rafaleBody.angularVelocity = Vector3.zero;
        if (yawInput == 0)
        {
            planeAngularDrag = 3f;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void OnYaw(InputAction.CallbackContext context)
    {
        yawInput = context.ReadValue<float>();
        planeAngularDrag = 0.05f;
    }

    private void OnCancelYaw(InputAction.CallbackContext context)
    {
        yawInput = 0f;
        if (pitchRollInput == Vector2.zero)
        {
            planeAngularDrag = 3f;
            StartCoroutine(NullifyAngularSpeed());
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        throttleInput = context.ReadValue<float>();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        brakeInput = context.ReadValue<float>();
    }

    public void OnCancelBrake(InputAction.CallbackContext context)
    {
        brakeInput = 0f;
    }

    public void OnFireweapon(InputAction.CallbackContext context)
    {
        //weapons[selectedWeaponIdx].Fire();
    }

    public void OnStopFiringWeapon(InputAction.CallbackContext context)
    {
        //weapons[selectedWeaponIdx].StopFiring();
    }

    public void OnToggleautospeed(InputAction.CallbackContext context)
    {
        SetAutoSpeed();
        updateAutoSpeedIndicator.Invoke(isAutoSpeedOn, planeMagnitudeRounded);
    }

    public void OnTracktarget(InputAction.CallbackContext context)
    {
        //Debug.Log("Setting target to " + objective);
    }

    public void OnToggletracker(InputAction.CallbackContext context)
    {
        toggleUITracker.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constants.TerrainTagName) && isAirbourne)
        {
            Debug.Log("Landing angle: " + signedEulerPitch);
            isAirbourne = false;
        }
    }

    private IEnumerator NullifyAngularSpeed()
    {
        yield return new WaitForSeconds(0.8f);
        if(pitchRollInput == Vector2.zero && yawInput == 0) rafaleBody.angularVelocity = Vector3.zero;
    }

}
