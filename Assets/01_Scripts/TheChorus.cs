using System.Collections;
using UnityEngine;

public class TheChorus : Boss
{
    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float jumpForce = 8f;
    public float chaseRange = 10f;
    public Rigidbody2D rb;
    private Transform player;
    private bool canAttack = true;
    private bool canMove = true;
    private bool facingRight = true;

    [Header("Daño por contacto")]
    public int contactDamage = 1;
    public float knockbackForce = 6f;
    private bool canDealContactDamage = true;
    public float contactCooldown = 1.5f;

    [Header("Ataques")]
    public GameObject bodyProjectilePrefab;
    public Transform shootPoint;
    public GameObject debrisPrefab;
    public GameObject roarWavePrefab;
    public GameObject impactEffect; // opcional: polvo/efecto de impacto

    public float timeBetweenAttacks = 2f;

    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (healthBar != null)
            healthBar.SetMaxHealth(Life);

        StartCoroutine(AttackPattern());
    }

    void FixedUpdate()
    {
        if (Life <= 0 || !canMove) return;
        MovementLogic();
    }

    void MovementLogic()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < chaseRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);

            if (dir.x > 0 && !facingRight) Flip();
            else if (dir.x < 0 && facingRight) Flip();
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    IEnumerator AttackPattern()
    {
        while (Life > 0)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);
            if (!canAttack) continue;

            canAttack = false;
            canMove = false;

            int randomAttack = Random.Range(0, 5); // 👈 ahora hay 5 ataques (0–4)

            switch (randomAttack)
            {
                case 0:
                    yield return StartCoroutine(JumpAttack());
                    break;
                case 1:
                    yield return StartCoroutine(BodyProjectileAttack());
                    break;
                case 2:
                    yield return StartCoroutine(DebrisAttack());
                    break;
                case 3:
                    yield return StartCoroutine(RoarAttack());
                    break;
                case 4:
                    yield return StartCoroutine(LeapSmashAttack()); // ⚡ nuevo ataque
                    break;
            }

            canMove = true;
            canAttack = true;
        }
    }

    // -------------------------
    // NUEVO ATAQUE: LeapSmash
    // -------------------------
    IEnumerator LeapSmashAttack()
    {
        Debug.Log("💢 Leap Smash Attack!");

        // 1️⃣ Pequeña carga antes del salto
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);

        // 2️⃣ Salta directamente hacia el jugador
        Vector2 targetDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        Vector2 jumpVector = targetDir + Vector2.up * 1.2f; // más vertical para que no caiga plano
        rb.velocity = jumpVector * jumpForce * 1.2f; // salto más fuerte que el normal

        // 3️⃣ Esperar a que toque el suelo
        yield return new WaitUntil(() => isGrounded);

        // 4️⃣ Al caer → genera impacto / daño en área
        Debug.Log("💥 Impacto del salto");
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (roarWavePrefab != null)
            Instantiate(roarWavePrefab, transform.position, Quaternion.identity);

        // daño cercano opcional
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Player p = hit.GetComponent<Player>();
                if (p != null) p.TakeDamage(1);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator JumpAttack()
    {
        yield return new WaitForSeconds(0.2f);
        Vector2 jumpDir = ((Vector2)player.position - (Vector2)transform.position).normalized + Vector2.up * 0.5f;
        rb.velocity = jumpDir * jumpForce;
        yield return new WaitForSeconds(1f);
    }

    IEnumerator BodyProjectileAttack()
    {
        yield return new WaitForSeconds(0.3f);

        int count = 4;
        for (int i = 0; i < count; i++)
        {
            GameObject proj = Instantiate(bodyProjectilePrefab, shootPoint.position, Quaternion.identity);
            Vector2 dir = (player.position - shootPoint.position).normalized;
            Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
            if (rbProj != null)
                rbProj.velocity = dir * 8f;
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator DebrisAttack()
    {
        rb.velocity = Vector2.up * (jumpForce * 1.5f);
        yield return new WaitForSeconds(0.7f);

        int debrisCount = 6;
        for (int i = 0; i < debrisCount; i++)
        {
            float randX = Random.Range(-8f, 8f);
            Vector2 spawnPos = new Vector2(transform.position.x + randX, transform.position.y + 10f);
            Instantiate(debrisPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator RoarAttack()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(roarWavePrefab, transform.position, Quaternion.identity);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canDealContactDamage)
            StartCoroutine(ContactDamage(collision.gameObject));
    }

    IEnumerator ContactDamage(GameObject playerObj)
    {
        canDealContactDamage = false;
        Player p = playerObj.GetComponent<Player>();
        if (p != null)
        {
            p.TakeDamage(contactDamage);
            Vector2 dir = (p.transform.position - transform.position).normalized;
            Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
            if (prb != null)
                prb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(contactCooldown);
        canDealContactDamage = true;
    }
    protected override void Die()
    {
        AudioSource bossAudio = GetComponent<AudioSource>();
        if (bossAudio != null)
        {
            bossAudio.Stop();
        }

    }
}
