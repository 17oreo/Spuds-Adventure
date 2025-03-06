using System.Collections;
using TMPro;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpudScript : MonoBehaviour
{
   private int Maxlives = 3;
   public int currentLives;
   [SerializeField] private TextMeshProUGUI livesText;
   [SerializeField] private GameObject restartPanel;
   [SerializeField] private Button restartButton;
   [SerializeField] private Transform spawnPoint;
   private bool buttonPressed;
   public bool gameEnd; 
   private CaptainCarrotScript captainCarrotScript;
   private PlayerMovement playerMovement;
   private SpriteRenderer spriteRenderer;
   private Color damageColor = new Color(255f / 255f, 142f / 255f, 142f / 255f);
   private Color originalColor;
   public bool isInvincible; 

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
        if (other.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(1);
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
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
        //do not take damage if spud is dashing
        if (isInvincible)
        {
            return;
        }
        currentLives -= damage;
        livesText.text = "Health: " + currentLives;
        StartCoroutine(IFrames());
        StartCoroutine(FlashRed());
        if (currentLives <= 0)
        {
            Die();
            gameEnd = true;
        }
    }

    IEnumerator IFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(2);
        isInvincible = false;
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
        spriteRenderer.flipX = false;

        buttonPressed = false;
        currentLives = Maxlives;
        livesText.text = "Health: " + currentLives;
        gameEnd = false;
        restartPanel.SetActive(false);
        captainCarrotScript.RestartCarrot();

    }

}
