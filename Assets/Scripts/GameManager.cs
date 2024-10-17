using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InGameMenuController _inGameMenu;
    [SerializeField]
    private ConfirmPromptController _confirmPrompt;
    [SerializeField]
    private UnityEvent<PlayMode> _initInGameMenu;
    [SerializeField]
    private UnityEvent<GameOverType> _initCrash;

    private MenuController _currentMenu;
    private UnityEvent _confirmOption;
    private UnityEvent _backMenu;
    private PlayerInputHandler _input;
    private UIController _inGameUIController;
    public static GameState CurrentState { get; private set; }
    public static PlayMode CurrentPlayMode { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _input = transform.GetComponent<PlayerInputHandler>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _input.SwitchToGameplayControls();
        StartCoroutine(LoadGameplayScene(Constants.Stage1SceneName));
        CurrentPlayMode = PlayMode.FireMission;

        if (CurrentState == GameState.Playing)
        {
            _currentMenu = _inGameMenu;
        }
    }

    public void GoBack()
    {
        if (CurrentState == GameState.Pause)
        {
            Unpause();
        }
    }

    public void Navigate(Vector2 movement)
    {
        if (CurrentState == GameState.Pause || CurrentState == GameState.GameOver)
        {
            _inGameMenu.NavigateMenu(Vector2Int.CeilToInt(movement));
        }
    }

    public void ChooseMenuOption()
    {
        if (CurrentState == GameState.Pause || CurrentState == GameState.GameOver)
        {
            switch (_inGameMenu.CursorIndex.y)
            {
                case 0:
                    if (CurrentState == GameState.Pause)   // Continue
                    {
                        Unpause();
                    }
                    else if (CurrentState == GameState.GameOver && CurrentPlayMode == PlayMode.FireMission)    // Continue from checkpoint
                    {
                        Debug.Log("To be implemented");
                    }
                    break;
                case 1: // Restart stage/tutorial
                    if (CurrentState == GameState.Pause)
                    {
                        StartCoroutine(ConfirmAndExecute(LoadGameplayScene(SceneManager.GetActiveScene().name)));
                    }
                    else
                    {
                        StartCoroutine(LoadGameplayScene(SceneManager.GetActiveScene().name));
                    }
                    break;
                case 2: // Back to main menu
                    Debug.Log("To be implemented.");
                    break;
                case 3: // Exit game
                    Application.Quit();
                    break;
            }
        }
        
    }

    public void Pause()
    {
        _inGameMenu.ShowPuseMenu();
        CurrentState = GameState.Pause;
    }

    public void Unpause()
    {
        _inGameMenu.HidePuseMenu();
        CurrentState = GameState.Playing;
    }

    public void InitGameOver(GameOverType type)
    {
        CurrentState = GameState.GameOver;
        _initCrash.Invoke(type);
    }

    public void ShowGameOver()
    {
        _inGameMenu.ShowGameOverMenu();
    }

    public void ToggleFreezeGameplay(bool freeze)
    {
        if (freeze)
        {
            Time.timeScale = 0;
            _input.SwitchToMenuControls();
        }
        else
        {
            Time.timeScale = 1;
            _input.SwitchToGameplayControls();
        }
    }

    private IEnumerator LoadMainMenu()
    {
        Time.timeScale = 1;
        _input.SwitchToMenuControls();
        // To be implemented
        yield return null;
    }

    private IEnumerator LoadGameplayScene(string sceneName)
    {
        _input.DisableInput();
        SceneManager.LoadScene(sceneName);
        yield return null;

        _inGameUIController = GameObject.FindWithTag(Constants.InGameUICanvasTagName).GetComponent<UIController>();
        _inGameMenu = GameObject.FindWithTag(Constants.InGameMenuTagNam).GetComponent<InGameMenuController>();
        _confirmPrompt = GameObject.FindWithTag(Constants.ConfirmPromptMenuTagName).GetComponent<ConfirmPromptController>();
        _input.Player = GameObject.FindWithTag(Constants.PlayerTagName).GetComponent<PlayerController>();
        _initInGameMenu.AddListener(_inGameMenu.SetInGameMenuForMode);
        _initCrash.AddListener(_inGameUIController.CrashSequence);
        _inGameUIController.CrashComplete.AddListener(ShowGameOver);
        _inGameMenu.MenuReady.AddListener(ToggleFreezeGameplay);
        _input.Player.SignalGameOver.AddListener(InitGameOver);
        _initInGameMenu.Invoke(CurrentPlayMode);

        ToggleFreezeGameplay(false);
        CurrentState = GameState.Playing;
    }

    private IEnumerator ConfirmAndExecute(IEnumerator action)
    {
        _confirmPrompt.Show();
        yield return new WaitUntil(() => _confirmPrompt.Responded);
        if (_confirmPrompt.Confirmed)
        {
            StartCoroutine(action);
        }
        /*executedMethod += menuAction;
        executedMethod();*/
    }

}
