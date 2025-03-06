using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using UnityEngine;

public class CaptainCarrotScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject carrotPrefab; 
    //Phase 1 Spawn points
    [Header("Phase 1 - Spawn Points")]
    [SerializeField] private Transform spawnPointMid; //mid spawn point for the projectile
    [SerializeField] private Transform spawnPointTop;
    [SerializeField] private Transform spawnPointBottom;

    [Header("Phase 2 - Spawn Points")]
    //Phase 2 Spawn points
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] private Transform spawnPoint3;
    [SerializeField] private Transform spawnPoint4;
    [SerializeField] private Transform spawnPoint5;
    [SerializeField] private Transform spawnPoint6;
    [SerializeField] private Transform spawnPoint7;


    private Transform spawnPoint;
    private Transform spawnPointRain;
    private SpudScript spudScript;
    private SpriteRenderer spriteRenderer;
    private UI_Script UI;

    //Floats
    public float shootInterval = 5f; //time before shots
    
    //Integers
    private int maxHealth = 1200;
    [SerializeField] private int health;

    //booleans
    public bool destroyCarrot = false;
    private bool phase1 = true;
    private bool phase2 = false;

    //colors
    private Color damageColor = new Color(255f / 255f, 194f / 255f, 194f / 255f);
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        spudScript = FindAnyObjectByType<SpudScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        StartCoroutine(ShootCarrotRoutine());
    }
    IEnumerator ShootCarrotRoutine()
    {
        while (health > 800)
        {
            yield return new WaitForSeconds(shootInterval);
            ShootCarrot();

            if (spudScript.gameEnd)
            {
                yield break;
            }
        }
        phase1 = false;
        destroyCarrot = true;
        yield return new WaitForSeconds(5f);
        phase2 = true;
        while (health > 400)
        {
            yield return new WaitForSeconds(.3f);
            RainCarrots();
            ShootCarrot();

            if (spudScript.gameEnd)
            {
                yield break;
            }
        }
        //StartCoroutine(Phase2Routine());
        Destroy(gameObject);
        UI.WinScreen();
    }

    private void RainCarrots()
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
        else //top
        {
            spawnPoint = spawnPointTop;
            //have top and mid enabled for crouching
            carrot1 = Instantiate(carrotPrefab, spawnPoint.position, Quaternion.Euler(0,180,90));
            spawnPoint = spawnPointMid;
        }

        //only spawn in bottom level if in phase 2
        if (phase2)
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
            carrot1.GetComponent<Carrot>().Launch();
            carrot.GetComponent<Carrot>().Launch();
        }

        carrot.GetComponent<Carrot>().Launch(); //call the launch function from the Carrot script
    }
}
