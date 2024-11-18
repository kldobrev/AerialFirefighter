using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    private InGameMenuController _inGameMenu;
    private ConfirmPromptController _confirmPrompt;
    private UnityEvent<PlayMode> _initInGameMenu;
    private MenuController _currentMenu;
    private MenuController _previousMenu;
    private UnityEvent<byte, byte, float> _screenFadeEffect;
    private PlayerInputHandler _input;
    private UIController _inGameUIController;
    private GameState _previousState;
    public static GameState CurrentState { get; private set; }
    public static PlayMode CurrentPlayMode { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _input = transform.GetComponent<PlayerInputHandler>();
        _screenFadeEffect = new();
        _initInGameMenu = new();
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
        _currentMenu.NavigateMenu(Vector2Int.CeilToInt(movement));
    }

    public void ChooseMenuOption()
    {
        if (CurrentState == GameState.Confirmation)
        {
            _confirmPrompt.GiveResponse();
        }
        else if (CurrentState == GameState.Pause || CurrentState == GameState.GameOver)
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
                    if (CurrentState == GameState.Pause || (CurrentState == GameState.GameOver && CurrentPlayMode == PlayMode.FireMission))
                    {
                        _inGameMenu.ClosePauseMenu();
                        StartCoroutine(ConfirmAndExecute(LoadGameplayScene(SceneManager.GetActiveScene().name)));
                    }
                    else
                    {
                        StartCoroutine(LoadGameplayScene(SceneManager.GetActiveScene().name));
                    }
                    break;
                case 2: // Back to main menu
                    if (CurrentState == GameState.Pause)
                    {
                        Debug.Log("To be implemented.");
                    }
                    else
                    {
                        Debug.Log("To be implemented.");
                    }
                    break;
                case 3: // Exit game
                    _inGameMenu.ClosePauseMenu();
                    StartCoroutine(ConfirmAndExecute(QuitGame()));
                    break;
            }
        }
        
    }

    public void Pause()
    {
        StartCoroutine(PauseCoroutine());
    }

    public void Unpause()
    {
        StartCoroutine(UnpauseCoroutine());
    }

    public void InitGameOver(GameOverType type)
    {
        CurrentState = GameState.GameOver;
        StartCoroutine(_inGameUIController.CrashSequence(type));
    }

    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverCoroutine());
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
        _input.Camera = GameObject.FindWithTag(Constants.CameraTagName).GetComponent<CameraController>();
        _initInGameMenu.AddListener(_inGameMenu.SetInGameMenuForMode);
        _screenFadeEffect.AddListener(_inGameUIController.ScreenFadeInGame);
        _inGameUIController.CrashComplete.AddListener(ShowGameOver);
        _input.Player.SignalGameOver.AddListener(InitGameOver);
        _initInGameMenu.Invoke(CurrentPlayMode);
        _currentMenu = _inGameMenu;
        _previousMenu = _currentMenu;

        ToggleFreezeGameplay(false);
        CurrentState = GameState.Playing;
    }

    private IEnumerator ConfirmAndExecute(IEnumerator action)
    {
        _input.DisableInput();
        _previousMenu = _currentMenu;
        _currentMenu = _confirmPrompt;
        _previousState = CurrentState;
        CurrentState = GameState.Confirmation;
        _confirmPrompt.ResetCursorPosition();
        _confirmPrompt.Open();
        yield return new WaitUntil(() => _confirmPrompt.Opened);
        _input.SwitchToMenuControls();
        yield return new WaitUntil(() => _confirmPrompt.Responded);
        _confirmPrompt.Close();
        yield return new WaitUntil(() => !_confirmPrompt.Opened);
        if (_confirmPrompt.Confirmed)
        {
            StartCoroutine(action);
        }
        else
        {
            StartCoroutine(ReopenPreviousMenu());
        }
    }

    private IEnumerator PauseCoroutine()
    {
        _input.DisableInput();
        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaPause, 
            Constants.ScreenFadePauseSpeed);
        _inGameMenu.ResetCursorPosition();
        _inGameMenu.OpenPauseMenu();
        yield return new WaitUntil(() => _inGameMenu.Opened);
        CurrentState = GameState.Pause;
        ToggleFreezeGameplay(true);
    }

    private IEnumerator UnpauseCoroutine()
    {
        _input.DisableInput();
        _inGameMenu.ClosePauseMenu();
        yield return new WaitUntil(() => !_inGameMenu.Opened);
        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaPause, 
            -Constants.ScreenFadePauseSpeed);
        ToggleFreezeGameplay(false);
        CurrentState = GameState.Playing;
    }

    private IEnumerator ShowGameOverCoroutine()
    {
        _inGameMenu.OpenGameOverMenu();
        yield return new WaitUntil(() => _inGameMenu.Opened);
        CurrentState = GameState.GameOver;
        ToggleFreezeGameplay(true);
    }

    private IEnumerator ReopenPreviousMenu()
    {
        if (_previousState == GameState.Pause)
        {
            _inGameMenu.OpenPauseMenu();
        }
        else if (_previousState == GameState.GameOver)
        {
            _inGameMenu.OpenGameOverMenu();
        }
        yield return new WaitUntil(() => _previousMenu.Opened);
        CurrentState = _previousState;
        _currentMenu = _previousMenu;
        _input.SwitchToMenuControls();
    }

    private IEnumerator QuitGame()
    {
        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax, 
            Constants.ScreenFadeQuitSpeed);
        yield return new WaitUntil(() => UIController.ScreenAlpha == Constants.FadeScreenAlphaMax);
        Application.Quit();
    }

}
