using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    [Header("Patrulla")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    [Header("Persecución")]
    public float chaseRange = 5f;
    public int contactDamage = 1;

    [Header("Vida")]
    public int maxHealth = 5;
    private int currentHealth;

    private Transform target;
    private Transform player;
    private bool chasing = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        target = pointB;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            chasing = true;
            MoveTowards(player.position);
        }
        else
        {
            chasing = false;
            MoveTowards(target.position);

            if (Vector2.Distance(transform.position, target.position) < 0.1f)
                target = target == pointA ? pointB : pointA;
        }

        // Girar sprite según dirección
        if (chasing)
        {
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            if (target.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void MoveTowards(Vector2 position)
    {
        transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log($"🩸 Bat recibió daño: -{dmg}. HP restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("☠️ Bat destruido");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player p = collision.collider.GetComponent<Player>();
            if (p != null)
                p.TakeDamage(contactDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar ataque cuerpo a cuerpo del jugador
        PlayerAttackArea atk = collision.GetComponent<PlayerAttackArea>();
        if (atk != null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
                TakeDamage(player.attackDamage);
        }

        // Detectar proyectiles del jugador
        if (collision.CompareTag("Projectile"))
        {
            PlayerProjectile p = collision.GetComponent<PlayerProjectile>();
            if (p != null)
                TakeDamage(p.damage);
            Destroy(collision.gameObject);
        }
    }
}