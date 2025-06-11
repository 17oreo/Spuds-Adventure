using System;
using System.Collections;
using System.Security;
using UnityEngine;

public class DonCalienteScript : MonoBehaviour
{
    //references
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private GameObject[] seedPrefabs;
    [SerializeField] private Transform seedSpawnPoint;
    [SerializeField] private GameObject fogPanel;
    [SerializeField] private float timeBetweenShots = 1.5f;
    [SerializeField] private GameObject hotSaucePrefab;
    [SerializeField] private Transform sauceLaunchPoint;
    [SerializeField] private Transform spud; // Reference to Spud's Transform
    [SerializeField] private GameObject ringPrefab;

    //floats 
    private float shotTimer;

    //integers
    public int maxHealth = 1600;
    [SerializeField] public int health;

    //bools
    private bool phase1; //1600 - 1300 hp (300hp)
    private bool phase2; //1300 - 800 hp  (500hp)
    private bool phase3; //800 - 0 hp (800hp)
    private bool gameWon;

    public bool destroySeed = false;
    public bool destroyBottle = false;
    public bool destroyRing = false;
    public bool destroyPelt = false;

    //colors
    private Color damageColor = new Color(255f / 255f, 194f / 255f, 194f / 255f);
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing )
        {
            StopAllCoroutines();
        }
        if (health <= 0 && !gameWon)
        {
            StopAllCoroutines();
            GameManager.Instance.SetState(GameState.Victory);

            // End of phase
            //set stuff to be destroyed
            destroySeed = true;
            destroyBottle = true;
            destroyRing = true;
            destroyPelt = true;

            gameWon = true;
            GameManager.Instance.defeatedCaliente = true;
        }
    }

    public void StopAnyCoroutines()
    {
        StopAllCoroutines();
    }

    public void RestartCaliente()
    {
        health = maxHealth;
        phase1 = true;
        phase2 = false;
        phase3 = false;

        gameWon = false;

        destroyBottle = true;
        destroyRing = true;
        destroyPelt = true;


        //set animations
        animator.SetBool("Phase 1", true);
        animator.SetBool("Phase 2", false);
        animator.SetBool("Phase 3", false);

        StartCoroutine(Phase1Routine());
    }

    IEnumerator Phase1Routine()
    {
        yield return new WaitForSeconds(1f);
        
        phase1 = true;
        destroySeed = false;
        while (phase1 && health > 1300)
        {
            yield return new WaitForSeconds(timeBetweenShots); //or some shoot interval
            FireSeed();
            //yield return null;
        } 
        phase1 = false;
        phase2 = true;

        animator.SetBool("Phase 1", false);
        animator.SetBool("Phase 2", true);

        //Start the phase2 routine
        StartCoroutine(Phase2Routine());
        StopCoroutine(Phase1Routine());  
    }

    private void FireSeed()
    {
        if (seedPrefabs.Length == 0 || seedSpawnPoint == null) return;

        int index = UnityEngine.Random.Range(0, seedPrefabs.Length);
        GameObject seed = Instantiate(seedPrefabs[index], seedSpawnPoint.position, Quaternion.identity);

        // Set fog panel on the newly spawned seed
        TimedHomingSeed seedScript = seed.GetComponent<TimedHomingSeed>();
        seedScript.SetFogPanel(fogPanel); // Pass fog panel
    }

    public void ThrowHotSauce()
    {
        // Spawn the bottle
        GameObject bottle = Instantiate(hotSaucePrefab, sauceLaunchPoint.position, Quaternion.identity);

        // Calculate lob force
        Vector2 dir = (spud.position - sauceLaunchPoint.position).normalized; //calculates a direction vector 
        float horizontalForce = dir.x * 5.4f; // tweak as needed
        float verticalForce = Mathf.Clamp(Vector2.Distance(spud.position, sauceLaunchPoint.position), 4f, 8f); //determines how high to 
                                                                                                                // launch the bottle

        // Launch it
        HotSauceProjectile projectile = bottle.GetComponent<HotSauceProjectile>();
        projectile.Launch(new Vector2(horizontalForce, verticalForce));
    }


    IEnumerator Phase2Routine()
    {
        yield return new WaitForSeconds(1f);
        destroyBottle = false;
        destroyPelt = false;

        while (phase2 && health > 800)
        {

            ThrowHotSauce();

            yield return new WaitForSeconds(.5f);

            FireSeed();

            yield return new WaitForSeconds(3f); //or some shoot interval
        }
        phase2 = false;
        phase3 = true;

        animator.SetBool("Phase 2", false);
        animator.SetBool("Phase 3", true);

        StartCoroutine(Phase3Routine());
        StopCoroutine(Phase2Routine()); 
    }

    IEnumerator Phase3Routine()
    {
        yield return new WaitForSeconds(1f);
        destroyRing = false;

        while (phase3 && health > 0)
        {

            ShootRing();
            yield return new WaitForSeconds(.6f);
            ShootRing();

            yield return new WaitForSeconds(1f); //or some shoot interval

            int randomNum = UnityEngine.Random.Range(0, 2); //random number of 0 or 1

            Debug.Log("Random Number is: " + randomNum);

            if (randomNum == 0)
            {
                FireSeed();
            }
            else
            {
                ThrowHotSauce();
            }

            yield return new WaitForSeconds(2f);

        }
        phase3 = false;
    }

    private void ShootRing()
    {
        GameObject ring = Instantiate(ringPrefab, seedSpawnPoint.position, Quaternion.identity);
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        //check if was hit by a bullet
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
}
