using UnityEngine;



public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory,
        Tutorial,
        Selection,
    }

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject mainCanvas;

    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public GameObject selectionPanel;

    [Header("Main Canvas Panels")]
    public GameObject hudUI;
    public GameObject tutorialUI;
    public GameObject restartPanel;
    public GameObject winScreen;
    public GameObject pauseScreen;

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

    //bools
    public bool fightingCarrot;
    public bool fightingTomato;



    public GameState CurrentState { get; private set; }

     private void Awake()
    {
        spudScript = spud.GetComponent<SpudScript>();
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: keeps it alive across scenes

        SetState(GameState.MainMenu);; //default state

        captCarrotMusic.Stop(); //stop the music
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
        selectionPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        pauseScreen.SetActive(false);
        

        switch (CurrentState)
        {
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
                break;

            case GameState.Playing:
                mainCanvas.SetActive(true);
                hudUI.SetActive(true);
                break;

            case GameState.Paused:
                hudUI.SetActive(true);
                pauseScreen.SetActive(true);
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

    public void SelectBoss()
    {
        SetState(GameState.Selection);
    }


    public void Restart()
    {
        if (fightingCarrot)
        {
            FightCarrot();
        }
        else if (fightingTomato)
        {
            FightTomato();
        }
    }

    public void PauseGame()
    {
        SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (fightingCarrot)
        {
            CaptainCarrotScript script = captainCarrot.GetComponent<CaptainCarrotScript>();
            script.Resume();
        }
        else if (fightingTomato)
        {
            
        }
        SetState(GameState.Playing);
    }

    public void FightCarrot()
    {
        fightingCarrot = true;
        fightingTomato = false;

        captCarrotMusic.Play(); //start playing the music
        camera.transform.position = captCarrotLocation;

        CaptainCarrotScript script = captainCarrot.GetComponent<CaptainCarrotScript>();
        script.RestartCarrot();

        spudScript.Restart();

        StartGame();
    }

    public void FightTomato()
    {
        fightingTomato = true;
        fightingCarrot = false;



        //sgtSplatMusic.Play();
        camera.transform.position = sgtSplatLocation;
        spudScript.Restart();

        StartGame();
    }
    
}
