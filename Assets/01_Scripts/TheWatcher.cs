using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
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
    public GameObject dashEffect;
    [Header("Control de Ataques")]
    public float timeBetweenAttacks = 2f;
    private bool canAttack = true;
    private Transform player;

    [Header("Ataque Final - Rayos del Cielo")]
    public GameObject lightningWarningPrefab;
    public GameObject lightningStrikePrefab;
    public int lightningCount = 10;
    public float lightningDelay = 0.8f;
    public float warningTime = 1f;
    public float lightningAreaWidth = 16f;
    public float lightningYSpawn = 10f;

    [Header("Animación del jefe")]
    public Animator animator;
    public AudioClip BossMusic;
    public AudioClip LightningEffect;
    public AudioClip ProjectileEffect;
    public AudioClip tpEffect;

    public GameObject Door;

    void Start()
    {
        Life = 150;
        target = EndPoint;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (healthBar != null)
            healthBar.SetMaxHealth(Life);
        AudioSource bossAudio = GetComponent<AudioSource>();
        if (BossMusic != null && bossAudio != null)
        {
            bossAudio.clip = BossMusic;
            bossAudio.loop = false;
            bossAudio.Play();
            StartCoroutine(RestartBossMusic(bossAudio));
        }

    }

    IEnumerator RestartBossMusic(AudioSource audioSource)
    {
        while (true)
        {
            yield return new WaitForSeconds(BossMusic.length);

            // 🔹 Si el jefe sigue vivo y el audio se detuvo, volver a reproducir
            if (Life > 0 && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // 🔹 Si el jefe ya murió, cortar la música
            if (Life <= 0)
            {
                audioSource.Stop();
                yield break;
            }
        }
    }


    void Update()
    {
        if (canMove)
            Movement();

        if (canAttack)
            StartCoroutine(AttackPattern());
    }

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

    IEnumerator AttackPattern()
    {
        canAttack = false;

        if (Life <= 70)
        {
            int specialChance = Random.Range(0, 4);
            if (specialChance == 0)
            {
                yield return StartCoroutine(FinalLightningStorm());
                yield return new WaitForSeconds(timeBetweenAttacks);
                canAttack = true;
                yield break;
            }
        }

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

        canMove = true;
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    IEnumerator ExplosionAttack()
    {
        Debug.Log("💥 Explosion radial");
        canMove = false;

        animator.SetTrigger("Trigger_Explosion");
        Instantiate(dashEffect, transform.position, transform.rotation);
        transform.position = centerPoint.position;
        UIAudioManager.Instance.PlaySFX(tpEffect, 1f);
        yield return new WaitForSeconds(0.3f);
        Instantiate(dashEffect, transform.position, transform.rotation);
        int numProjectiles = 20;
        float angleStep = 360f / numProjectiles;
        float shootForce = 10f;

        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = dir * shootForce;
            proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            UIAudioManager.Instance.PlaySFX(ProjectileEffect, 1f);
        }

        yield return new WaitForSeconds(0.6f);
        canMove = true;
    }

    IEnumerator FireEyesAttack()
    {
        Debug.Log("👁️ Disparo en movimiento");
        animator.SetTrigger("Trigger_FireEyes");

        int burstCount = 3;
        float burstDelay = 0.25f;
        float projectileSpeed = 8f;

        for (int b = 0; b < burstCount; b++)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoints[i].position, Quaternion.identity);
                Vector2 dir = (player.position - firePoints[i].position).normalized;
                dir = Quaternion.Euler(0, 0, Random.Range(-4f, 4f)) * dir;
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null) rb.velocity = dir * projectileSpeed;
                proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                UIAudioManager.Instance.PlaySFX(ProjectileEffect, 1f);
            }
            yield return new WaitForSeconds(burstDelay);
        }
    }

    IEnumerator LaserAttack()
    {
        Debug.Log("🔫 The Watcher prepara ataque de láser");
        canMove = false;
        animator.SetTrigger("Trigger_Laser");

        Transform attackPoint = (Random.value < 0.5f) ? StartPoint : EndPoint;
        Instantiate(dashEffect, transform.position, transform.rotation);
        transform.position = attackPoint.position;
        Instantiate(dashEffect, transform.position, transform.rotation);
        UIAudioManager.Instance.PlaySFX(tpEffect, 1f);

        Vector3 scale = transform.localScale;
        scale.x = (player.position.x > transform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;

        yield return new WaitForSeconds(0.3f);

        Vector2 dir = (player.position - transform.position).normalized;
        GameObject laser = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = dir * 9f;
        laser.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        UIAudioManager.Instance.PlaySFX(ProjectileEffect, 1f);
        yield return new WaitForSeconds(0.6f);
        canMove = true;
    }

    IEnumerator FinalLightningStorm()
    {
        Debug.Log("⚡ The Watcher desata la tormenta final");
        canMove = false;
        animator.SetTrigger("Trigger_Storm");

        Transform teleportTarget = (Random.value < 0.5f) ? StartPoint : EndPoint;
        Instantiate(dashEffect, transform.position, transform.rotation);
        transform.position = teleportTarget.position;
        Instantiate(dashEffect, transform.position, transform.rotation);
        UIAudioManager.Instance.PlaySFX(tpEffect, 1f);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < lightningCount; i++)
        {
            float randomX = Random.Range(-lightningAreaWidth / 2f, lightningAreaWidth / 2f);
            Vector2 groundPos = new Vector2(randomX, -3.91f);
            Vector2 spawnPos = new Vector2(randomX, -3.91f + lightningYSpawn);
            GameObject warning = Instantiate(lightningWarningPrefab, groundPos, Quaternion.identity);
            Destroy(warning, warningTime + 0.5f);
            StartCoroutine(SpawnLightningAfterDelay(spawnPos, warningTime));
            yield return new WaitForSeconds(lightningDelay);
        }

        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    IEnumerator SpawnLightningAfterDelay(Vector2 spawnPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject lightning = Instantiate(lightningStrikePrefab, spawnPos, Quaternion.identity);
        UIAudioManager.Instance.PlaySFX(LightningEffect, 1f);
        Rigidbody2D rb = lightning.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.down * 15f;
        Destroy(lightning, 2f);
    }

    protected override void Die()
    {
        Debug.Log("💀 The Watcher ha sido derrotado.");

        canMove = false;
        canAttack = false; // 🔒 bloquea el patrón de ataque

        AudioSource bossAudio = GetComponent<AudioSource>();
        if (bossAudio != null)
            bossAudio.Stop();

        if (animator != null)
            animator.SetTrigger("Trigger_Death");

        // Ejecuta la coroutine que activará la puerta y luego destruirá al jefe
        StartCoroutine(EnableDoorAfterDelay());
    }

    IEnumerator EnableDoorAfterDelay()
    {
        // Espera exactamente lo que dura la animación de muerte
        float deathAnimDuration = 2.5f; // ajusta según la duración real de tu clip
        yield return new WaitForSeconds(deathAnimDuration);
        //yield return new WaitForSeconds(2f); // tiempo para terminar animación de muerte

        if (Door != null)
        {
            Door.SetActive(true);
            Debug.Log("🚪 Puerta activada después de la muerte del jefe");
        }
        //else
        //{
        //    Debug.LogWarning("⚠️ No se asignó ninguna puerta en el inspector");
        //}

        // 💥 Ahora sí destruye al jefe
        base.Die();
    }


}