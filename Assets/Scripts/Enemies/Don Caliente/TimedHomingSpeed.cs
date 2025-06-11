using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using NUnit.Framework;

public class TimedHomingSeed : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float homingDuration = .6f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Transform player;
    private bool lockedOn = false;
    private float homingTimer;

    private GameObject fogPanel;
    private UnityEngine.UI.Image fogImage;

    private Coroutine fogCoroutine;

    private DonCalienteScript DCscript;
    private MadameMelon MMscript;

    public void Initialize(Transform target)
    {
        player = target;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DCscript = FindAnyObjectByType<DonCalienteScript>();
        MMscript = FindAnyObjectByType<MadameMelon>();
        homingTimer = homingDuration;

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (player != null)
        {
            moveDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = moveDirection * speed;
        }
    }

    void Update()
    {
        if (GameManager.Instance.fightingCaliente)
        {
            if (DCscript.destroySeed)
            {
                Debug.Log("Destroying Seed");
                Destroy(gameObject);
            }
        }
        else if (GameManager.Instance.fightingMelone)
        {
            if (MMscript.destroyHeart)
            {
                Debug.Log("Destroy Heart is set to true...Destroying");
                Destroy(gameObject);
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (lockedOn || player == null) return;

        homingTimer -= Time.fixedDeltaTime;

        if (homingTimer > 0f)
        {
            moveDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = moveDirection * speed;
        }
        else
        {
            lockedOn = true;
            rb.linearVelocity = moveDirection * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && GameManager.Instance.fightingCaliente )
        {
            Debug.Log("Collided with ground...Destroying");
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            //if Spud is not invincible then start the fog
            if (!collision.GetComponent<SpudScript>().isInvincible && GameManager.Instance.fightingCaliente)
            {
                FogController.Instance.TriggerFogEffect();
                Destroy(gameObject);
            }
            else if (!collision.GetComponent<SpudScript>().isInvincible)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetFogPanel(GameObject panel)
    {
        fogPanel = panel;
        fogImage = fogPanel.GetComponent<UnityEngine.UI.Image>();
    }



    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
