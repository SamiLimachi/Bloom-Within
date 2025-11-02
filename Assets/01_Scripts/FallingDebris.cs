using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDebris : MonoBehaviour
{
    [Header("Comportamiento del escombro")]
    public float lifeTime = 3f;      // Tiempo antes de desaparecer
    public int damage = 1;           // Daño al jugador
    public AudioClip impactSound;    // Sonido opcional
    public GameObject hitEffect;     // Efecto visual opcional

    private Rigidbody2D rb;
    private bool hasHit = false;

    void Start()
    {
        // ✅ Asegura visibilidad
        Vector3 fixedPos = transform.position;
        fixedPos.z = 0;
        transform.position = fixedPos;

        // ✅ Asegura capa visible
        gameObject.layer = LayerMask.NameToLayer("Default");

        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;
        hasHit = true;

        // 💢 Solo quita vida (sin reiniciar)
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(damage);
        }

        // 🌫️ Efecto visual
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        // 🎧 Sonido de impacto
        if (impactSound != null)
            UIAudioManager.Instance.PlaySFX(impactSound, 1.5f);

        Destroy(gameObject, 0.1f);
    }
}