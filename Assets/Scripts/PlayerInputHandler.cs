using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, PlayerControls.IGameplayActions, PlayerControls.IMenuActions
{
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private UnityEvent pauseGame;
    [SerializeField]
    private UnityEvent prevGameState;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.startstopengine.canceled += OnStartstopengine;
        controls.gameplay.pitchroll.performed += OnPitchroll;
        controls.gameplay.pitchroll.canceled += OnCancelPitchroll;
        controls.gameplay.yaw.performed += OnYaw;
        controls.gameplay.yaw.canceled += OnCancelYaw;
        controls.gameplay.accelerate.performed += OnAccelerate;
        controls.gameplay.accelerate.canceled += OnCancelAccelerate;
        controls.gameplay.brake.performed += OnBrake;
        controls.gameplay.brake.canceled += OnCancelBrake;
        controls.gameplay.dropwater.canceled += OnDropwater;
        controls.gameplay.toggleautospeed.performed += OnToggleautospeed;
        controls.gameplay.pause.performed += OnPause;
        controls.menu.confirm.canceled += OnConfirm;
        controls.menu.back.canceled += OnBack;
        controls.menu.navigation.performed += OnNavigation;
    }


    // Start is called before the first frame update
    void Start()
    {
        SwitchToGameplayControls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToGameplayControls()
    {
        controls.menu.Disable();
        controls.gameplay.Enable();
    }

    public void SwitchToMenuControls()
    {
        controls.gameplay.Disable();
        controls.menu.Enable();
    }

    public void OnStartstopengine(InputAction.CallbackContext context)
    {
        player.ToggleEngine();
    }

    public void OnPitchroll(InputAction.CallbackContext context)
    {
        player.PitchRoll(context.ReadValue<Vector2>());
    }

    private void OnCancelPitchroll(InputAction.CallbackContext context)
    {
        if (!player.IsUnityNull())
        {
            player.CancelPitchroll();
        }
    }

    public void OnYaw(InputAction.CallbackContext context)
    {
        player.Yaw(context.ReadValue<float>());
    }

    private void OnCancelYaw(InputAction.CallbackContext context)
    {
        if (!player.IsUnityNull())
        {
            player.CancelYaw();
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        player.Accelerate(context.ReadValue<float>());
    }

    public void OnCancelAccelerate(InputAction.CallbackContext context)
    {
        player.CancelAccelerate();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        player.Brake(context.ReadValue<float>());
    }

    public void OnCancelBrake(InputAction.CallbackContext context)
    {
        player.CancelBrake();
    }

    public void OnDropwater(InputAction.CallbackContext context)
    {
        player.ToggleDropwater();
    }

    public void OnToggleautospeed(InputAction.CallbackContext context)
    {
        player.ToggleAutoSpeed();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        pauseGame.Invoke();
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        Debug.Log("Confirm");
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        prevGameState.Invoke();
    }

    public void OnNavigation(InputAction.CallbackContext context)
    {
        Debug.Log("Navigation");
    }

    private void OnEnable()
    {
        controls.gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.gameplay.Disable();
    }
}
