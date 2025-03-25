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
            // spriteRenderer = GetComponent<SpriteRenderer>();
            // boxCollider = GetComponent<BoxCollider2D>();

            // // Ensure sprite renderer is in Tiled mode to avoid distortion
            // spriteRenderer.drawMode = SpriteDrawMode.Tiled;

            // initialHeight = spriteRenderer.size.y;
        }

        public void Initialize()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();

            if (spriteRenderer != null)
            {
                spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                initialHeight = spriteRenderer.size.y;
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, initialHeight); // Reset size
            }

            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(boxCollider.size.x, initialHeight); // Reset collider
            }

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

        // Update is called once per frame
        IEnumerator GrowVine()
        {

            Debug.Log("GrowVine coroutine running...");

            Vector2 newSize = spriteRenderer.size;
            newSize.y += growSpeed * Time.deltaTime;
            spriteRenderer.size = newSize;

            while (newSize.y < maxVineHeight && !hasHitGround)
            {
                newSize.y += growSpeed * Time.deltaTime;
                spriteRenderer.size = newSize;

                if (boxCollider != null)
                {
                    boxCollider.size = newSize;
                    //boxCollider.offset = new Vector2(0f, -newSize.y / 2f); // shifts the collider down with the sprite
                    boxCollider.offset = Vector2.zero;
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
            newSize.y -= shrinkSpeed * Time.deltaTime;
                spriteRenderer.size = newSize;

            while (newSize.y > initialHeight)
            {
                newSize.y -= shrinkSpeed * Time.deltaTime;
                spriteRenderer.size = newSize;

                if (boxCollider != null)
                {
                    boxCollider.size = newSize;
                    //boxCollider.offset = new Vector2(0f, -newSize.y / 2f);
                    boxCollider.offset = Vector2.zero;
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
