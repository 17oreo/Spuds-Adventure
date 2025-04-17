using System.Collections;
using UnityEngine;

public class SgtSplatScript : MonoBehaviour
{
    //references
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private UI_Script UI;  
    [SerializeField] private GameObject ketchupPrefab;
    [SerializeField] private GameObject laserPrefab; 
    [SerializeField] private Transform ketchupSpawnPoin;
    [SerializeField] private Transform laserSpawnPoint;

    //integers
    private int maxHealth = 1200;
    [SerializeField] private int health;

    //bools
    private bool phase1;
    private bool phase2;
    private bool phase3;
    private bool animationComplete;
    public bool destroyKetchup;

    //colors
    private Color damageColor = new Color(255f / 255f, 194f / 255f, 194f / 255f);
    private Color originalColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UI = FindAnyObjectByType<UI_Script>();
        originalColor = spriteRenderer.color;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartTomato()
    {
        health = maxHealth;
        phase1 = true;
        phase2 = false;
        phase3 = false;

        StartCoroutine(Phase1Routine());
    }

    IEnumerator Phase1Routine()
    {
        phase1 = true;
        while (phase1 && health > 800)
        {
            yield return new WaitForSeconds(2f);
            animationComplete = false;
            animator.SetBool("Spit", true);

            while (!animationComplete)
            {
                yield return null;
            }
            //yield return new WaitForSeconds(.5f);

            animator.SetBool("Spit", false);

            ShootTomato();

            yield return null;
        }
    }

    public void ShootTomato()
    {
        GameObject ketchup1 = Instantiate(ketchupPrefab, ketchupSpawnPoin.position, Quaternion.Euler(0,0,0));
        ketchup1.GetComponent<Ketchup>().Launch();
    }

    public void SpitAnimationComplete()
    {
        animationComplete = true;
    }
}
