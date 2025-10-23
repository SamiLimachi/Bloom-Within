using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWatcher : Boss
{
    [Header("Movimiento del Jefe")]
    public float MoveSpeed = 2.5f;
    public Transform StartPoint;
    public Transform EndPoint;
    private Transform target;
    private bool canMove = true;

    [Header("Referencias de Ataque")]
    public Transform centerPoint;
    public Transform[] firePoints;
    public GameObject projectilePrefab;

    [Header("Control de Ataques")]
    public float timeBetweenAttacks = 2f; // 🔹 Menor tiempo entre ataques
    private bool canAttack = true;
    private Transform player;

    void Start()
    {
        Life = 30;
        target = EndPoint;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (canMove)
            Movement();

        if (canAttack)
            StartCoroutine(AttackPattern());
    }

    // -------------------------------
    // MOVIMIENTO ENTRE PUNTOS
    // -------------------------------
    public void Movement()
    {
        if (StartPoint == null || EndPoint == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            MoveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == EndPoint) ? StartPoint : EndPoint;

            Vector3 scale = transform.localScale;
            scale.x = (target == EndPoint) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    // -------------------------------
    // CONTROL DE ATAQUES
    // -------------------------------
    IEnumerator AttackPattern()
    {
        canAttack = false;

        int randomAttack = Random.Range(0, 3);

        switch (randomAttack)
        {
            case 0:
                yield return StartCoroutine(ExplosionAttack());
                break;
            case 1:
                yield return StartCoroutine(FireEyesAttack());
                break;
            case 2:
                yield return StartCoroutine(LaserAttack());
                break;
        }

        // ✅ Ahora el movimiento se reanuda inmediatamente
        canMove = true;

        // 🔹 Espera breve antes del próximo ataque (sin detener movimiento)
        yield return new WaitForSeconds(timeBetweenAttacks);

        canAttack = true;
    }

    // -------------------------------
    // ATAQUE 1: EXPLOSIÓN RADIAL
    // -------------------------------
    IEnumerator ExplosionAttack()
    {
        Debug.Log("💥 Explosion radial");
        canMove = false;
        transform.position = centerPoint.position;

        yield return new WaitForSeconds(0.2f); // pequeña carga visual

        int numProjectiles = 20;
        float angleStep = 360f / numProjectiles;
        float shootForce = 7f;

        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = dir * shootForce;

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        yield return new WaitForSeconds(0.5f); // tiempo de quedarse quieto

        // Teletransporte rápido
        Transform teleportTarget = (Random.value < 0.5f) ? StartPoint : EndPoint;
        transform.position = teleportTarget.position;

        Debug.Log("✨ Teleport rápido al borde");

        yield return new WaitForSeconds(0.3f); // breve pausa antes de volver a moverse
        canMove = true;
    }

    // -------------------------------
    // ATAQUE 2: DISPARO DE LOS OJOS (puede moverse)
    // -------------------------------
    IEnumerator FireEyesAttack()
    {
        Debug.Log("👁️ Disparo en movimiento");

        int burstCount = 3;          // número de ráfagas
        float burstDelay = 0.25f;    // tiempo entre ráfagas
        float projectileSpeed = 8f;

        for (int b = 0; b < burstCount; b++)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoints[i].position, Quaternion.identity);
                Vector2 dir = (player.position - firePoints[i].position).normalized;

                // Pequeña variación para patrón más natural
                dir = Quaternion.Euler(0, 0, Random.Range(-4f, 4f)) * dir;

                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null) rb.velocity = dir * projectileSpeed;

                float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                proj.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            }

            yield return new WaitForSeconds(burstDelay);
        }
    }

    // -------------------------------
    // ATAQUE 3: LÁSER DIRECCIONAL
    // -------------------------------
    IEnumerator LaserAttack()
    {
        Debug.Log("🔫 The Watcher prepara ataque de láser");

        canMove = false;

        Transform attackPoint = (Random.value < 0.5f) ? StartPoint : EndPoint;
        transform.position = attackPoint.position;

        Vector3 scale = transform.localScale;
        scale.x = (player.position.x > transform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;

        yield return new WaitForSeconds(0.2f); // pequeña carga

        Vector2 dir = (player.position - transform.position).normalized;
        GameObject laser = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = dir * 9f;

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(0, 0, rotZ);

        yield return new WaitForSeconds(0.6f); // menos tiempo quieto
        canMove = true;
    }

    protected override void Die()
    {
        Debug.Log("💀 The Watcher ha sido derrotado. Se abre la puerta dimensional...");
        base.Die();
    }
}
