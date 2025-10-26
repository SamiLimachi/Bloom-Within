using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpitter : MonoBehaviour
{
    [Header("Parámetros del enemigo")]
    public int life = 3;
    public float fireRate = 2f;       // tiempo entre disparos
    public float fireSpeed = 6f;      // velocidad del fuego
    public Transform firePoint;       // punto de salida del fuego
    public GameObject firePrefab;     // prefab del fuego

    [Header("Orientación y detección")]
    public bool facingRight = true;
    public float detectionRange = 6f;
    private Transform player;

    private bool canShoot = true;
    private SpriteRenderer sr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        // Determina dirección hacia el jugador
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            FlipTowardsPlayer();

            if (canShoot)
                StartCoroutine(Shoot());
        }
    }

    void FlipTowardsPlayer()
    {
        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        // 📍 Dirección hacia el jugador
        Vector2 dir = (player.position - firePoint.position).normalized;

        // 🔥 Crear el proyectil
        GameObject fire = Instantiate(firePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fire.GetComponent<Rigidbody2D>();
        rb.velocity = dir * fireSpeed;

        // ❗ Ignorar colisión con el propio enemigo
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D fireCollider = fire.GetComponent<Collider2D>();
        if (myCollider != null && fireCollider != null)
            Physics2D.IgnoreCollision(fireCollider, myCollider, true);

        // 🔄 Gira el sprite del fuego según dirección
        Vector3 fireScale = fire.transform.localScale;
        fireScale.x = (dir.x >= 0) ? Mathf.Abs(fireScale.x) : -Mathf.Abs(fireScale.x);
        fire.transform.localScale = fireScale;

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }


    public void TakeDamage(int dmg)
    {
        life -= dmg;
        if (life <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("🔥 Enemigo de fuego destruido");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}