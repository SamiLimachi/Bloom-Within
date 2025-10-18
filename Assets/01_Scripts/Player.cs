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

    [Header("Vida y Checkpoint")]
    public int life = 5;
    public bool BossFight = false;
    public Transform startPoint;

    [Header("Dash / Teletransporte")]
    public float dashDistance = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;
    private float lastDirection = 1f;
    public GameObject DashEffect;

    [Header("Escalera")]
    public bool isClimbing = false;
    private float verticalInput;
    private float originalGravity;

    [Header("Ataque")]
    public bool isAttacking = false;
    public float attackDuration = 0.5f;   // Duración del ataque
    public GameObject attackSmokeEffect;  // Prefab del efecto de humo (opcional)

    [Header("Animación")]
    public Animator animator;

    void Start()
    {
        originalGravity = rb.gravityScale;
        canJump = true; // 🔥 asegura que empiece en el suelo
        animator.SetBool("isGrounded", true);
    }

    void Update()
    {
        // Evitar movimiento mientras hace Dash o Attack
        if (isDashing || isAttacking) return;

        if (isClimbing)
        {
            Climb();
        }
        else
        {
            Movement();
            Jump();
        }

        // Dash (E)
        if (Input.GetKeyDown(KeyCode.E) && canDash && !isClimbing)
        {
            StartCoroutine(Dash());
        }

        // Ataque (Click Izquierdo o tecla J)
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)) && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        UpdateAnimatorParameters();
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");

        // Correr
        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runningSpeed : moveSpeed;

        rb.velocity = new Vector2(x * currentSpeed, rb.velocity.y);

        // Girar sprite según dirección
        if (x > 0.1f)
        {
            lastDirection = 1f;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (x < -0.1f)
        {
            lastDirection = -1f;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void Jump()
    {
        // 🔥 sin trigger — solo cambia flags
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
            animator.SetBool("isJumping", true);
            animator.SetBool("isGrounded", false);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        // Instancia efecto de humo si existe
        if (attackSmokeEffect != null)
        {
            Instantiate(attackSmokeEffect, transform.position, Quaternion.identity);
        }

        // Espera la duración del ataque
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void Climb()
    {
        verticalInput = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(0, verticalInput * moveSpeed / 1.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = false;
            animator.SetBool("isGrounded", false);
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
        if (life <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (!BossFight)
        {
            transform.position = startPoint.position;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float dashDirection = lastDirection;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        Instantiate(DashEffect, transform.position, transform.rotation);

        // Detección de obstáculos
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * dashDirection, dashDistance, LayerMask.GetMask("Ground"));

        Vector2 targetPos;
        if (hit.collider != null)
        {
            targetPos = hit.point - (Vector2.right * dashDirection * 0.2f);
        }
        else
        {
            targetPos = new Vector2(transform.position.x + dashDistance * dashDirection, transform.position.y);
        }

        rb.position = targetPos;

        yield return new WaitForSeconds(0.1f);
        Instantiate(DashEffect, transform.position, transform.rotation);
        if (sr != null) sr.enabled = true;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        isDashing = false;
    }

    // 🔥 Actualización completa del Animator
    void UpdateAnimatorParameters()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));   // Movimiento horizontal
        animator.SetFloat("yVelocity", rb.velocity.y);          // Velocidad vertical

        bool isFalling = rb.velocity.y < -0.1f && !canJump;
        bool isJumpingNow = rb.velocity.y > 0.1f && !canJump;

        animator.SetBool("isJumping", isJumpingNow);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isGrounded", canJump);
        animator.SetBool("isClimbing", isClimbing);
        animator.SetBool("isAttacking", isAttacking);
    }
}