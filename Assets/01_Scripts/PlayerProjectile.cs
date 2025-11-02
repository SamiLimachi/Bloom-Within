using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damage = 2;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
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
}
