using System.Collections;
using UnityEngine;

public class SgtSplatScript : MonoBehaviour
{
    //references
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private UI_Script UI;  
    [SerializeField] private GameObject ketchupPrefab;
    [SerializeField] private GameObject laserPrefab; 
    [SerializeField] private Transform ketchupSpawnPoint;
    [SerializeField] private Transform laserSpawnPoint;

    [SerializeField] private float ketchupShootInterval = 1.5f;

    //integers
    public int maxHealth = 1000;
    [SerializeField] public int health;

    //bools
    private bool phase1;
    private bool phase2;
    private bool spitAnimationComplete;
    private bool laserAnimationComplete;
    public bool destroyKetchup;
    public bool destroyLaser;

    //colors
    private Color damageColor = new Color(255f / 255f, 194f / 255f, 194f / 255f);
    private Color originalColor;

    private bool gameWon;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UI = FindAnyObjectByType<UI_Script>();
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
            destroyKetchup = true;
            destroyLaser = true;

            gameWon = true;
            GameManager.Instance.defeatedTomato = true;
        }
    }

    public void StopAnyCoroutines()
    {
        StopAllCoroutines();
    }

    public void RestartTomato()
    {
        health = maxHealth;
        phase1 = true;
        phase2 = false;

        destroyLaser = true;
        
        StartCoroutine(Phase1Routine());
    }

    IEnumerator Phase1Routine()
    {
        yield return new WaitForSeconds(1f);
        destroyKetchup = false;

        phase1 = true;
        while (phase1 && health > 500)
        {
            yield return new WaitForSeconds(ketchupShootInterval);
            spitAnimationComplete = false;
            animator.SetBool("Spit", true);

            while (!spitAnimationComplete)
            {
                yield return null;
            }
            //yield return new WaitForSeconds(.5f);

            animator.SetBool("Spit", false);

            ShootTomato();

            yield return null;
        }
        phase1 = false;
        phase2 = true;

        StartCoroutine(Phase2Routine());
        StopCoroutine(Phase1Routine());
    }

    IEnumerator Phase2Routine()
    {
        animator.SetBool("Phase1", false);
        animator.SetBool("Phase2", true);
        while (phase2 && health > 0)
        {
            yield return new WaitForSeconds(ketchupShootInterval);
            spitAnimationComplete = false;
            animator.SetBool("Spit", true);

            while (!spitAnimationComplete)
            {
                yield return null;
            }
            //yield return new WaitForSeconds(.5f);

            animator.SetBool("Spit", false);

            ShootTomato();

            yield return new WaitForSeconds(ketchupShootInterval);

            laserAnimationComplete = false;
            animator.SetBool("Laser", true);
            while (!laserAnimationComplete)
            {
                yield return null;
            }
            animator.SetBool("Laser", false);

            ShootLaser();
        }
    }

    public void ShootTomato()
    {
        GameObject ketchup1 = Instantiate(ketchupPrefab, ketchupSpawnPoint.position, Quaternion.Euler(0,0,0));
        ketchup1.GetComponent<Ketchup>().Launch();
    }

    public void SpitAnimationComplete()
    {
        spitAnimationComplete = true;
    }

    public void LaserAnimationComplete()
    {
        laserAnimationComplete = true;
    }

    public void ShootLaser()
    {
        GameObject laser1 = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.Euler(0,0,0));
        laser1.GetComponent<Laser>().Launch();
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

}
