using System.Collections;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [Header("Comportamiento del rayo")]
    public float fallSpeed = 15f;   // velocidad de caída
    public float lifeTime = 1.5f;   // cuánto dura en pantalla

    private bool hasImpacted = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.down * fallSpeed; // movimiento descendente

        Destroy(gameObject, lifeTime); // autodestrucción de seguridad
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasImpacted) return;
        hasImpacted = true;

        // Daña al jugador si lo toca
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(1);
        }

        // Efecto de impacto visual
            Destroy(gameObject);

    }

    IEnumerator DestroyAfterFlash()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
