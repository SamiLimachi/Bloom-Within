using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [Header("Movimiento y Salto")]
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float detectionRange = 6f;
    public float jumpCooldown = 2f;
    private bool canJump = true;
    private bool facingRight = true;

    [Header("Vida")]
    public int maxHealth = 10;
    private int currentHealth;
    public GameObject deathEffect;

    [Header("Daño al jugador")]
    public int contactDamage = 1;
    public float damageCooldown = 1f;
    private bool canDamage = true;

    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Detectar al jugador y saltar hacia él
        if (distance <= detectionRange && canJump)
            StartCoroutine(JumpTowardsPlayer());

        // Voltear sprite según dirección
        if (player.position.x > transform.position.x && !facingRight)
            Flip();
        else if (player.position.x < transform.position.x && facingRight)
            Flip();
    }

    IEnumerator JumpTowardsPlayer()
    {
        canJump = false;

        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 jumpVelocity = new Vector2(direction.x * moveSpeed, jumpForce);
        rb.velocity = jumpVelocity;

        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ---------------------- VIDA ----------------------
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"💢 Slime recibió daño: -{amount}. HP restante: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Debug.Log("☠️ Slime destruido correctamente por daño del jugador");
        Destroy(gameObject);
    }

    // ---------------------- DAÑO AL JUGADOR ----------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Solo daña al jugador al contacto
        if (collision.collider.CompareTag("Player") && canDamage)
        {
            Player playerScript = collision.collider.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(contactDamage);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    // ---------------------- RECIBIR ATAQUES ----------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ✅ Solo recibe daño de las áreas de ataque o proyectiles
        if (collision.GetComponent<PlayerAttackArea>() != null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
                TakeDamage(player.attackDamage);
        }

        if (collision.CompareTag("Projectile"))
        {
            PlayerProjectile p = collision.GetComponent<PlayerProjectile>();
            if (p != null)
                TakeDamage(p.damage);

            Destroy(collision.gameObject);
        }
    }

    // ---------------------- DEBUG VISUAL ----------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}