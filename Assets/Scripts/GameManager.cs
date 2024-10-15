using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InGameMenuController inGameMenu;
    [SerializeField]
    private ConfirmPromptController confirmPrompt;
    [SerializeField]
    private UnityEvent<PlayMode> initInGameMenu;
    [SerializeField]
    private UnityEvent<GameOverType> initCrash;

    private MenuController currentMenu;
    private UnityEvent<Vector2> navigateMenu;
    private UnityEvent confirmOption;
    private UnityEvent backMenu;
    private PlayerInputHandler input;
    private UIController inGameUIController;
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
    private void Start()
    {
        input.SwitchToGameplayControls();
        StartCoroutine(LoadGameplayScene(Constants.Stage1SceneName));
        currentMode = PlayMode.FireMission;

        if (state == GameState.Playing)
        {
            currentMenu = inGameMenu;
        }
    }

    public void GoBack()
    {
        if (state == GameState.Pause)
        {
            Unpause();
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
        if (state == GameState.Pause || state == GameState.GameOver)
        {
            switch (inGameMenu.CursorIndex.y)
            {
                case 0:
                    if (state == GameState.Pause)   // Continue
                    {
                        Unpause();
                    }
                    else if (state == GameState.GameOver && currentMode == PlayMode.FireMission)    // Continue from checkpoint
                    {
                        Debug.Log("To be implemented");
                    }
                    break;
                case 1: // Restart stage/tutorial
                    StartCoroutine(LoadGameplayScene(SceneManager.GetActiveScene().name));
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
        inGameMenu.ShowPuseMenu();
        state = GameState.Pause;
    }

    public void Unpause()
    {
        inGameMenu.HidePuseMenu();
        state = GameState.Playing;
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

    public void ToggleFreezeGameplay(bool freeze)
    {
        if (freeze)
        {
            Time.timeScale = 0;
            input.SwitchToMenuControls();
        }
        else
        {
            Time.timeScale = 1;
            input.SwitchToGameplayControls();
        }
    }

    private IEnumerator LoadMainMenu()
    {
        Time.timeScale = 1;
        input.SwitchToMenuControls();
        // To be implemented
        yield return null;
    }

    private IEnumerator LoadGameplayScene(string sceneName)
    {
        input.DisableInput();
        SceneManager.LoadScene(sceneName);
        yield return null;

        inGameUIController = GameObject.FindWithTag(Constants.InGameUICanvasTagName).GetComponent<UIController>();
        inGameMenu = GameObject.FindWithTag(Constants.InGameMenuTagNam).GetComponent<InGameMenuController>();
        confirmPrompt = GameObject.FindWithTag(Constants.ConfirmPromptMenuTagName).GetComponent<ConfirmPromptController>();
        input.player = GameObject.FindWithTag(Constants.PlayerTagName).GetComponent<PlayerController>();
        initInGameMenu.AddListener(inGameMenu.SetInGameMenuForMode);
        initCrash.AddListener(inGameUIController.CrashSequence);
        inGameUIController.crashComplete.AddListener(ShowGameOver);
        inGameMenu.menuReady.AddListener(ToggleFreezeGameplay);
        input.player.signalGameOver.AddListener(InitGameOver);
        initInGameMenu.Invoke(currentMode);

        ToggleFreezeGameplay(false);
        state = GameState.Playing;
    }

}
