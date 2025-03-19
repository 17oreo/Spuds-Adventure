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
    private bool phase1 = false;
    private bool phase2 = false;
    private bool phase3 = false;

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
        StartCoroutine(ShootCarrotRoutine());
    }

    void Update()
    {
        
    }
    public void RestartCarrot()
    {
        health = maxHealth;
        phase1 = true;
        phase2 = false;
        phase3 = false;
        destroyCarrot = false;  
        animator.SetBool("Phase2", false);
        StartCoroutine(ShootCarrotRoutine());
    }
    IEnumerator ShootCarrotRoutine()
    {
        phase1 = true;
        while (health > 800)
        {
            yield return new WaitForSeconds(shootInterval);
            ShootCarrot();

            if (spudScript.gameEnd)
            {
                yield break;
            }
        }
        destroyCarrot = true;
        animator.SetBool("Phase2", true);
        phase1 = false;

        StopCoroutine(ShootCarrotRoutine());
        
    }
    private void StartPhase2()
    {
        destroyCarrot = false;
        StartCoroutine(Phase2Routine());    
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
        while (health > 400)
        {
            StartCoroutine(RainCarrotRoutine());

            yield return new WaitForSeconds(1.2f);
            ShootCarrot();

            if (spudScript.gameEnd)
            {
                yield break;
            }
        }
        
       // phase3 = true;
        phase2 = false;
       // StartCoroutine(Phase3Routine());
        //SpawnAllVines();
        StopCoroutine(Phase2Routine());
        
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
            int num = Random.Range(1, 8); //random number fo 1 - 7
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

    void SpawnAllVines()
    {
        spawnedVines = new GameObject[vineSpawnPoints.Length];

        for (int i = 0; i < vineSpawnPoints.Length; i++)
        {
            spawnedVines[i] = Instantiate(vinePrefab, vineSpawnPoints[i].position, Quaternion.identity);
            spawnedVines[i].SetActive(true);
        }
    } 

    IEnumerator Phase3Routine()
    {

        while (health > 0)
        {
            // Drop a vine at a random spawn point
            StartCoroutine(ManageVineDrops());

            // Continue shooting carrots as well
            ShootCarrot();

            yield return new WaitForSeconds(1.2f);
        }

        // Phase 3 ends - Destroy boss and show win screen
        destroyCarrot = true;
        Destroy(gameObject);
        UI.WinScreen();
        
    }  
    
    IEnumerator ManageVineDrops()
    {
        while (phase3)
        {
            // Pick a random vine that is NOT currently dropping
            GameObject vineToDrop = spawnedVines[Random.Range(0, spawnedVines.Length)];
            VineScript vineScript = vineToDrop.GetComponent<VineScript>();

            if (vineScript != null && !vineScript.IsDropping())
            {
                vineScript.StartDropping();
                yield return new WaitUntil(() => vineScript.HasRetracted()); // Wait until the vine is back up
                yield return new WaitForSeconds(1f); // 1-second delay before the next one
            }
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
        int num = Random.Range(0, 5); //random number of 0, 1 or 2, 3, 4
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

        //only spawn in bottom level if in phase 2
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
