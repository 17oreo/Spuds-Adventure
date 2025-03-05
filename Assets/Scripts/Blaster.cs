using UnityEngine;

public class Blaster : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.13f;
    private float nextFireTime = 0f;
    private Transform spudTransform;

    void Start()
    {
        spudTransform = transform.parent;
    }

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

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        // Determine direction based on Spud's rotation (facing right = 0, facing left = 180)
        float direction = spudTransform.rotation.eulerAngles.y == 180 ? -1f : 1f;
        bulletScript.Launch(direction);
    }
}
