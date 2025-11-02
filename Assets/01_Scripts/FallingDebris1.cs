using UnityEngine;

public class FallingDebris1 : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 4f;
    public float knockback = 5f;
    public LayerMask groundMask;
    public AudioClip impactClip;
    void Start()
    {
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
                Vector2 dir = (Vector2.down + (Vector2)(other.transform.position - transform.position).normalized * 0.2f);
                rb.AddForce(dir.normalized * knockback, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
            return;
        }

        // “Romperse” al tocar suelo/escena
        if (((1 << other.gameObject.layer) & groundMask) != 0)
        {
            UIAudioManager.Instance.PlaySFX(impactClip, 2f);
            Destroy(gameObject);
        }
    }
}
