using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movimiento Básico")]
    public float moveSpeed = 6f;
    public float jumpForce = 6f;
    public float runningSpeed = 10f;
    public Rigidbody2D rb;
    public bool isRunning = false;
    float currentSpeed;
    public bool canJump = true;
    public AudioClip walkingSound;
    public AudioClip jumpingSound;
    private float footstepTimer = 0f;
    public float footstepInterval = 0.4f;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Vida y Checkpoint")]
    public int life = 5;
    public bool BossFight = false;
    public Transform startPoint;

    [Header("Dash / Teletransporte")]
    public bool isDashUnlocked = false;
    public float dashDistance = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;
    private float lastDirection = 1f;
    public GameObject DashEffect;
    public AudioClip dashSound;

    [Header("Escalera")]
    public bool isClimbing = false;
    private float verticalInput;
    private float originalGravity;
    private float climbStepTimer = 0f;
    public float climbStepInterval = 0.45f;

    [Header("Ataque Cuerpo a Cuerpo")]
    public bool isAttacking = false;
    public float attackDuration = 0.5f;
    public GameObject attackSmokeEffect;
    public AudioClip attackSound;
    public AudioClip damageSound;
    public GameObject attackArea;
    public int attackDamage = 5;

    [Header("Ataque a Distancia")]
    public bool isShootUnlocked = false; // 🔓 Desbloqueable como el Dash
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 12f;
    public float shootCooldown = 0.8f;
    private bool canShoot = true;
    public AudioClip shootSound;

    [Header("Animación")]
    public Animator animator;

    public int hitsEnemy = 0;

    void Start()
    {
        originalGravity = rb.gravityScale;
        canJump = true;
    }

    void Update()
    {
        if (isDashing || isAttacking) return;

        CheckGrounded();

        if (isClimbing)
            Climb();
        else
        {
            Movement();
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E) && canDash && !isClimbing && isDashUnlocked)
            StartCoroutine(Dash());

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)) && !isAttacking)
            StartCoroutine(Attack());

        // 🔫 Disparo desbloqueable con tecla K
        if (isShootUnlocked && Input.GetKeyDown(KeyCode.K) && canShoot)
            StartCoroutine(ShootProjectile());

        UpdateAnimatorParameters();
    }

    public void Heal()
    {
        hitsEnemy++;
        if (hitsEnemy >= 5)
        {
            if (life < 5)
            {
                life++;
                hitsEnemy = 0;
            }
        }
    }

    void CheckGrounded()
    {
        canJump = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        animator.SetBool("isGrounded", canJump);
        if (canJump)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }

    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runningSpeed : moveSpeed;

        rb.velocity = new Vector2(x * currentSpeed, rb.velocity.y);

        if (x > 0.1f)
        {
            lastDirection = 1f;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (x < -0.1f)
        {
            lastDirection = -1f;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        bool isMovingByInput = Mathf.Abs(x) > 0.1f;
        if (isMovingByInput && canJump && !isClimbing)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                UIAudioManager.Instance.PlaySFX(walkingSound, 0.3f);
                footstepTimer = isRunning ? footstepInterval * 0.7f : footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    void Jump()
    {
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
            animator.SetBool("isJumping", true);
            animator.SetBool("isGrounded", false);
            UIAudioManager.Instance.PlaySFX(jumpingSound);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        if (attackSmokeEffect != null)
            Instantiate(attackSmokeEffect, transform.position, Quaternion.identity);
        UIAudioManager.Instance.PlaySFX(attackSound, 1f);
        attackArea.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackArea.SetActive(false);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    // 🔫 Nuevo ataque a distancia (solo si está desbloqueado)
    IEnumerator ShootProjectile()
    {
        canShoot = false;
        if (shootSound != null)
            UIAudioManager.Instance.PlaySFX(shootSound, 1f);

        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
            if (prb != null)
                prb.velocity = new Vector2(projectileSpeed * lastDirection, 0f);

            // invertir sprite si mira a la izquierda
            Vector3 scale = projectile.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * lastDirection;
            projectile.transform.localScale = scale;
        }

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    void Climb()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(0, verticalInput * moveSpeed / 1.5f);

        bool isClimbingByInput = Mathf.Abs(verticalInput) > 0.1f;
        if (isClimbingByInput)
        {
            climbStepTimer -= Time.deltaTime;
            if (climbStepTimer <= 0f)
            {
                UIAudioManager.Instance.PlaySFX(walkingSound, 0.3f);
                climbStepTimer = climbStepInterval;
            }
        }
        else
        {
            climbStepTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = originalGravity;
        }
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        UIAudioManager.Instance.PlaySFX(damageSound, 1f);

        if (life > 0)
        {
            if (!BossFight)
            {
                transform.position = startPoint.position;
            }
            Debug.Log($"💔 Jugador herido. Vida actual: {life}");
            StartCoroutine(FlashDamage());
            return;
        }

        Debug.Log("☠️ Jugador ha muerto.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator FlashDamage()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = Color.white;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            sr.color = originalColor;
        }
    }

    // 🩹 Tu método Heal1 conservado exactamente igual
    public IEnumerator Heal1()
    {
        if (life < 5)
        {
            life++;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color originalColor = Color.white;
                sr.color = Color.black;
                yield return new WaitForSeconds(0.5f);
                sr.color = originalColor;
            }
        }
    }

    private IEnumerator ForceGroundDetection()
    {
        yield return new WaitForFixedUpdate();
        CheckGrounded();
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float dashDirection = lastDirection;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        Instantiate(DashEffect, transform.position, transform.rotation);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * dashDirection, dashDistance, LayerMask.GetMask("Ground"));
        Vector2 targetPos = hit.collider != null
            ? hit.point - (Vector2.right * dashDirection * 0.2f)
            : new Vector2(transform.position.x + dashDistance * dashDirection, transform.position.y);

        rb.position = targetPos;
        UIAudioManager.Instance.PlaySFX(dashSound);

        yield return new WaitForSeconds(0.1f);

        Instantiate(DashEffect, transform.position, transform.rotation);
        if (sr != null) sr.enabled = true;

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void UpdateAnimatorParameters()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);

        bool isFalling = rb.velocity.y < -0.1f && !canJump;
        bool isJumpingNow = rb.velocity.y > 0.1f && !canJump;

        animator.SetBool("isJumping", isJumpingNow);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isClimbing", isClimbing);
        animator.SetBool("isAttacking", isAttacking);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        if (shootPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(shootPoint.position, shootPoint.position + Vector3.right * 0.5f);
        }
    }

    public void EnterDoor()
    {
        Destroy(gameObject);
    }
}
