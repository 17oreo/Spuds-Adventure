using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class Spud : MonoBehaviour
{
   private int Maxlives = 3;
   public int currentLives;

    void Start()
    {
        currentLives = Maxlives;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            TakeDamage(1);
        }
    }

    private void TakeDamage(int damage)
    {
        currentLives -= damage;
        if (currentLives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Spud is defeated");
        Destroy(gameObject);
    }

}
