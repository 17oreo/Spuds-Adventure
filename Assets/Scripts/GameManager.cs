using UnityEngine;



public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory,
        Tutorial,
    }

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject mainCanvas;

    [Header("Main Canvas Panels")]
    public GameObject hudUI;
    public GameObject tutorialUI;
    public GameObject restartPanel;
    public GameObject winScreen;

    [Header("Camera")]
    [SerializeField] private Camera camera;
    [SerializeField] private Vector3 tutorialLocation;
    [SerializeField] private Vector3 mainLocation;



    public GameState CurrentState { get; private set; }

     private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: keeps it alive across scenes

        SetState(GameState.MainMenu);; //default state
    }

    

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        UpdateUI(); // 👈 Make sure UI updates on state change
    }

    public void UpdateUI()
    {
        // Set defaults
        mainMenuCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        hudUI.SetActive(false);
        restartPanel.SetActive(false);
        winScreen.SetActive(false);
        tutorialUI.SetActive(false);

        switch (CurrentState)
        {
            case GameState.MainMenu:
                mainMenuCanvas.SetActive(true);
                camera.transform.position = mainLocation;
                break;

            case GameState.Playing:
                mainCanvas.SetActive(true);
                hudUI.SetActive(true);
                break;

            case GameState.Paused:
                // Optional pause panel
                break;

            case GameState.GameOver:
                mainCanvas.SetActive(true);
                restartPanel.SetActive(true);
                break;

            case GameState.Victory:
                mainCanvas.SetActive(true);
                winScreen.SetActive(true);
                break;
            case GameState.Tutorial:
                mainCanvas.SetActive(true);
                tutorialUI.SetActive(true);
                camera.transform.position = tutorialLocation;
                break;

        }
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void StartTutorial()
    {
        SetState(GameState.Tutorial);
    }

    public void MainMenu()
    {
        SetState(GameState.MainMenu);
    }
}
