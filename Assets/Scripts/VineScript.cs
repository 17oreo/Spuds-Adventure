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

    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set draw mode to Tiled so it stretches properly
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;

        // Set initial sizes
        spriteRenderer.size = new Vector2(spriteWidth, initialHeight);
        boxCollider.size = new Vector2(colliderWidth, initialHeight);

        // Center the collider
        // float xOffset = (spriteWidth - colliderWidth) / 2f;
        boxCollider.offset = new Vector2(colliderOffsetX, -20);

        isDropping = false;
        hasHitGround = false;
        hasRetracted = false;
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

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ground") && !hasHitGround)
    //     {
    //         hasHitGround = true;
    //         StopAllCoroutines(); // prevent double shrink
    //         StartCoroutine(ShrinkAfterDelay());
    //     }
    // }

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
