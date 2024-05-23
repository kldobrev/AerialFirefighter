using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, PlayerControls.IGameplayActions
{
    private PlayerControls controls;
    [SerializeField]
    private Transform bodyTransform;
    [SerializeField]
    private Rigidbody rafaleBody;
    [SerializeField]
    private float throttleAcceleration;
    [SerializeField]
    private float pitchFactor;
    [SerializeField]
    private float yawFactor;
    [SerializeField]
    private float rollFactor;
    [SerializeField]
    private float liftForce;
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
    [SerializeField]
    private UnityEvent trackTarget;
    [SerializeField]
    private UnityEvent stopTrackingTarget;
    [SerializeField]
    private UnityEvent<bool, float, float> sendWeaponDataToTracker;


    private float accelerateValue;
    private float throttleInput;
    private float brakeInput;
    private Vector2 torqueInput;
    private float rollInput;
    private Animator playerAnimator;
    private float autoSpeed;
    private bool isAirbourne;
    private bool isAutoSpeedOn;
    private float airbourneThresholdY;
    private float planeDrag;
    private Transform rigBodyTransform;
    private int planeMagnitudeRounded;
    private int sendHeightFrameCount;
    private int sendCoordsFrameCount;
    private int sendSpeedFrameCount;
    private int selectedWeaponIdx;
    public static Transform PlayerBodyTransform;


    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.turn.performed += OnTurn;
        controls.gameplay.turn.canceled += OnCancelTurn;
        controls.gameplay.roll.performed += OnRoll;
        controls.gameplay.roll.canceled += OnCancelRoll;
        controls.gameplay.accelerate.performed += OnAccelerate;
        controls.gameplay.accelerate.canceled += context => throttleInput = 0f;
        controls.gameplay.brake.performed += OnBrake;
        controls.gameplay.brake.canceled += OnCancelBrake;
        controls.gameplay.fireweapon.performed += OnFireweapon;
        controls.gameplay.fireweapon.canceled += OnStopFiringWeapon;
        controls.gameplay.switchweapon.canceled += OnSwitchweapon;
        controls.gameplay.toggleautospeed.performed += OnToggleautospeed;
        controls.gameplay.tracktarget.performed += OnTracktarget;
        controls.gameplay.stoptrackingtarget.performed += OnStoptrackingtarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = transform.GetComponent<Animator>();
        isAirbourne = false;
        isAutoSpeedOn = false;
        airbourneThresholdY = transform.position.y + 10;
        autoSpeed = 0;
        planeMagnitudeRounded = 0;
        planeDrag = Constants.PlDefaultDrag;
        sendHeightFrameCount = sendCoordsFrameCount = sendSpeedFrameCount = 0;
        planeDrag = Constants.PlDefaultDrag;
        selectedWeaponIdx = 0;
        UIAmmoTracker = transform.GetComponent<AmmunitionUITracker>();
        rigBodyTransform = rafaleBody.transform;
        //weapons[0].SetWeapon(Constants.HeatseekerMissile);   // Will be set by player
        //weapons[1].SetWeapon(Constants.BulletCannon);   // Will be set by player
        //UIAmmoTracker.UpdateWeaponAmmoInUI(weapons[selectedWeaponIdx].Ammunition);
        //sendWeaponDataToTracker.Invoke(true, weapons[selectedWeaponIdx].Range, weapons[selectedWeaponIdx].LockingStep);
        PlayerBodyTransform = bodyTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAirbourne && transform.position.y > airbourneThresholdY)
        {
            isAirbourne = true;
        }

        if(sendHeightFrameCount == Constants.SendHeightFramerule)
        {
            sendHeightFrameCount = 0;
            updateHeightMeter.Invoke(transform.position.y);
        }
        else
        {
            sendHeightFrameCount++;
        }
        
        if(sendCoordsFrameCount == Constants.SendCoordsFramerule)
        {
            sendCoordsFrameCount = 0;
            updateRadarCamera.Invoke(new Vector2(transform.position.x, transform.position.z), transform.rotation.eulerAngles.y);
        }
        else
        {
            sendCoordsFrameCount++;
        }

        if (sendSpeedFrameCount == Constants.SendSpeedFramerule)
        {
            sendSpeedFrameCount = 0;
            planeMagnitudeRounded = Mathf.RoundToInt(rafaleBody.velocity.magnitude);
            setSpeedometerSpeed.Invoke(planeMagnitudeRounded);
        }
        else
        {
            sendSpeedFrameCount++;
        }

    }

    void FixedUpdate()
    {
        accelerateValue = 0;
        if (throttleInput != 0)  // Accelerate using player input ignoring auto speed value
        {
            accelerateValue = throttleInput * throttleAcceleration;
        }
        else
        {
            if (isAirbourne && isAutoSpeedOn) // Maintain constant speed if enabled
            {
                accelerateValue = rafaleBody.velocity.magnitude < autoSpeed ? throttleAcceleration : 0;
            }   
        }
        
        if (brakeInput != 0)    // Brake engaged
        {
            if(rafaleBody.velocity.magnitude >= Constants.PlMinSpeedAir)
            {
                planeDrag += Constants.PlBrakeDrag;
            }
        }
        else
        {
            rafaleBody.AddRelativeForce(Vector3.forward * accelerateValue, ForceMode.Acceleration);
            rafaleBody.AddRelativeForce(Vector3.up * rafaleBody.velocity.magnitude * liftForce, ForceMode.Acceleration);
        }

        if(rafaleBody.position.y > Constants.HeightTreshold)    // Height ceiling check
        {
            rafaleBody.AddRelativeForce(Vector3.down * Constants.HeightDrag, ForceMode.Acceleration);
            rafaleBody.AddRelativeTorque(Vector3.down * Constants.HeightDragTurn, ForceMode.VelocityChange);
        }

        if (torqueInput != Vector2.zero && rafaleBody.velocity.magnitude > 1)
        {
            if (throttleInput == 0)
            {
                planeDrag = Constants.PlTurnDrag;
            }
            rafaleBody.AddRelativeTorque(torqueInput.y * pitchFactor * Vector3.right, ForceMode.Acceleration);
            rafaleBody.AddRelativeTorque(torqueInput.x * yawFactor * Vector3.up, ForceMode.Acceleration);
        }

        if (isAirbourne && rollInput != 0f)
        {
            if (throttleInput == 0)
            {
                planeDrag = Constants.PlTurnDrag;
            }
            rafaleBody.AddRelativeTorque(rollInput * rollFactor * Vector3.forward, ForceMode.Acceleration);
        }
        rafaleBody.drag = planeDrag;
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

    // Controls section

    public void OnTurn(InputAction.CallbackContext context)
    {
        torqueInput = context.ReadValue<Vector2>();
    }

    private void OnCancelTurn(InputAction.CallbackContext context)
    {
        torqueInput = Vector2.zero;
        planeDrag = Constants.PlDefaultDrag;
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        rollInput = context.ReadValue<float>();
    }

    private void OnCancelRoll(InputAction.CallbackContext context)
    {
        rollInput = 0f;
        planeDrag = Constants.PlDefaultDrag;
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        throttleInput = context.ReadValue<float>();
        planeDrag = Constants.PlDefaultDrag;
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        brakeInput = context.ReadValue<float>();
    }

    public void OnCancelBrake(InputAction.CallbackContext context)
    {
        brakeInput = 0f;
        planeDrag = Constants.PlDefaultDrag;
    }

    public void OnFireweapon(InputAction.CallbackContext context)
    {
        //weapons[selectedWeaponIdx].Fire();
    }

    public void OnStopFiringWeapon(InputAction.CallbackContext context)
    {
        //weapons[selectedWeaponIdx].StopFiring();
    }

    public void OnSwitchweapon(InputAction.CallbackContext context)
    {
        selectedWeaponIdx = selectedWeaponIdx == Constants.MaxNumWeapons - 1 ? 0 : selectedWeaponIdx + 1;
        //UIAmmoTracker.SwitchSelectedWeaponInUI(selectedWeaponIdx, weapons[selectedWeaponIdx]);
        //UIAmmoTracker.UpdateWeaponAmmoInUI(weapons[selectedWeaponIdx].Ammunition);
        //sendWeaponDataToTracker.Invoke(selectedWeaponIdx == 0, weapons[selectedWeaponIdx].Range, weapons[selectedWeaponIdx].LockingStep);
    }

    public void OnToggleautospeed(InputAction.CallbackContext context)
    {
        SetAutoSpeed();
        updateAutoSpeedIndicator.Invoke(isAutoSpeedOn, planeMagnitudeRounded);
    }

    public void OnTracktarget(InputAction.CallbackContext context)
    {
        trackTarget.Invoke();
    }

    public void OnStoptrackingtarget(InputAction.CallbackContext context)
    {
        stopTrackingTarget.Invoke();
    }

}
