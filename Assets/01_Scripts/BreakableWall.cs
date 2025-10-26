using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("Configuración de resistencia")]
    public int hitsToBreak = 4;
    private int currentHits = 0;

    [Header("Efectos visuales y sonoros")]
    public GameObject hitDustEffect;
    public GameObject breakEffect;
    public AudioClip hitSound;
    public AudioClip breakSound;

    [Header("Escombros al romperse")]
    public GameObject debrisPrefab;
    public int debrisCount = 3;

    private SpriteRenderer sr;
    private Collider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void TakeHit()
    {
        currentHits++;
        Debug.Log($"🧱 Golpe a pared: {currentHits}/{hitsToBreak}");

        // 🌫️ Polvo de golpe
        if (hitDustEffect != null)
        {
            GameObject dust = Instantiate(hitDustEffect, transform.position, Quaternion.identity);
            var ps = dust.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(dust, 1.5f);
        }

        // 🎧 Sonido de golpe
        if (hitSound != null)
            UIAudioManager.Instance.PlaySFX(hitSound, 0.5f);

        if (currentHits >= hitsToBreak)
            BreakWall();
    }

    void BreakWall()
    {
        Debug.Log("💥 ¡Pared destruida!");

        // 🌫️ Polvo final
        if (breakEffect != null)
        {
            GameObject dust = Instantiate(breakEffect, transform.position, Quaternion.identity);
            var ps = dust.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(dust, 2f);
        }

        // 🎧 Sonido fuerte
        if (breakSound != null)
            UIAudioManager.Instance.PlaySFX(breakSound, 1f);

        // 🪨 Instancia escombros físicos
        if (debrisPrefab != null)
        {
            for (int i = 0; i < debrisCount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.6f, 0.6f), 0.6f, 0);
                GameObject debris = Instantiate(debrisPrefab, spawnPos, Quaternion.identity);

                // Fuerza Z visible y capa Default
                spawnPos.z = 0;
                debris.transform.position = spawnPos;
                debris.layer = LayerMask.NameToLayer("Default");
            }
        }

        // ✅ Empuja ligeramente al jugador si está encima
        Collider2D playerCol = Physics2D.OverlapCircle(transform.position, 0.6f, LayerMask.GetMask("Player"));
        if (playerCol != null)
        {
            Rigidbody2D rb = playerCol.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.position += new Vector2(0, 0.25f);
        }

        StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.15f);
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;
        Destroy(gameObject, 0.3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}