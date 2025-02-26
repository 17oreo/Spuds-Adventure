using System.Collections;
using UnityEngine;

public class CaptainCarrot : MonoBehaviour
{
    [SerializeField] private GameObject carrotPrefab; 
    public Transform spawnPoint; //spawn point for the projectile
    public float shootInterval = 5f; //time before shots

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShootCarrotRoutine());
    }

    IEnumerator ShootCarrotRoutine()
    {
        int numOfShots = 0;

        while (numOfShots <= 10)
        {
            yield return new WaitForSeconds(shootInterval);
            shootCarrot();
            numOfShots++;
        }
    }

    public void shootCarrot()
    {   
        GameObject carrot = Instantiate(carrotPrefab, spawnPoint.position, Quaternion.Euler(0,90,0));

        carrot.GetComponent<Carrot>().launch(); //call the launch function from the Carrot script
    }
}
