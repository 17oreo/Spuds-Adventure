using System;
using System.Collections;
using UnityEngine;

public class MadameMelon : MonoBehaviour
{

    //references
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartSpawn;
    [SerializeField] private GameObject musicNotePrefab;
    [SerializeField] private Transform musicNoteSpawn;


    //integers
    public int maxHealth = 1800;
    [SerializeField] public int health;

    //bools
    private bool phase1;
    private bool phase2;
    private bool phase3;
    private bool gameWon;

    public bool destroyHeart;
    public bool destroyNote;


    //colors
    private Color damageColor = new Color(234f / 255f, 234f / 255f, 234f / 255f);
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
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            StopAllCoroutines();
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

        destroyHeart = true;
        destroyNote = true;

        gameWon = false;

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
        while (phase1 && health > 1200)
        {
            yield return new WaitForSeconds(1f); //or some shoot interval

            animator.SetBool("Kiss", true);

            yield return new WaitForSeconds(1f);

            FireHeart();

            animator.SetBool("Kiss", false);

            yield return new WaitForSeconds(1f);

            animator.SetBool("Sing", true);

            yield return new WaitForSeconds(1f);

            Sing();

            yield return new WaitForSeconds(.3f);

            Sing();

            yield return new WaitForSeconds(.3f);

            Sing();

            animator.SetBool("Sing", false);

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
        GameObject heart = Instantiate(heartPrefab, heartSpawn.position, Quaternion.identity);
    }

    private void Sing()
    {
        GameObject musicNote = Instantiate(musicNotePrefab, musicNoteSpawn.position, Quaternion.identity);
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
