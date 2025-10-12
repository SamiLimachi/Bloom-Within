using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 6f;
    public float runningSpeed = 10f;
    public Rigidbody2D rb;
    public bool isRunning = false;
    float currentSpeed;
    public bool canJump = true;
    public int life = 5;
    public bool BossFight = false;
    public Transform startPoint;

    [Header("Dash / Teletransporte")]
    public float dashDistance = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;
    private float lastDirection = 1f; // guarda la última dirección válida
    public GameObject DashEffect;

    void Start()
    {

    }

    void Update()
    {
        Movement();
        Jump();

        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void Movement()
    {
        if (isDashing) return; // no se mueve durante dash

        float x = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift))
            isRunning = true;
        else
            isRunning = false;

        currentSpeed = isRunning ? runningSpeed : moveSpeed;

        rb.velocity = new Vector2(x * currentSpeed, rb.velocity.y);

        // actualizar dirección y flip visual
        if (x > 0.1f)
        {
            lastDirection = 1f;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x); // asegura que el tamaño en X sea positivo
            transform.localScale = scale;
        }
        else if (x < -0.1f)
        {
            lastDirection = -1f;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); // invierte el signo de X, sin tocar Y ni Z
            transform.localScale = scale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = false;
        }
    }

    void Jump()
    {
        if (canJump && Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

        // Detectar obstáculos delante
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * dashDirection, dashDistance, LayerMask.GetMask("Ground"));

        Vector2 targetPos;
        if (hit.collider != null)
        {
            // Si hay obstáculo, quedarte justo antes de él
            targetPos = hit.point - (Vector2.right * dashDirection * 0.2f);
        }
        else
        {
            // Si no hay nada, hacer el dash completo
            targetPos = new Vector2(transform.position.x + dashDistance * dashDirection, transform.position.y);
        }

        // Teletransportar
        rb.position = targetPos;

        yield return new WaitForSeconds(0.1f);
        Instantiate(DashEffect, transform.position, transform.rotation);
        if (sr != null) sr.enabled = true;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        isDashing = false;
    }

}
