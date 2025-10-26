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
    public float footstepInterval = 0.4f; // tiempo entre sonidos de pasos

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Vida y Checkpoint")]
    public int life = 5;
    public bool BossFight = false;
    public Transform startPoint;

    [Header("Dash / Teletransporte")]
    public bool isDashUnlocked=false;
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
    public float climbStepInterval = 0.45f; // tiempo entre sonidos al trepar

    [Header("Ataque")]
    public bool isAttacking = false;
    public float attackDuration = 0.5f;
    public GameObject attackSmokeEffect;
    
    
    public GameObject attackArea;/// ATAQUE 
    public int attackDamage = 10;


    [Header("Animación")]
    public Animator animator;


   

    void Start()
    {
        originalGravity = rb.gravityScale;
        canJump = true;
    }

    void Update()
    {
        if (isDashing || isAttacking) return;

        CheckGrounded(); // 🔥 Actualiza canJump cada frame

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

        UpdateAnimatorParameters();
    }

    void CheckGrounded()
    {
        // 🔥 Detecta si hay suelo justo debajo del jugador
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
        float x = Input.GetAxisRaw("Horizontal"); // 🔥 RAW detecta input directo, no arrastre físico
        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runningSpeed : moveSpeed;

        rb.velocity = new Vector2(x * currentSpeed, rb.velocity.y);

        // Girar sprite según dirección
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

        // --- 🎧 Control de sonido de pasos ---
        bool isMovingByInput = Mathf.Abs(x) > 0.1f; // solo si hay input real
        if (isMovingByInput && canJump && !isClimbing)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                UIAudioManager.Instance.PlaySFX(walkingSound, 0.3f);

                footstepTimer = isRunning ? footstepInterval * 0.7f : footstepInterval; // pasos más rápidos al correr
            }
        }
        else
        {
            footstepTimer = 0f; // resetea el temporizador al quedarse quieto
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
        //isAttacking = true;
        //animator.SetBool("isAttacking", true);

        //if (attackSmokeEffect != null)
        //    Instantiate(attackSmokeEffect, transform.position, Quaternion.identity);

        //yield return new WaitForSeconds(attackDuration);

        //isAttacking = false;
        //animator.SetBool("isAttacking", false);

        isAttacking = true;
        animator.SetBool("isAttacking", true);

        if (attackSmokeEffect != null)
            Instantiate(attackSmokeEffect, transform.position, Quaternion.identity);

        attackArea.SetActive(true); // 🔥 activar colisión del ataque
        yield return new WaitForSeconds(attackDuration);
        attackArea.SetActive(false); // 🔥 desactivar al terminar

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    

    void Climb()
    {
        verticalInput = Input.GetAxisRaw("Vertical"); // input directo
        rb.velocity = new Vector2(0, verticalInput * moveSpeed / 1.5f);

        // --- 🔊 sonido de escalada ---
        bool isClimbingByInput = Mathf.Abs(verticalInput) > 0.1f; // solo si se mueve realmente
        if (isClimbingByInput)
        {
            climbStepTimer -= Time.deltaTime;
            if (climbStepTimer <= 0f)
            {
                UIAudioManager.Instance.PlaySFX(walkingSound, 0.3f); // 🔥 mismo clip, recortado
                climbStepTimer = climbStepInterval;
            }
        }
        else
        {
            climbStepTimer = 0f; // resetea cuando se detiene
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

        // Si la vida es mayor que 0, solo actualiza corazones
        if (life > 0)
        {
            Debug.Log($"💔 Jugador herido. Vida actual: {life}");
            // Aquí puedes agregar un pequeño parpadeo o invulnerabilidad temporal
            return;
        }

        // Si la vida llega a 0, entonces sí reinicia
        Debug.Log("☠️ Jugador ha muerto.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        isDashing = false;
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
    }
    public void EnterDoor()
    {
        Destroy(gameObject);
    }
}
