using System.Collections;
using UnityEngine;

public class VineScript : MonoBehaviour
{
    [SerializeField] private float growSpeed = 2f; //how fast the vine grows
    [SerializeField] private float shrinkSpeed = 3f; // How fast the vine shrinks
    [SerializeField] private float waitTime = 2f; // How long the vine waits before shrinking

    //references
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    //bools
    private bool isShrinking = false;
    private bool hasHitGround = false;
    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShrinking)
        {
            // Expand the vine’s size dynamically
            Vector2 newSize = spriteRenderer.size;
            newSize.y += growSpeed * Time.deltaTime;
            spriteRenderer.size = newSize;

            // Update the collider to match the new vine size
            if (boxCollider != null)
            {
                boxCollider.size = newSize;
            }
        }
        else
        {
            // Shrink the vine back up
            Vector2 newSize = spriteRenderer.size;
            newSize.y -= shrinkSpeed * Time.deltaTime;

            if (newSize.y <= 0)
            {
                newSize.y = 0; // Stop shrinking once fully gone
                isShrinking = false;
                hasHitGround = false; // Reset for the next cycle
            }

            spriteRenderer.size = newSize;
            if (boxCollider != null)
            {
                boxCollider.size = newSize;
            }
        }

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
        isShrinking = true;
    }
}
