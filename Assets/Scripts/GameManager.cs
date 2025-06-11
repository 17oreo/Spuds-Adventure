using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
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
    public GameObject settingsPanel;

    //images
    [SerializeField] private UnityEngine.UI.Image tomatoImage;
    [SerializeField] private UnityEngine.UI.Image carrotImage;
    [SerializeField] private UnityEngine.UI.Image donCalienteImage;
    [SerializeField] private UnityEngine.UI.Image melonImage;
    [SerializeField] private GameObject eliminatedTomato;
    [SerializeField] private GameObject eliminatedCarrot;
    [SerializeField] private GameObject eliminatedCaliente;
    [SerializeField] private GameObject eliminatedMelone;

    [Header("Selection")]
    [SerializeField] private GameObject backButton;

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
    [SerializeField] private Vector3 selectionLocation;
    [SerializeField] private Vector3 captCarrotLocation;
    [SerializeField] private Vector3 sgtSplatLocation;
    [SerializeField] private Vector3 donCalienteLocation;
    [SerializeField] private Vector3 madameMeloneLocation;


    [Header("Audio")]
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource captCarrotMusic;
    [SerializeField] private AudioSource sgtSplatMusic;
    [SerializeField] private AudioSource donCalienteMusic;
    [SerializeField] private AudioSource cutsceneMusic;
    [SerializeField] private AudioSource[] announcerAudios;

    [Header("Characters")]
    [SerializeField] private GameObject spud;
    private SpudScript spudScript;
    [SerializeField] private GameObject captainCarrot;
    [SerializeField] private GameObject sgtSplat;
    [SerializeField] private GameObject donCaliente;
    [SerializeField] private GameObject madameMelone;

    //Scripts
    private CaptainCarrotScript captainCarrotScript;
    private SgtSplatScript sgtSplatScript;
    private DonCalienteScript donCalienteScript;
    private MadameMelon madameMelonScript;

    [SerializeField] private UnityEngine.UI.Slider bossHealth;


    private CutsceneScript cutsceneScript;
    //bools
    public bool fightingCarrot;
    public bool defeatedCarrot = false;
    public bool fightingTomato;
    public bool defeatedTomato = false;
    public bool fightingCaliente;
    public bool defeatedCaliente = false;
    public bool fightingMelone;
    public bool defeatedMelone = false;

    public bool gameWon = false;
    public bool cutScene1Played = false;
    public bool cutScene2Played = false;
    private bool canPlayBossMusic = false;



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
        donCalienteScript = donCaliente.GetComponent<DonCalienteScript>();
        madameMelonScript = madameMelone.GetComponent<MadameMelon>();

        cutsceneScript = FindAnyObjectByType<CutsceneScript>();

        tomatoImage.color = new Color(0 / 255f, 0 / 255f, 0 / 255f);
        carrotImage.color = new Color(0 / 255f, 0 / 255f, 0 / 255f);
        donCalienteImage.color = new Color(0 / 255f, 0 / 255f, 0 / 255f);
        eliminatedTomato.SetActive(false);
        eliminatedCarrot.SetActive(false);
        eliminatedCaliente.SetActive(false);


        //SetState(GameState.Cutscene); //default state
        SetState(GameState.MainMenu); ; //default state
        menuMusic.Play();

        sgtSplatMusic.Stop();
        cutsceneMusic.Stop();
        captCarrotMusic.Stop(); //stop the music
        donCalienteMusic.Stop();


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (CurrentState == GameState.Playing))
        {
            TogglePause();
        }
        if (CurrentState == GameState.GameOver)
        {
            spudScript.destroyBullet = true;
        }
        gameWon = defeatedCarrot && defeatedTomato && defeatedCaliente && defeatedMelone;
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
        mainMenuPanel.SetActive(false);
        pauseScreen.SetActive(false);
        settingsPanel.SetActive(false);
        firstCutScene.SetActive(false);
        endCutScene.SetActive(false);
        backButton.SetActive(false);



        switch (CurrentState)
        {
            case GameState.Cutscene:
                //Debug.Log("Trying to open the cutsceneCanvas");
                cutsceneCanvas.SetActive(true);
                PlayMusic(cutsceneMusic);

                if (!gameWon)
                {
                    firstCutScene.SetActive(true);
                }
                else
                {
                    endCutScene.SetActive(true);
                    //cutsceneScript.startEndScene();
                }

                break;
            case GameState.MainMenu:
                mainMenuCanvas.SetActive(true);
                mainMenuPanel.SetActive(true);
                camera.transform.position = captCarrotLocation;

                PlayMusic(menuMusic);

                break;
            case GameState.Selection:
                backButton.SetActive(true);

                camera.transform.position = selectionLocation;

                spudScript.goToSelectionMap();

                if (defeatedTomato)
                {
                    tomatoImage.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                    eliminatedTomato.SetActive(true);
                }
                if (defeatedCarrot)
                {
                    carrotImage.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                    eliminatedCarrot.SetActive(true);
                }
                if (defeatedCaliente)
                {
                    donCalienteImage.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                    eliminatedCaliente.SetActive(true);
                }
                if (defeatedMelone)
                {
                    melonImage.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                    eliminatedMelone.SetActive(true);
                }

                PlayMusic(menuMusic);

                break;
            case GameState.Settings:
                mainMenuCanvas.SetActive(true);
                settingsPanel.SetActive(true);

                PlayMusic(menuMusic);

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
                HealthBar();
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

    private void PlayMusic(AudioSource targetMusic)
    {

        if (targetMusic.isPlaying) return;

        // Stop all music first
        menuMusic.Stop();
        cutsceneMusic.Stop();
        captCarrotMusic.Stop();
        sgtSplatMusic.Stop();
        donCalienteMusic.Stop();

        targetMusic.Play();
    }

    IEnumerator PlayMusicRoutine(AudioSource targetMusic)
    {
        // Stop all music first
        menuMusic.Stop();
        cutsceneMusic.Stop();
        captCarrotMusic.Stop();
        sgtSplatMusic.Stop();
        donCalienteMusic.Stop();
        for (int i = 0; i < announcerAudios.Length; i++)
        {
            announcerAudios[i].Stop();
        }

        //pick a random announcement
            int targetIndex = UnityEngine.Random.Range(0, announcerAudios.Length);
        AudioSource target = announcerAudios[targetIndex];

        target.Play();

        //wait until the announcement is done playing
        while (target.isPlaying)
        {
            if (CurrentState != GameState.Playing)
            {
                target.Stop();
                yield break;
            }
            yield return null;
        }

        if (CurrentState == GameState.Playing)
        {
            //play the intended boss music 
            targetMusic.Play();
        }
        
    }

    public void StartGame()
    {
        //Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    private void HealthBar()
    {
        if (fightingCarrot)
        {
            float maxValue = captainCarrotScript.maxHealth;
            float currentHealth = captainCarrotScript.health;

            bossHealth.maxValue = maxValue;

            bossHealth.value = maxValue - currentHealth;
        }
        else if (fightingTomato)
        {
            float maxValue = sgtSplatScript.maxHealth;
            float currentHealth = sgtSplatScript.health;

            bossHealth.maxValue = maxValue;

            bossHealth.value = maxValue - currentHealth;
        }
        else if (fightingCaliente)
        {
            float maxValue = donCalienteScript.maxHealth;
            float currentHealth = donCalienteScript.health;

            bossHealth.maxValue = maxValue;

            bossHealth.value = maxValue - currentHealth;
        }
        else if (fightingMelone)
        {
            float maxValue = madameMelonScript.maxHealth;
            float currentHealth = madameMelonScript.health;

            bossHealth.maxValue = maxValue;
            bossHealth.value = maxValue - currentHealth;
        }
    }

    public void StartTutorial()
    {
        SetState(GameState.Tutorial);
    }

    public void MainMenu()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;

        if (gameWon && !cutScene2Played)
        {
            SetState(GameState.Cutscene);
        }
        else
        {
            SetState(GameState.MainMenu);
        }

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
        Time.timeScale = 1f;
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

        donCalienteScript.destroyBottle = true;
        donCalienteScript.destroyPelt = true;
        donCalienteScript.destroyRing = true;
        donCalienteScript.destroySeed = true;

        madameMelonScript.destroyHeart = true;
        madameMelonScript.destroyNote = true;

        sgtSplatScript.StopAnyCoroutines();
        captainCarrotScript.StopAnyCoroutines();
        donCalienteScript.StopAnyCoroutines();
        madameMelonScript.StopAllCoroutines();


        if (fightingCarrot)
        {
            captCarrotMusic.Stop();
            //captCarrotMusic.Play();
            FightCarrot();
        }
        else if (fightingTomato)
        {
            sgtSplatMusic.Stop();
            //sgtSplatMusic.Play();
            FightTomato();
        }
        else if (fightingCaliente)
        {
            donCalienteMusic.Stop();
            //donCalienteMusic.Play();
            FightCaliente();
        }
        else if (fightingMelone)
        {
            //stop Melone music
            //play Melone music
            FightMelone();

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
        StartGame();

        fightingCarrot = true;
        fightingTomato = false;
        fightingCaliente = false;
        fightingMelone = false;

        StartCoroutine(PlayMusicRoutine(captCarrotMusic));
        camera.transform.position = captCarrotLocation;

        captainCarrotScript.RestartCarrot();
        spudScript.Restart();
    }

    public void FightTomato()
    {
        StartGame();

        fightingTomato = true;
        fightingCarrot = false;
        fightingCaliente = false;
        fightingMelone = false;

        StartCoroutine(PlayMusicRoutine(sgtSplatMusic));
        camera.transform.position = sgtSplatLocation;

        sgtSplatScript.RestartTomato();
        spudScript.Restart();
    }

    public void FightCaliente()
    {
        StartGame(); 

        fightingTomato = false;
        fightingCarrot = false;
        fightingCaliente = true;
        fightingMelone = false;

        //play music
        // PlayMusic(donCalienteMusic);
        StartCoroutine(PlayMusicRoutine(donCalienteMusic));
        camera.transform.position = donCalienteLocation;

        donCalienteScript.RestartCaliente();
        spudScript.Restart();

        
    }

    public void FightMelone()
    {
        fightingTomato = false;
        fightingCarrot = false;
        fightingCaliente = false;
        fightingMelone = true;

        //play music
        camera.transform.position = madameMeloneLocation;

        madameMelonScript.restartMelone();
        spudScript.Restart();

        StartGame();
    }
    
}
