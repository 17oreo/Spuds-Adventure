using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;



public enum GameState
    {
        Cutscene,
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory,
        Tutorial,
        Selection,
        Settings
    }

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject mainCanvas;
    public GameObject cutsceneCanvas;

    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public GameObject selectionPanel;
    public GameObject settingsPanel;
        //images
    [SerializeField] private UnityEngine.UI.Image tomatoImage;
    [SerializeField] private UnityEngine.UI.Image carrotImage;


    [Header("Main Canvas Panels")]
    public GameObject hudUI;
    public GameObject tutorialUI;
    public GameObject restartPanel;
    public GameObject winScreen;
    public GameObject pauseScreen;

    [Header("Cutscene Canvas")]
    public GameObject firstCutScene;
    public GameObject endCutScene;

    [Header("Camera")]
    [SerializeField] private Camera camera;
    [SerializeField] private Vector3 tutorialLocation;
    [SerializeField] private Vector3 captCarrotLocation;
    [SerializeField] private Vector3 sgtSplatLocation;

    [Header("Audio")]
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource captCarrotMusic;

    [Header("Characters")]
    [SerializeField] private GameObject spud;
    private SpudScript spudScript;
    [SerializeField] private GameObject captainCarrot;
    [SerializeField] private GameObject sgtSplat;
    private CaptainCarrotScript captainCarrotScript;
    private SgtSplatScript sgtSplatScript;

    //bools
    public bool fightingCarrot;
    public bool defeatedCarrot = false;
    public bool fightingTomato;
    public bool defeatedTomato = false;

    public bool gameWon = false;
    public bool cutScene1Played = false;
    public bool cutScene2Played = false;


    
    public bool IsGamePaused { get; private set; }



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

        spudScript = spud.GetComponent<SpudScript>();
        captainCarrotScript = captainCarrot.GetComponent<CaptainCarrotScript>();
        sgtSplatScript = sgtSplat.GetComponent<SgtSplatScript>();
        
        tomatoImage.color = new Color(0 / 255f, 0 / 255f, 0 / 255f);
        carrotImage.color = new Color(0 / 255f, 0 / 255f, 0 / 255f);

        
        //SetState(GameState.Cutscene); //default state
        SetState(GameState.MainMenu);; //default state

        captCarrotMusic.Stop(); //stop the music
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        gameWon = defeatedCarrot && defeatedTomato;
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        UpdateUI(); 
    }

    public void UpdateUI()
    {
        // Set defaults
        mainMenuCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        cutsceneCanvas.SetActive(false);
        hudUI.SetActive(false);
        restartPanel.SetActive(false);
        winScreen.SetActive(false);
        tutorialUI.SetActive(false);
        selectionPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        pauseScreen.SetActive(false);
        settingsPanel.SetActive(false);
        firstCutScene.SetActive(false);
        endCutScene.SetActive(false);
        

        switch (CurrentState)
        {
            case GameState.Cutscene:
                //Debug.Log("Trying to open the cutsceneCanvas");
                cutsceneCanvas.SetActive(true);

                if (!gameWon)
                {
                    firstCutScene.SetActive(true);
                }
                else
                {
                    endCutScene.SetActive(true);
                }
                break;
            case GameState.MainMenu:
                mainMenuCanvas.SetActive(true);
                mainMenuPanel.SetActive(true);
                camera.transform.position = captCarrotLocation;
                captCarrotMusic.Stop();
                break;
            case GameState.Selection:
                mainMenuCanvas.SetActive(true);
                selectionPanel.SetActive(true);
                camera.transform.position = captCarrotLocation;
                captCarrotMusic.Stop();

                if (defeatedTomato)
                {
                    tomatoImage.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                }
                if (defeatedCarrot)
                {
                    carrotImage.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                }

                break;
            case GameState.Settings:
                mainMenuCanvas.SetActive(true);
                settingsPanel.SetActive(true);
                break;
            case GameState.Playing:
                mainCanvas.SetActive(true);
                hudUI.SetActive(true);
                break;

            case GameState.Paused:
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
        //Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void StartTutorial()
    {
        SetState(GameState.Tutorial);
    }

    public void MainMenu()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
        SetState(GameState.MainMenu);
    }

    public void CutScene()
    {
        if (!cutScene1Played)
        {
            SetState(GameState.Cutscene);
        }
        else
        {
            SetState(GameState.Selection);
        }
    }

    public void SelectBoss()
    {
        SetState(GameState.Selection);
    }

    public void Settings()
    {
        SetState(GameState.Settings);
    }


    public void Restart()
    {
        Time.timeScale = 1f;

        captainCarrotScript.destroyVine = true;
        captainCarrotScript.destroyCarrot = true;

        sgtSplatScript.destroyKetchup = true;
        sgtSplatScript.destroyLaser = true;

        sgtSplatScript.StopAnyCoroutines();
        captainCarrotScript.StopAnyCoroutines();    

        
        if (fightingCarrot)
        {
            FightCarrot();
        }
        else if (fightingTomato)
        {
            FightTomato();
        }
    }

    public void TogglePause()
    {
        if (IsGamePaused)
            ResumeGame();
        else
            PauseGame();
    }


    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        IsGamePaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        IsGamePaused = false;
    }

    public void FightCarrot()
    {
        fightingCarrot = true;
        fightingTomato = false;

        captCarrotMusic.Play(); //start playing the music
        camera.transform.position = captCarrotLocation;

        
        captainCarrotScript.RestartCarrot();

        spudScript.Restart();

        StartGame();
    }

    public void FightTomato()
    {
        fightingTomato = true;
        fightingCarrot = false;



        //sgtSplatMusic.Play();
        camera.transform.position = sgtSplatLocation;
        
        sgtSplatScript.RestartTomato();
        spudScript.Restart();

        StartGame();
    }
    
}
