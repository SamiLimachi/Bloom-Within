using UnityEngine;

public class RoarWave : MonoBehaviour
{
    public float expandSpeed = 6f;
    public float maxRadius = 5f;
    public float stunDuration = 1f;
    public float knockback = 4f;

    private CircleCollider2D col;
    private float currentRadius = 0.1f;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = currentRadius;
    }

    void Update()
    {
        currentRadius = Mathf.MoveTowards(currentRadius, maxRadius, expandSpeed * Time.deltaTime);
        col.radius = currentRadius;

        // Escalar visualmente el sprite para que coincida
        float s = currentRadius * 2f;
        transform.localScale = new Vector3(s, s, 1f);

        if (Mathf.Approximately(currentRadius, maxRadius))
            Destroy(gameObject, 0.05f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Intenta stun
        var stun = other.GetComponent<StunReceiver>();
        if (stun != null)
        {
            stun.ApplyStun(stunDuration);
        }
        else
        {
            // Si no hay StunReceiver, al menos empuja
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
                rb.AddForce(dir * knockback, ForceMode2D.Impulse);
            }
        }
    }
}
