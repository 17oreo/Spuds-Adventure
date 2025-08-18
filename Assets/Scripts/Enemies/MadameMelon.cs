using System;
using System.Collections;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;

public class MadameMelon : MonoBehaviour
{

    //references
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    //prefabs and spawns
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartSpawn;
    [SerializeField] private GameObject musicNotePrefab;
    [SerializeField] private Transform musicNoteSpawn;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private Transform seedSpawnPoint;
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private Transform shockwaveRSpawnPoint;
    [SerializeField] private Transform shockwaveLSpawnPoint;
    
    [SerializeField] private Transform MMSpawnPoint;
    [SerializeField] private Transform MMLeftTarget;

    [SerializeField] private Transform spud; // Reference to Spud's Transform
    //integers
    public int maxHealth = 1800;
    [SerializeField] public int health;

    //bools
    private bool phase1;
    private bool phase2;
    private bool phase3;
    private bool gameWon;
    private bool jumpToLeft;

    public bool destroyHeart;
    public bool destroyNote;
    public bool destroySeed;
    public bool destroyShockwave;

    public bool checkForCollision = false;
    private bool inAir;


    //colors
    private Color damageColor = new Color(234f / 255f, 234f / 255f, 234f / 255f);
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;

        gameObject.transform.position = MMSpawnPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            StopAllCoroutines();
            gameObject.transform.position = MMSpawnPoint.position;
        }
        if (health <= 0 && !gameWon)
        {
            StopAllCoroutines();
            GameManager.Instance.SetState(GameState.Victory);

            // End of phase
            //set stuff to be destroyed
            destroyHeart = true;
            destroyNote = true;

            gameWon = true;
            GameManager.Instance.defeatedMelone = true;
        }
    }

    IEnumerator Wink()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            int index = UnityEngine.Random.Range(0, 5); //random 5 numbers 

            if (index == 0)
            {
                animator.SetBool("Wink", true);
                yield return new WaitForSeconds(.2f);
                animator.SetBool("Wink", false);
            }
        }
    }

    public void StopAnyCoroutines()
    {
        StopAllCoroutines();
    }

    public void restartMelone()
    {
        health = maxHealth;
        phase1 = true;
        phase2 = false;
        phase3 = false;

        //phase 1 bools
        destroyHeart = true;
        destroyNote = true;

        //phase 2 bools
        checkForCollision = false;
        inAir = false;
        destroySeed = true;
        destroyShockwave = true;

        gameWon = false;

        gameObject.transform.position = MMSpawnPoint.transform.position;

        animator.SetBool("Phase 1", true);
        animator.SetBool("Phase 2", false);
        animator.SetBool("Phase 3", false);

        StartCoroutine(Phase1Routine());
        StartCoroutine(Wink());
    }

    IEnumerator Phase1Routine()
    {
        destroyHeart = false;
        destroyNote = false;
        yield return new WaitForSeconds(1f);

        phase1 = true;
        destroySeed = false; //move to phase 2 when done testing
        destroyShockwave = false; //move to phase 2 when done testing
        while (phase1 && health > 1200)
        {
            //Calculate the distance between the MM and the left side as well as between MM and the right side
            float distanceToLeft = UnityEngine.Vector2.Distance(gameObject.transform.position, MMLeftTarget.position);
            float distanceToRight = UnityEngine.Vector2.Distance(gameObject.transform.position, MMSpawnPoint.position);

            //if the toTheLeft distance is greater -> jump to the left
            if (distanceToLeft > distanceToRight)
            {
                jumpToLeft = true;
            }
            else
            {
                jumpToLeft = false;
            }

            if (!inAir)
            {
                Flip(!jumpToLeft);
                JumpToSide(jumpToLeft);
                checkForCollision = true;
            }

            yield return new WaitForSeconds(5f);

            // yield return new WaitForSeconds(1f); //or some shoot interval

            // animator.SetBool("Kiss", true);

            // yield return new WaitForSeconds(1f);

            // FireHeart();

            // animator.SetBool("Kiss", false);

            // yield return new WaitForSeconds(1f);

            // animator.SetBool("Sing", true);

            // yield return new WaitForSeconds(1f);

            // Sing();

            // yield return new WaitForSeconds(.3f);

            // Sing();

            // yield return new WaitForSeconds(.3f);

            // Sing();

            // animator.SetBool("Sing", false);

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


    IEnumerator Phase2Routine()
    {
        yield return new WaitForSeconds(1f);

        while (phase2 && health > 800)
        {

        }
        phase2 = false;
        phase3 = true;

        animator.SetBool("Phase 2", false);
        animator.SetBool("Phase 3", true);

        StartCoroutine(Phase3Routine());
        StopCoroutine(Phase2Routine());
    }

    public void JumpToSide(bool toLeft)
    {
        if (toLeft) //jump to the left
        {
            UnityEngine.Vector2 dir = (MMLeftTarget.position - gameObject.transform.position).normalized; //calculates a direction vector 
            float horizontalForce = dir.x * 5f; // tweak as needed
            float verticalForce = Mathf.Clamp(UnityEngine.Vector2.Distance(spud.position, gameObject.transform.position), 12f, 12f);

            UnityEngine.Vector2 force = new UnityEngine.Vector2(horizontalForce, verticalForce);


            rb.AddForceY(2.5f, ForceMode2D.Impulse);
            rb.AddForce(force, ForceMode2D.Impulse);

            inAir = true;
        }
        else
        {
            UnityEngine.Vector2 dir = (MMSpawnPoint.position - gameObject.transform.position).normalized; //calculates a direction vector 
            float horizontalForce = dir.x * 5f; // tweak as needed
            float verticalForce = Mathf.Clamp(UnityEngine.Vector2.Distance(spud.position, gameObject.transform.position), 12f, 12f);

            UnityEngine.Vector2 force = new UnityEngine.Vector2(horizontalForce, verticalForce);


            rb.AddForceY(2.5f, ForceMode2D.Impulse);
            rb.AddForce(force, ForceMode2D.Impulse);

            inAir = true;
        }
        StartCoroutine(dropSeeds());
    }

    IEnumerator dropSeeds()
    {
        yield return new WaitForSeconds(.5f);
        while (inAir)
        {
            GameObject seed = Instantiate(seedPrefab, seedSpawnPoint.position, UnityEngine.Quaternion.identity);
            yield return new WaitForSeconds(.6f);
        }
    }

    private void Flip(bool facingLeft)
    {
        if (facingLeft)
        {
            transform.rotation = UnityEngine.Quaternion.Euler(0, 180, 0); // Rotate MM left
        }
        else
        {
            transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0); // Rotate MM right
        }
    }


    IEnumerator Phase3Routine()
    {
        yield return new WaitForSeconds(1f);

        while (phase3 && health > 0)
        {

        }
        phase3 = false;
    }


    private void FireHeart()
    {
        Debug.Log("Spawning in heart");
        GameObject heart = Instantiate(heartPrefab, heartSpawn.position, UnityEngine.Quaternion.identity);
    }

    private void Sing()
    {
        GameObject musicNote = Instantiate(musicNotePrefab, musicNoteSpawn.position, UnityEngine.Quaternion.identity);
        MusicNote script = musicNote.GetComponent<MusicNote>();
        int num = UnityEngine.Random.Range(0, 2);
        if (num == 1)
        {
            Debug.Log("trying to make red note");
            SpriteRenderer spriteRenderer = musicNote.GetComponent<SpriteRenderer>();
            Debug.Log("Trying to switch color");
            spriteRenderer.color = new Color(255f / 255f, 19f / 255f, 0f / 255f); //change the color to red
            Debug.Log("Check! increasing speed");
            GoTowardPlayer goToward = musicNote.GetComponent<GoTowardPlayer>();
            Debug.Log("Check!");
            goToward.speed = 7f;
            script.MakeRed();
        }
        else
        {
            Debug.Log("Keeping the note blue");
        }
        int numToInvert = UnityEngine.Random.Range(0, 2);
        if (numToInvert == 1)
        {
            script.inverted = true;
        }

    }

    private void LaunchShockwave()
    {
        if (jumpToLeft)//if it is on the left
        {
            GameObject shockwave = Instantiate(shockwavePrefab, shockwaveRSpawnPoint.position, UnityEngine.Quaternion.identity); //spawn the shockwave on the right of MM
            Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();
            shockwaveScript.Launch(false); //Get the shockwave script and call the Launch function and have it shoot to the right
        }
        else
        {
            GameObject shockwave = Instantiate(shockwavePrefab, shockwaveLSpawnPoint.position, UnityEngine.Quaternion.identity); //spawn the shockwave on the left of MM
            Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();
            shockwaveScript.Launch(true); //Get the shockwave script and call the Launch function and have it shoot to the left
        }
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && checkForCollision)
        {
            StartCoroutine(bounceOff());
        }
        if (collision.gameObject.CompareTag("Ground") && GameManager.Instance.fightingMelone)
        {
            inAir = false;
            LaunchShockwave();
            // Flip(!jumpToLeft);
        }
    }

    IEnumerator bounceOff()
    {
        yield return new WaitForSeconds(.3f);
        JumpToSide(!jumpToLeft);
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
