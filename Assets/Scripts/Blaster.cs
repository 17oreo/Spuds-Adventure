using UnityEngine;

public class Blaster : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.13f;
    private float nextFireTime = 0f;

    
    public void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            shootBullet();
            nextFireTime = Time.time + fireRate; //set next fire time
        }
    }
    public void shootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Launch();

    }
}
