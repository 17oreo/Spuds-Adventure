using System.Collections;
using UnityEngine;

public class DropThroughPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    public float dropDuration = 0.5f;
    private bool isDropping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Z)) // Z = jump
        {
            StartCoroutine(DropThrough());
        }
    }

    private IEnumerator DropThrough()
    {
        isDropping = true;

        // Temporarily disable collision with "Platform" layer
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"), true);

        yield return new WaitForSeconds(dropDuration);

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"), false);
        isDropping = false;
    }
}
