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
    private UnityEvent pauseMenu;
    [SerializeField]
    private UnityEvent<PlayMode> initInGameMenu;
    [SerializeField]
    private UnityEvent<GameOverType> initGameOver;
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
    }

    public void GoBack()
    {
        if (state == GameState.Pause)
        {
            Unpause();
        }
    }

    public void Pause()
    {
        pauseMenu.Invoke();
        state = GameState.Pause;
    }

    public void Unpause()
    {
        state = GameState.Playing;
        pauseMenu.Invoke();
    }

    public void GameOver(GameOverType type)
    {
        state = GameState.GameOver;
        initGameOver.Invoke(type);
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
