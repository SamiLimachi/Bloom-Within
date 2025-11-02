using System.Collections;
using UnityEngine;

public class TheMirrorPlayerBoss : Boss
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;
    public float jumpForce = 6f;
    public float chaseRange = 10f;
    private bool facingRight = true;
    public bool canJump = true;

    [Header("Componentes")]
    public Rigidbody2D rb;
    public Animator animator;
    private Transform player;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    private bool canAttack = true;
    public int attackDamage = 1;
    public GameObject attackSmokeEffect;
    public GameObject attackArea;
    public AudioClip attackSound;

    [Header("Dash")]
    public bool canDash = true;
    public float dashDistance = 3f;
    public float dashCooldown = 3f;
    public GameObject DashEffect;
    public AudioClip dashSound;

    [Header("Vida")]
    public AudioClip damageSound;
    private bool isDashing = false;
    private bool isAttacking = false;
    private bool isGrounded = false;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Puerta o evento al morir")]
    public GameObject Door;

    void Start()
    {
        Life = 150;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (healthBar != null)
            healthBar.SetMaxHealth(Life);
        if (bossUI != null)
            bossUI.SetActive(true);
    }

    void Update()
    {
        if (player == null || isAttacking || isDashing) return;

        CheckGrounded();
        ChasePlayer();
        TryAttackOrDash();
        UpdateAnimator();
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded)
            animator.SetBool("isJumping", false);
    }

    void ChasePlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < chaseRange && !isAttacking)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

            if (direction > 0 && !facingRight) Flip();
            else if (direction < 0 && facingRight) Flip();

            // ?? Si el jugador está más arriba, salta igual que el Player
            if (canJump && isGrounded && Mathf.Abs(player.position.y - transform.position.y) > 1.5f)
            {
                Jump();
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("isGrounded", false);
        StartCoroutine(ResetJump());
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(1f);
        canJump = true;
    }

    void TryAttackOrDash()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && canAttack && !isAttacking)
        {
            StartCoroutine(Attack());
        }
        else if (distance > 4f && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;
        animator.SetBool("isAttacking", true);

        if (attackSmokeEffect != null)
            Instantiate(attackSmokeEffect, transform.position, Quaternion.identity);
        if (attackSound != null)
            UIAudioManager.Instance.PlaySFX(attackSound, 1f);

        attackArea.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        attackArea.SetActive(false);

        isAttacking = false;
        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float dashDir = facingRight ? 1 : -1;
        if (DashEffect != null)
            Instantiate(DashEffect, transform.position, transform.rotation);

        Vector2 targetPos = new Vector2(transform.position.x + dashDistance * dashDir, transform.position.y);
        rb.position = targetPos;
        if (dashSound != null)
            UIAudioManager.Instance.PlaySFX(dashSound);

        yield return new WaitForSeconds(0.1f);

        if (DashEffect != null)
            Instantiate(DashEffect, transform.position, transform.rotation);

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void UpdateAnimator()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);

        bool isFalling = rb.velocity.y < -0.1f && !isGrounded;
        bool isJumpingNow = rb.velocity.y > 0.1f && !isGrounded;

        animator.SetBool("isJumping", isJumpingNow);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isAttacking", isAttacking);
    }

    protected override void Die()
    {
        Debug.Log("?? The Mirror Player Boss ha sido derrotado.");
        rb.velocity = Vector2.zero;
        isAttacking = false;
        isDashing = false;

        if (bossUI != null)
            bossUI.SetActive(false);

        if (Door != null)
            StartCoroutine(EnableDoorAfterDelay());
        else
            Destroy(gameObject, 2f);
    }

    IEnumerator EnableDoorAfterDelay()
    {
        yield return new WaitForSeconds(2.5f);
        Door.SetActive(true);
        base.Die();
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
