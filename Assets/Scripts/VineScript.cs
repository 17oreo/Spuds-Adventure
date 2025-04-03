using System.ComponentModel;
using System.Collections;
using UnityEngine;

public class VineScript : MonoBehaviour
{
    [SerializeField] private float growSpeed = 2f;
    [SerializeField] private float shrinkSpeed = 3f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float maxVineHeight = 5f;
    [SerializeField] private float initialHeight = 0.2f;

    [SerializeField] private float spriteWidth = 10f;
    [SerializeField] private float colliderWidth = 5.42f;
    [SerializeField] private float colliderOffsetX = 0;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private bool isDropping = false;
    private bool hasHitGround = false;
    private bool hasRetracted = false;
    public bool doneFlashing = false;

    private Color originalColor;
    private Color flashColor = new Color(190f / 255f, 190f / 255f, 190f / 255f);
    private CaptainCarrotScript captainCarrotScript;

    void Awake()
    {
        captainCarrotScript = FindAnyObjectByType<CaptainCarrotScript>();
    }

    void Update()
    {
        if (captainCarrotScript.destroyVine)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set draw mode to Tiled so it stretches properly
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;

        // Set initial sizes
        spriteRenderer.size = new Vector2(spriteWidth, initialHeight);
        boxCollider.size = new Vector2(colliderWidth, initialHeight);


        boxCollider.offset = new Vector2(colliderOffsetX, -initialHeight / 2f);

        originalColor = spriteRenderer.color;

        isDropping = false;
        hasHitGround = false;
        hasRetracted = false;
    }

    public void destroyVines()
    {
        Destroy(gameObject);
    }

    public void StartDropping()
    {
        if (!isDropping)
        {
            Debug.Log("StartDropping started");
            isDropping = true;
            hasRetracted = false;
            StartCoroutine(GrowVine());
        }
    }

    IEnumerator GrowVine()
    {
        Debug.Log("GrowVine running");

        Vector2 newSize = spriteRenderer.size;

        while (newSize.y < maxVineHeight && !hasHitGround)
        {
            newSize.y += growSpeed * Time.deltaTime;
            spriteRenderer.size = newSize;

            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(colliderWidth, newSize.y);
                // float xOffset = (spriteWidth - colliderWidth) / 2f;
                // boxCollider.offset = new Vector2(xOffset, -newSize.y / 2f);
                boxCollider.offset = new Vector2(colliderOffsetX,  -newSize.y / 2f);
            }

            yield return null;
        }

        if (!hasHitGround)
        {
            StartCoroutine(ShrinkAfterDelay());
        }
    }

    IEnumerator Flash()
    {
        float elapsed = 0f;
        while (elapsed < 2f)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }
        doneFlashing = true;

    }

    public void startFlash()
    {
        doneFlashing = false;
        StartCoroutine(Flash());
    }

    IEnumerator ShrinkAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(ShrinkVine());
    }

    IEnumerator ShrinkVine()
    {
        Vector2 newSize = spriteRenderer.size;

        while (newSize.y >= initialHeight)
        {
            newSize.y -= shrinkSpeed * Time.deltaTime;
            spriteRenderer.size = newSize;

            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(colliderWidth, newSize.y);
                // float xOffset = (spriteWidth - colliderWidth) / 2f;
                // boxCollider.offset = new Vector2(xOffset, -newSize.y / 2f);
                boxCollider.offset = new Vector2(colliderOffsetX, -newSize.y / 2f); 
            }

            yield return null;
        }

        isDropping = false;
        hasRetracted = true;
    }

    public bool IsDropping() => isDropping;
    public bool HasRetracted() => hasRetracted;
}
