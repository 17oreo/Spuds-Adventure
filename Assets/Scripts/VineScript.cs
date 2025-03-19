using System.Collections;
using UnityEngine;

public class VineScript : MonoBehaviour
{
    [SerializeField] private float growSpeed = 2f; //how fast the vine grows
    [SerializeField] private float shrinkSpeed = 3f; // How fast the vine shrinks
    [SerializeField] private float waitTime = 2f; // How long the vine waits before shrinking
     [SerializeField] private float maxVineHeight = 5f;

    //references
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    //bools
    private bool isDropping = false;
    private bool hasHitGround = false;
    private bool hasRetracted = false;
    private float initialHeight;
    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Ensure sprite renderer is in Tiled mode to avoid distortion
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;

        initialHeight = spriteRenderer.size.y;

    }
    public void StartDropping()
    {
        if (!isDropping)
        {
            isDropping = true;
            hasRetracted = false;
            StartCoroutine(GrowVine());
        }
    }

    // Update is called once per frame
    IEnumerator GrowVine()
    {
        Vector2 newSize = spriteRenderer.size;

        while (newSize.y < maxVineHeight && !hasHitGround)
        {
            newSize.y += growSpeed * Time.deltaTime;
            spriteRenderer.size = newSize;

            if (boxCollider != null)
            {
                boxCollider.size = newSize;
            }

            yield return null;
        }

        StartCoroutine(ShrinkAfterDelay());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && !hasHitGround)
        {
            hasHitGround = true;
            StartCoroutine(ShrinkAfterDelay());
        }
    }

    // Coroutine to shrink the vine after waiting
    IEnumerator ShrinkAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(ShrinkVine());
    }

    IEnumerator ShrinkVine()
    {
        Vector2 newSize = spriteRenderer.size;

        while (newSize.y > initialHeight)
        {
            newSize.y -= shrinkSpeed * Time.deltaTime;
            spriteRenderer.size = newSize;

            if (boxCollider != null)
            {
                boxCollider.size = newSize;
            }

            yield return null;
        }

        isDropping = false;
        hasRetracted = true;
    }

    public bool IsDropping()
    {
        return isDropping;
    }

    public bool HasRetracted()
    {
        return hasRetracted;
    }
}
