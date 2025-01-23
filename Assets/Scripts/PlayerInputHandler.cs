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
    private UnityEvent _pauseGame;
    [SerializeField]
    private UnityEvent _passBack;
    [SerializeField]
    private UnityEvent _passConfirm;
    [SerializeField]
    private UnityEvent<Vector2> _passNavData;

    public PlayerController Player { get; set; }
    public CameraController Camera { get; set; }
    private PlayerControls _controls;

    void Awake()
    {
        _controls = new PlayerControls();

        _controls.gameplay.startstopengine.canceled += OnStartstopengine;
        _controls.gameplay.pitch.performed += OnPitch;
        _controls.gameplay.pitch.canceled += OnCancelPitch;
        _controls.gameplay.bank.performed += OnBank;
        _controls.gameplay.bank.canceled += OnCancelBank;
        _controls.gameplay.yaw.performed += OnYaw;
        _controls.gameplay.yaw.canceled += OnCancelYaw;
        _controls.gameplay.accelerate.performed += OnAccelerate;
        _controls.gameplay.accelerate.canceled += OnCancelAccelerate;
        _controls.gameplay.brake.performed += OnBrake;
        _controls.gameplay.brake.canceled += OnCancelBrake;
        _controls.gameplay.dropwater.canceled += OnDropwater;
        _controls.gameplay.toggleautospeed.performed += OnToggleautospeed;
        _controls.gameplay.changecameramode.performed += OnChangecameramode;
        _controls.gameplay.pause.performed += OnPause;
        _controls.menu.confirm.canceled += OnConfirm;
        _controls.menu.back.canceled += OnBack;
        _controls.menu.navigation.performed += OnNavigation;
    }

    public void SwitchToGameplayControls()
    {
        _controls.menu.Disable();
        _controls.gameplay.Enable();
    }

    public void SwitchToMenuControls()
    {
        _controls.gameplay.Disable();
        _controls.menu.Enable();
    }

    public void DisableInput()
    {
        _controls.gameplay.Disable();
        _controls.menu.Disable();
    }

    public void OnStartstopengine(InputAction.CallbackContext context)
    {
        Player.ToggleEngine();
    }

    public void OnPitch(InputAction.CallbackContext context)
    {
        Player.Pitch(context.ReadValue<float>());
    }

    private void OnCancelPitch(InputAction.CallbackContext context)
    {
        if (!Player.IsUnityNull())
        {
            Player.CancelPitch();
        }
    }

    public void OnBank(InputAction.CallbackContext context)
    {
        Player.Bank(context.ReadValue<float>());
    }

    private void OnCancelBank(InputAction.CallbackContext context)
    {
        if (!Player.IsUnityNull())
        {
            Player.CancelBank();
        }
    }

    public void OnYaw(InputAction.CallbackContext context)
    {
        Player.Yaw(context.ReadValue<float>());
    }

    private void OnCancelYaw(InputAction.CallbackContext context)
    {
        if (!Player.IsUnityNull())
        {
            Player.CancelYaw();
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        Player.Accelerate(context.ReadValue<float>());
    }

    public void OnCancelAccelerate(InputAction.CallbackContext context)
    {
        Player.CancelAccelerate();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        Player.Brake(context.ReadValue<float>());
    }

    public void OnCancelBrake(InputAction.CallbackContext context)
    {
        Player.CancelBrake();
    }

    public void OnDropwater(InputAction.CallbackContext context)
    {
        Player.ToggleDropwater();
    }

    public void OnToggleautospeed(InputAction.CallbackContext context)
    {
        Player.ToggleAutoSpeed();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        _pauseGame.Invoke();
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        _passConfirm.Invoke();
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        _passBack.Invoke();
    }

    public void OnNavigation(InputAction.CallbackContext context)
    {
        _passNavData.Invoke(context.ReadValue<Vector2>());
    }

    public void OnChangecameramode(InputAction.CallbackContext context)
    {
        Camera.SetCameraMode(context.ReadValue<float>());
    }
}
