using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CaptainCarrotScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject carrotPrefab; 
    [SerializeField] private GameObject vinePrefab; // Assign the Vine prefab in the Inspector

    //Phase 1 Spawn points
    [Header("Phase 1 - Spawn Points")]
    [SerializeField] private Transform spawnPointMid; //mid spawn point for the projectile
    [SerializeField] private Transform spawnPointTop;
    [SerializeField] private Transform spawnPointBottom;

    [Header("Phase 2 - Transition Spawn Points")]
    [SerializeField] private Transform spawnPointTransition1;
    [SerializeField] private Transform spawnPointTransition2;

    [Header("Phase 2 - Spawn Points")]
    //Phase 2 Spawn points
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] private Transform spawnPoint3;
    [SerializeField] private Transform spawnPoint4;
    [SerializeField] private Transform spawnPoint5;
    [SerializeField] private Transform spawnPoint6;
    [SerializeField] private Transform spawnPoint7;

    [Header("Phase 3  - Spawn Points")]
    [SerializeField] private Transform[] vineSpawnPoints; // Assign 3 spawn points in the Inspector
    private GameObject[] spawnedVines;

    private Transform spawnPoint;
    private Transform spawnPointRain;
    private SpudScript spudScript;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private UI_Script UI;

    

    //Floats
    public float shootInterval = 5f; //time before shots
    
    //Integers
    private int maxHealth = 1200;
    [SerializeField] private int health;

    //booleans
    public bool destroyCarrot = false;
    public bool destroyVine = false;
    private bool phase1 = false;
    private bool phase2 = false;
    private bool phase3 = false;
    private String currentPhase;
    public bool paused = false;
    private bool gameStarted = false;

    //colors
    private Color damageColor = new Color(255f / 255f, 194f / 255f, 194f / 255f);
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        spudScript = FindAnyObjectByType<SpudScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UI = FindAnyObjectByType<UI_Script>();
        originalColor = spriteRenderer.color;

        gameStarted = false;
 
    }

    void Update()
    {
        // if ((GameManager.Instance.CurrentState != GameState.Playing) && !GameManager.Instance.IsGamePaused)
        // {
        //     StopAllCoroutines();
        // }
        if (GameManager.Instance.CurrentState == GameState.MainMenu)
        {
            StopAllCoroutines();
        }
    }
    public void RestartCarrot()
    {
        health = maxHealth;
        phase1 = true;
        phase2 = false;
        phase3 = false;
        //destroyCarrot = false;  
        gameStarted = true;
        destroyVine = true;
        animator.SetBool("Phase2", false);
        StopAllCoroutines();
        StartCoroutine(Phase1Routine());
    }

    public void StopAnyCoroutines()
    {
        StopAllCoroutines();
    }

    IEnumerator Phase1Routine()
    {
        yield return new WaitForSeconds(1f);
        destroyCarrot = false; 

        phase1 = true;
        currentPhase = "Phase1";
        while (phase1 && health > 800)
        {
            yield return new WaitForSeconds(shootInterval);
            ShootCarrot();

            if (GameManager.Instance.CurrentState == GameState.GameOver)
            {
                yield break;
            }
        }
        if (!paused)
        {
            destroyCarrot = true;
            animator.SetBool("Phase2", true);
            phase1 = false;

            StopCoroutine(Phase1Routine());
        }
        
        
    }
    private void StartPhase2()
    {
        destroyCarrot = false;
        StartCoroutine(Phase2Routine());    
    }
    

    public void Pause()
    {
        phase1 = false;
        phase2 = false;
        phase3 = false;
        paused = true;
    }

    public void Resume()
    {
        if (currentPhase.Equals("Phase1"))
        {
            phase1 = true;
        }
        else if (currentPhase.Equals("Phase2"))
        {
            phase2 = true;
        }
        else
        {
            phase3 = true;
        }
    }

    IEnumerator Phase2Routine()
    {
        Transform spawnPointT; 
        for (int i = 0; i < 8; i++)
        {
            if (i % 2 == 0)
            {
                spawnPointT = spawnPointTransition1;
            }
            else
            {
                spawnPointT = spawnPointTransition2;    
            }

            GameObject carrot = Instantiate(carrotPrefab, spawnPointT.position, Quaternion.Euler(0,180,180));
            carrot.GetComponent<Carrot>().ShootUp(); //call the launch function from the Carrot script
            yield return new WaitForSeconds(.15f);
        }
        
        
        phase2 = true;
        currentPhase = "Phase2";
        while ( phase2 && health > 400)
        {
            StartCoroutine(RainCarrotRoutine());

            yield return new WaitForSeconds(1.2f);
            ShootCarrot();

            if (GameManager.Instance.CurrentState == GameState.GameOver)
            {
                yield break;
            }
        }
        if (!paused)
        {
            phase3 = true;
            currentPhase = "Phase3";
            phase2 = false;
            StopCoroutine(Phase2Routine());
            StartPhase3();
        }
        

        
    }
    IEnumerator RainCarrotRoutine()
    {
        yield return new WaitForSeconds(.4f);
        RainCarrots();
    }

    private void RainCarrots()
    {
        for (int i = 0; i<2;i++)
        {
            int num = UnityEngine.Random.Range(1, 8); //random number fo 1 - 7
            if      (num == 1) spawnPointRain = spawnPoint1;
            else if (num == 2) spawnPointRain = spawnPoint2;
            else if (num == 3) spawnPointRain = spawnPoint3;
            else if (num == 4) spawnPointRain = spawnPoint4;
            else if (num == 5) spawnPointRain = spawnPoint5;
            else if (num == 6) spawnPointRain = spawnPoint6;
            else if (num == 7) spawnPointRain = spawnPoint7;

            GameObject carrot = Instantiate(carrotPrefab, spawnPointRain.position, Quaternion.Euler(0,180,0));
            carrot.GetComponent<Carrot>().Drop(); //call the launch function from the Carrot script
        }   
    }

    void StartPhase3()
    {
        destroyCarrot = false;
        destroyVine = false;

        // Step 1: Spawn all vines
        SpawnAllVines();

        // Step 2: Begin rotating drop pattern
        StartCoroutine(ManageVineDrops());

        // (Optional) Keep shooting carrots
        StartCoroutine(ShootCarrotRoutine());
    }

    IEnumerator ShootCarrotRoutine()
    {
        yield return new WaitForSeconds(2);
        while (health > 0 && phase3 && GameManager.Instance.CurrentState == GameState.Playing)
        {
            ShootCarrot();
            yield return new WaitForSeconds(1.2f); // Or tweak timing for intensity
        }
        if (paused)
        {
            yield return new WaitUntil(() => !paused);
        }
    }

    void SpawnAllVines()
    {
        spawnedVines = new GameObject[vineSpawnPoints.Length];

        for (int i = 0; i < vineSpawnPoints.Length; i++)
        {
            spawnedVines[i] = Instantiate(vinePrefab, vineSpawnPoints[i].position, Quaternion.identity);

            VineScript vineScript = spawnedVines[i].GetComponent<VineScript>();
            if (vineScript != null)
            {
                vineScript.Initialize(); //Important
            }
        }
    }

    IEnumerator ManageVineDrops()
    {

        while (health > 0 && phase3 && GameManager.Instance.CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(1.5f);

            // Pick two distinct random vine indices
            int index1 = UnityEngine.Random.Range(0, spawnedVines.Length);
            int index2;
            do
            {
                index2 = UnityEngine.Random.Range(0, spawnedVines.Length);
            } while (index2 == index1);

            VineScript vine1 = spawnedVines[index1].GetComponent<VineScript>();
            VineScript vine2 = spawnedVines[index2].GetComponent<VineScript>();

            // Start flashing both vines
            vine1.startFlash();
            vine2.startFlash();

            // Wait for both to finish flashing
            yield return new WaitUntil(() => vine1.doneFlashing && vine2.doneFlashing);

            // Start both if not already dropping
            if (!vine1.IsDropping())
            {
                vine1.StartDropping();
            }

            if (!vine2.IsDropping())
            {
                vine2.StartDropping();
            }

            // Wait until both have retracted
            yield return new WaitUntil(() => vine1.HasRetracted() && vine2.HasRetracted());
        }
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            StopCoroutine(ManageVineDrops());
        }
        else if (!paused)
        {
            // End of phase
            destroyCarrot = true;
            destroyVine = true;

            GameManager.Instance.SetState(GameState.Victory);
            GameManager.Instance.defeatedCarrot = true;
        }

        
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            TakeDamage(bullet.GetDamage());
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = damageColor;
        
        yield return new WaitForSeconds(.1f);

        spriteRenderer.color = originalColor;
    }

    public void ShootCarrot()
    {   
        GameObject carrot1 = null;
        int num = UnityEngine.Random.Range(0, 5); //random number of 0, 1 or 2, 3, 4
        if (num == 0 || num == 1) //bottom
        {
            spawnPoint = spawnPointBottom;
        }
        else if (num == 2) //mid
        {
            spawnPoint = spawnPointMid;
        }
        else if (phase1) //top
        {
            spawnPoint = spawnPointTop;
            //have top and mid enabled for crouching
            carrot1 = Instantiate(carrotPrefab, spawnPoint.position, Quaternion.Euler(0,180,90));
            spawnPoint = spawnPointMid;
        }

        //only spawn in bottom level if in phase 2 or 3
        if (!phase1)
        {
            spawnPoint = spawnPointBottom;
        }
        GameObject carrot = Instantiate(carrotPrefab, spawnPoint.position, Quaternion.Euler(0,180,90));

        if (carrot1 != null)
        {
            carrot1.GetComponent<Carrot>().Launch();
        }

        if (destroyCarrot)
        {
            if (carrot1 != null)
            {
                carrot1.GetComponent<Carrot>().Launch();
            }
           
            carrot.GetComponent<Carrot>().Launch();
        }

        carrot.GetComponent<Carrot>().Launch(); //call the launch function from the Carrot script
    }
}
