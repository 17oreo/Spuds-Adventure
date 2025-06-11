using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

public class Blaster : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject superBulletPrefab;
    [SerializeField] private float fireRate = 0.13f;
    private float nextFireTime = 0f;
    private float nextSuperTime = 0f;
    [SerializeField] private float superRate = 30f; //30 seconds before each super
    public bool shotSuper = false;
    private Transform spudTransform;
    public bool shootUp = false;

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

    public void TrySuper()
    {
        shootSuper();
    }

    public void shootBullet()
    {
        Vector2 shootDir;
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        float facingDir = spudTransform.rotation.eulerAngles.y == 180 ? -1f : 1f;
        bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (isMoving)
            {
                shootDir = new Vector2(facingDir, 1f).normalized; //45 degrees diagonal
            }
            else
            {
                shootDir = Vector2.up; //straight up
            }
            
        }
        else
        {
            shootDir = new Vector2(facingDir, 0f);
        }

        bulletScript.TestLaunch(shootDir);
    }

    public void shootSuper()
    {
        GameObject super = Instantiate(superBulletPrefab, spawnPoint.position, Quaternion.identity);

        Bullet bulletScript = super.GetComponent<Bullet>();

        //Determine direction based on Spud's rotation (facing right = 0, facing left = 180)
        float direction = spudTransform.rotation.eulerAngles.y == 180 ? -1f : 1f;
        bulletScript.Launch(direction);
    }
}
