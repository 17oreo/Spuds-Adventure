using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] float speed;
    private Rigidbody2D rb;
    private SpudScript spudScript;
    private MadameMelon madameMelonScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spudScript = FindAnyObjectByType<SpudScript>();
        madameMelonScript = FindAnyObjectByType<MadameMelon>();

        if (rb == null)
        {
            Debug.Log("RigidBody2D not found on shockwave");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.GameOver)
        {
            Destroy(gameObject);
        }
        if (madameMelonScript.destroyShockwave)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //ignore attack is spud is dashing
            if (spudScript.isInvincible || spudScript.invincibleAfterDamage)
            {
                return;
            }
            //Destroy(gameObject);
        }
    }

    public void Launch(bool toLeft)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        if (toLeft)
        {
            transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0); // Rotate MM right
            rb.linearVelocity = Vector2.left * speed;
        }
        else
        {
            transform.rotation = UnityEngine.Quaternion.Euler(0, 180, 0); // Rotate MM left
            rb.linearVelocity = Vector2.right * speed;
        }
    }
    
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
