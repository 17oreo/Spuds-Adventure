using System.Collections;
using UnityEngine;

public class CaptainCarrotScript : MonoBehaviour
{
    [SerializeField] private GameObject carrotPrefab; 
    public Transform spawnPoint; //spawn point for the projectile
    public float shootInterval = 5f; //time before shots
    private SpudScript spudScript;
    private int numOfShots = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spudScript = FindAnyObjectByType<SpudScript>();
        StartCoroutine(ShootCarrotRoutine());
    }

    public void RestartCarrot()
    {
        numOfShots = 0;
        StartCoroutine(ShootCarrotRoutine());
    }
    IEnumerator ShootCarrotRoutine()
    {
        while (numOfShots <= 10)
        {
            yield return new WaitForSeconds(shootInterval);
            ShootCarrot();
            numOfShots++;

            if (spudScript.gameEnd)
            {
                yield break;
            }
        }
    }

    public void ShootCarrot()
    {   
        GameObject carrot = Instantiate(carrotPrefab, spawnPoint.position, Quaternion.Euler(0,180,90));

        carrot.GetComponent<Carrot>().Launch(); //call the launch function from the Carrot script
    }
}
