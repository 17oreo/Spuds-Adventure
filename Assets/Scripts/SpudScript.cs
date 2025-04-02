using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpudScript : MonoBehaviour
{

    //References
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Transform spawnPoint;
    private CaptainCarrotScript captainCarrotScript;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private Coroutine iframeCoroutine; // Store reference to the coroutine


    //integers
    private int Maxlives = 3;
    public int currentLives;

    //Colors
    private Color damageColor = new Color(255f / 255f, 142f / 255f, 142f / 255f);
    private Color IframesColor = new Color(134f / 255f, 134f / 255f, 134f / 255f);
    private Color originalColor;

    //bools 
    public bool isInvincible; 
    public bool invincibleAfterDamage = false;
    private bool buttonPressed;
    public bool gameEnd; 

    

    void Start()
    {
        currentLives = Maxlives;
        restartButton.onClick.AddListener(() => buttonPressed = true);
        captainCarrotScript = FindAnyObjectByType<CaptainCarrotScript>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        isInvincible = playerMovement.isInvincible;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if ( (other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Vine")) && !isInvincible)
        {
            TakeDamage(1);
        }
        
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (isInvincible) return;
            TakeDamage(1);
        }   
    }
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = damageColor;
        
        yield return new WaitForSeconds(.2f);

        spriteRenderer.color = originalColor;
    }

    private void TakeDamage(int damage)
    {
        if (isInvincible || invincibleAfterDamage) return;

        currentLives -= damage;
        livesText.text = "Health: " + currentLives;

        // Stop the previous IFrames coroutine if it's still running
        if (iframeCoroutine != null)
        {
            StopCoroutine(iframeCoroutine);
        }

        iframeCoroutine = StartCoroutine(IFrames());
        StartCoroutine(FlashRed());
        if (currentLives <= 0)
        {
            Die();
            gameEnd = true;
        }
    }

    IEnumerator IFrames()
    {
        
        invincibleAfterDamage = true;
        float elapsed = 0f;
        float flashInterval = 0.25f;

        while (elapsed < 2f)
        {
            spriteRenderer.color = IframesColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2f; // Because each flash is two waits
        }

        invincibleAfterDamage = false;
        spriteRenderer.color = originalColor;
        iframeCoroutine = null; // Reset reference after completio
    }

    private void Die()
    {
        restartPanel.SetActive(true);
        StartCoroutine(restartRoutine());
    }

    private IEnumerator restartRoutine()
    {
        while (!buttonPressed)
        {
            yield return null;
        }
        transform.position = spawnPoint.position;

        transform.rotation = Quaternion.Euler(0, 0, 0); // Rotate Spud right

        buttonPressed = false;
        currentLives = Maxlives;
        livesText.text = "Health: " + currentLives;
        gameEnd = false;
        restartPanel.SetActive(false);
        captainCarrotScript.RestartCarrot();

    }

}
