using System;
using UnityEngine;

public class MusicNote : MonoBehaviour
{

    private float sinCenterY;
    private MadameMelon MMscript;
    private GoTowardPlayer movementScript;
    private SpriteRenderer spriteRenderer;

    [SerializeField] public bool inverted = false;
    [SerializeField] private float amplitude = 2;
    [SerializeField] private float frequency = .5f;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sinCenterY = transform.position.y;

        MMscript = FindAnyObjectByType<MadameMelon>();
        movementScript = GetComponent<GoTowardPlayer>();
        if (movementScript == null)
        {
            Debug.Log("Could not find the GoTowardPlayer Script");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MMscript.destroyNote)
        {
            Debug.Log("Destroy note is set to true...Destroying");
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Vector2 pos = transform.position;

        float sin = Mathf.Sin(pos.x * frequency) * amplitude;

        if (inverted)
        {
            sin *= -1;
        }

        pos.y = sinCenterY + sin;

        transform.position = pos;
    }

    public void MakeRed() //higher amplitude but slower
    {
        Debug.Log("Make Red Called");
        Debug.Log("check; Changing Amplitude");
        amplitude = 3f;
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<SpudScript>().isInvincible)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
