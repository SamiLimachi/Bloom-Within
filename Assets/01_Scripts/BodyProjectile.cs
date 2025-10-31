using UnityEngine;

public class BodyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;
    public float knockback = 3f;
    public LayerMask groundMask;

    void Start()
    {
        Debug.Log("Proyectil iniciado");
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Daño a jugador
        if (other.CompareTag("Player"))
        {
            var p = other.GetComponent<Player>();
            if (p != null) p.TakeDamage(damage);

            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
                rb.AddForce(dir * knockback, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
            return;
        }

        // Morir al tocar suelo/escena
        if (((1 << other.gameObject.layer) & groundMask) != 0)
        {
            Debug.Log("Toco el suelo");
            Destroy(gameObject);
        }
    }
}
