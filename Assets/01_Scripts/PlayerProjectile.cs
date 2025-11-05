using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damage = 2;
    public float lifetime = 3f;
    public bool isPlayerProjectile=false;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerProjectile)
        {
            if (other.CompareTag("Boss"))
            {
                Boss boss = other.GetComponent<Boss>();
                if (boss != null)
                    boss.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Boss"))
            {
                Boss e = other.GetComponent<Boss>();
                if (e != null)
                    e.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
        else if (!isPlayerProjectile)
        {
            if (other.CompareTag("Player"))
            {
                Player boss = other.GetComponent<Player>();
                if (boss != null)
                    boss.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
