using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private InGameMenuController inGameMenu;
    [SerializeField]
    private UnityEvent<PlayMode> initInGameMenu;
    [SerializeField]
    private UnityEvent<GameOverType> initCrash;

    private MenuController currentMenu;
    private UnityEvent<Vector2> navigateMenu;
    private UnityEvent confirmOption;
    private UnityEvent backMenu;
    private PlayerInputHandler input;
    private static GameState state;
    private static PlayMode currentMode;
    public static GameState CurrentState => state;
    public static PlayMode CurrentPlayMode => currentMode;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        input = transform.GetComponent<PlayerInputHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Playing;
        currentMode = PlayMode.FireMission;
        initInGameMenu.Invoke(currentMode);

        if (state == GameState.Playing)
        {
            currentMenu = inGameMenu;
        }
    }

    public void GoBack()
    {
        if (state == GameState.Pause)
        {
            inGameMenu.HidePuseMenu();
        }
    }

    public void Navigate(Vector2 movement)
    {
        if (state == GameState.Pause || state == GameState.GameOver)
        {
            inGameMenu.NavigateMenu(Vector2Int.CeilToInt(movement));
        }
    }

    public void ChooseMenuOption()
    {
        Debug.Log("Confirm option.");
    }

    public void Pause()
    {
        inGameMenu.ShowPuseMenu();
        state = GameState.Pause;
    }

    public void InitGameOver(GameOverType type)
    {
        state = GameState.GameOver;
        initCrash.Invoke(type);
    }
    public void ShowGameOver()
    {
        inGameMenu.ShowGameOverMenu();
    }

    public void ToggleFreezeGameplay()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            input.SwitchToGameplayControls();
        }
        else
        {
            Time.timeScale = 0;
            input.SwitchToMenuControls();
        }
    }

}
