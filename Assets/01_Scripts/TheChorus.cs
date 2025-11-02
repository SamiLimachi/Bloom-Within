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
    public GameObject impactEffect;

    public float timeBetweenAttacks = 2f;

    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Animación")]
    public Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (healthBar != null)
            healthBar.SetMaxHealth(Life);

        if (animator == null)
            animator = GetComponent<Animator>();

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

            animator.SetBool("isMoving", true);

            if (dir.x > 0 && !facingRight) Flip();
            else if (dir.x < 0 && facingRight) Flip();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isMoving", false);
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

            int randomAttack = Random.Range(0, 5);

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
                    yield return StartCoroutine(LeapSmashAttack());
                    break;
            }

            canMove = true;
            canAttack = true;
        }
    }

    IEnumerator LeapSmashAttack()
    {
        animator.SetTrigger("LeapSmash");

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);

        Vector2 targetDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        Vector2 jumpVector = targetDir + Vector2.up * 1.2f;
        rb.velocity = jumpVector * jumpForce * 1.2f;

        yield return new WaitUntil(() => isGrounded);

        if (impactEffect) Instantiate(impactEffect, transform.position, Quaternion.identity);
        if (roarWavePrefab) Instantiate(roarWavePrefab, transform.position, Quaternion.identity);

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
        animator.SetTrigger("Jump");

        yield return new WaitForSeconds(0.2f);
        Vector2 jumpDir = ((Vector2)player.position - (Vector2)transform.position).normalized + Vector2.up * 0.5f;
        rb.velocity = jumpDir * jumpForce;
        yield return new WaitForSeconds(1f);
    }

    IEnumerator BodyProjectileAttack()
    {
        animator.SetTrigger("Shoot");
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
        animator.SetTrigger("Debris");

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
        animator.SetTrigger("Roar");

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
        canMove = false;
        canAttack = false;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");

        AudioSource bossAudio = GetComponent<AudioSource>();
        if (bossAudio != null) bossAudio.Stop();
    }
}