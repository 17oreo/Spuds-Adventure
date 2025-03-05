using System.Collections;
using UnityEngine;

public class CaptainCarrotScript : MonoBehaviour
{
    [SerializeField] private GameObject carrotPrefab; 
    public Transform spawnPointMid; //mid spawn point for the projectile
    private Transform spawnPoint;
    [SerializeField] Transform spawnPointTop;
    [SerializeField] Transform spawnPointBottom;
    public float shootInterval = 5f; //time before shots
    private SpudScript spudScript;
    private int numOfShots = 0;
    private int maxHealth = 12000;
    [SerializeField] private int health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        spudScript = FindAnyObjectByType<SpudScript>();
        StartCoroutine(ShootCarrotRoutine());
    }

    public void RestartCarrot()
    {
        health = maxHealth;
        StartCoroutine(ShootCarrotRoutine());
    }
    IEnumerator ShootCarrotRoutine()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(shootInterval);
            ShootCarrot();
            numOfShots++;

            if (spudScript.gameEnd)
            {
                yield break;
            }
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            TakeDamage(bullet.GetDamage());
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void ShootCarrot()
    {   
        GameObject carrot1 = null;
        int num = Random.Range(0, 5); //random number of 0, 1 or 2, 3, 5
        if (num == 0 || num == 1) //bottom
        {
            spawnPoint = spawnPointBottom;
        }
        else if (num == 2) //mid
        {
            spawnPoint = spawnPointMid;
        }
        else //top
        {
            spawnPoint = spawnPointTop;
            //have top and mid enabled for crouching
            carrot1 = Instantiate(carrotPrefab, spawnPoint.position, Quaternion.Euler(0,180,90));
            spawnPoint = spawnPointMid;
        }
        
        GameObject carrot = Instantiate(carrotPrefab, spawnPoint.position, Quaternion.Euler(0,180,90));
        if (carrot1 != null)
        {
            carrot1.GetComponent<Carrot>().Launch();
        }
        carrot.GetComponent<Carrot>().Launch(); //call the launch function from the Carrot script
    }
}
