using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;


public class GameManager : MonoBehaviour
{
    private MainMenuController _mainMenu;
    private InGameMenuController _inGameMenu;
    private ConfirmPromptController _confirmPrompt;
    private UnityEvent<PlayMode> _initInGameMenu;
    private MenuController _currentMenu;
    private MenuController _previousMenu;
    private UnityEvent<byte, byte, float> _screenFadeEffect;
    private PlayerInputHandler _input;
    private UIController _inGameUIController;
    private PlayerController _player;
    private GameState _previousState;
    private int _retriesCount;
    private FireMissionController _fireMissionController;
    private MenuCanvasController _mainMenuCanvas;


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
        StartCoroutine(LoadMainMenu());
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
            switch (_currentMenu.CursorIndex.y)
            {
                case 0:
                    if (CurrentState == GameState.Pause)   // Continue
                    {
                        Unpause();
                    }
                    else if (CurrentState == GameState.GameOver && CurrentPlayMode == PlayMode.FireMission && _player.CheckpointCreated)    // Continue from checkpoint
                    {
                        _inGameMenu.CloseMenu();
                        StartCoroutine(RestartFromCheckpoint());
                    }
                    break;
                case 1: // Restart stage/tutorial
                    if (CurrentState == GameState.Pause || (CurrentState == GameState.GameOver && CurrentPlayMode == PlayMode.FireMission))
                    {
                        _inGameMenu.CloseMenu();
                        StartCoroutine(ConfirmAndExecute(LoadGameplayScene(SceneManager.GetActiveScene().name)));
                    }
                    else
                    {
                        StartCoroutine(LoadGameplayScene(SceneManager.GetActiveScene().name));
                    }
                    break;
                case 2: // Back to main menu
                    _inGameMenu.CloseMenu();
                    StartCoroutine(ConfirmAndExecute(LoadMainMenu()));
                    break;
                case 3: // Exit game
                    _inGameMenu.CloseMenu();
                    StartCoroutine(ConfirmAndExecute(QuitGame()));
                    break;
            }
        }
        else if (CurrentState == GameState.Menu && _currentMenu == _mainMenu)
        {
            _mainMenu.CloseMenu();
            switch (_currentMenu.CursorIndex.y)
            {
                case 0:
                    StartCoroutine(LoadGameplayScene(Constants.Stage1SceneName));
                    break;
                case 1:
                    Debug.Log("TUTORIALS to be implemented");
                    break;
                case 2:
                    Debug.Log("ENDLESS STAGE to be implemented");
                    break;
                case 3:
                    Debug.Log("OPTIONS to be implemented");
                    break;
                case 4:
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

    private IEnumerator RestartFromCheckpoint()
    {
        _input.DisableInput();
        _retriesCount++;
        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaPause, Constants.FadeScreenAlphaMax,
            Constants.ScreenFadeRespawnSpeed);
        yield return null;
        yield return new WaitUntil(() => !UIController.ScreenFadeInProgress && _fireMissionController.IsReadyForCheckpointReload());
        _fireMissionController.RestoreFiresFromCheckpoint();
        _inGameMenu.ActivatePauseMenu();
        _player.RespawnFromCheckpoint();
        CurrentState = GameState.Playing;
        ToggleFreezeGameplay(false);
        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax,
            -Constants.ScreenFadeRespawnSpeed);
        yield return null;
        yield return new WaitUntil(() => !UIController.ScreenFadeInProgress);
    }

    private IEnumerator LoadMainMenu()
    {
        SceneManager.LoadScene(Constants.TitleScene);
        yield return null;

        _mainMenu = GameObject.FindGameObjectWithTag(Constants.MainMenuTag).GetComponent<MainMenuController>();
        _confirmPrompt = GameObject.FindWithTag(Constants.ConfirmPromptMenuTagName).GetComponent<ConfirmPromptController>();
        _mainMenuCanvas = GameObject.FindWithTag(Constants.MainMenuCanvasTag).GetComponent<MenuCanvasController>();
        _screenFadeEffect.RemoveAllListeners();
        _screenFadeEffect.AddListener(_mainMenuCanvas.StartScreenFade);
        _mainMenu.OpenMenu();
        yield return new WaitUntil(() => _mainMenu.Opened);
        _input.SwitchToMenuControls();
        _currentMenu = _mainMenu;
        CurrentState = GameState.Menu;
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
        _player = _input.Player;
        _input.Camera = GameObject.FindWithTag(Constants.CameraTagName).GetComponent<CameraController>();
        _initInGameMenu.RemoveAllListeners();
        _initInGameMenu.AddListener(_inGameMenu.SetInGameMenuForMode);
        _screenFadeEffect.RemoveAllListeners();
        _screenFadeEffect.AddListener(_inGameUIController.StartScreenFade);
        _inGameUIController.CrashComplete.RemoveAllListeners();
        _inGameUIController.CrashComplete.AddListener(ShowGameOver);
        _player.SignalGameOver.RemoveAllListeners();
        _player.SignalGameOver.AddListener(InitGameOver);
        _initInGameMenu.Invoke(CurrentPlayMode);
        _currentMenu = _inGameMenu;
        _previousMenu = _currentMenu;
        _fireMissionController = GameObject.FindGameObjectWithTag(Constants.FireGroupsContainerTag).GetComponent < FireMissionController>();

        Transform ground = GameObject.Find(Constants.TerrainPieceTagName).transform;
        for (int i = 0; i < ground.childCount; i++)
        {
            ground.GetChild(i).tag = Constants.TerrainTagName;
        }

        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax,
            -Constants.ScreenFadeQuitSpeed);
        ToggleFreezeGameplay(false);
        CurrentState = GameState.Playing;
        yield return null;
        yield return new WaitUntil(() => !UIController.ScreenFadeInProgress);
    }

    private IEnumerator ConfirmAndExecute(IEnumerator action)
    {
        _input.DisableInput();
        _previousMenu = _currentMenu;
        _currentMenu = _confirmPrompt;
        _previousState = CurrentState;
        CurrentState = GameState.Confirmation;
        _confirmPrompt.ResetCursorPosition();
        _confirmPrompt.OpenMenu();
        yield return new WaitUntil(() => _confirmPrompt.Opened);
        _input.SwitchToMenuControls();
        yield return new WaitUntil(() => _confirmPrompt.Responded);
        _confirmPrompt.CloseMenu();
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
        _inGameMenu.OpenMenu();
        yield return new WaitUntil(() => _inGameMenu.Opened);
        CurrentState = GameState.Pause;
        ToggleFreezeGameplay(true);
    }

    private IEnumerator UnpauseCoroutine()
    {
        _input.DisableInput();
        _inGameMenu.CloseMenu();
        yield return new WaitUntil(() => !_inGameMenu.Opened);
        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaPause, 
            -Constants.ScreenFadePauseSpeed);
        ToggleFreezeGameplay(false);
        CurrentState = GameState.Playing;
    }

    private IEnumerator ShowGameOverCoroutine()
    {
        _inGameMenu.OpenMenu();
        yield return new WaitUntil(() => _inGameMenu.Opened);
        CurrentState = GameState.GameOver;
        ToggleFreezeGameplay(true);
    }

    private IEnumerator ReopenPreviousMenu()
    {
        _previousMenu.OpenMenu();
        yield return new WaitUntil(() => _previousMenu.Opened);
        CurrentState = _previousState;
        _currentMenu = _previousMenu;
        _input.SwitchToMenuControls();
    }

    private IEnumerator QuitGame()
    {
        _screenFadeEffect.Invoke(Constants.FadeScreenAlphaMin, Constants.FadeScreenAlphaMax, 
            Constants.ScreenFadeQuitSpeed);
        yield return null;
        yield return new WaitUntil(() => !UIController.ScreenFadeInProgress);
        Application.Quit();
    }

}
