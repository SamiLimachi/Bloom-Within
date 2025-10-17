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

    void Start()
    {
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Si está haciendo dash, no permitir movimiento ni salto
        if (isDashing) return;

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
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");

        // Correr
        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runningSpeed : moveSpeed;

        rb.velocity = new Vector2(x * currentSpeed, rb.velocity.y);

        // Girar el sprite según dirección
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
        if (canJump && Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
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
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = false;
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
}
