using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ElectricLine : MonoBehaviour
{
    [Header("Puntos del rayo (asigna en el inspector)")]
    public Transform pointA;
    public Transform pointB;

    [Header("Apariencia")]
    public int segments = 2;            // basta con 2 para línea recta (A -> B)
    public float thickness = 0.2f;      // grosor del collider

    [Header("Comportamiento de daño")]
    public bool oneUse = true;          // si true: el rayo solo mata una vez y luego se desactiva
    public AudioClip deathSound;        // opcional: sonido al matar
    public GameObject impactEffect;     // opcional: efecto al impactar

    private LineRenderer lr;
    private BoxCollider2D boxCol;
    private Vector3[] points;
    private bool hasKilled = false;

    void Reset()
    {
        // por conveniencia, cuando agregas el componente desde inspector:
        // asegura que haya 2 segmentos por defecto y BoxCollider2D
        segments = 2;
    }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        boxCol = GetComponent<BoxCollider2D>();

        if (lr == null) lr = gameObject.AddComponent<LineRenderer>();
        if (boxCol == null) boxCol = gameObject.AddComponent<BoxCollider2D>();

        // configuraciones seguras por defecto
        lr.positionCount = Mathf.Max(2, segments);
        lr.useWorldSpace = true;

        boxCol.isTrigger = true;
    }

    void Start()
    {
        // Construye la línea estática A->B (sin ruido ni movimiento)
        points = new Vector3[lr.positionCount];

        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("ElectricLine: asigna pointA y pointB en el inspector.");
            return;
        }

        // Sólo 2 puntos necesarios para una línea recta entre A y B
        points[0] = pointA.position;
        points[points.Length - 1] = pointB.position;

        // Si por algún motivo segments > 2, interpolamos uniformemente entre A y B
        for (int i = 0; i < points.Length; i++)
        {
            float t = (float)i / (points.Length - 1);
            points[i] = Vector3.Lerp(pointA.position, pointB.position, t);
        }

        lr.SetPositions(points);

        // Ajusta el BoxCollider2D para cubrir la línea entre A y B
        UpdateColliderBetweenPoints(pointA.position, pointB.position);
    }

    // No actualizamos en Update porque la línea debe permanecer estática
    void Update() { }

    private void UpdateColliderBetweenPoints(Vector3 a, Vector3 b)
    {
        // Coloca el collider en el punto medio y lo orienta hacia B-A
        Vector3 mid = (a + b) * 0.5f;
        float distance = Vector2.Distance(a, b);

        // BoxCollider2D está en espacio local; colocamos su transform en el mundo.
        boxCol.transform.position = mid;

        // Tamaño en X = distancia, Y = grosor
        boxCol.size = new Vector2(distance, thickness);

        // Rota el collider para que apunte de A a B
        Vector3 dir = (b - a).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        boxCol.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasKilled && oneUse) return; // ya mató antes y está configurado para una sola vez

        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        // Mata instantáneamente: usa la vida actual del jugador para garantizar muerte
        player.TakeDamage(1);

        // Efectos opcionales
        if (impactEffect != null)
            Instantiate(impactEffect, collision.transform.position, Quaternion.identity);

        if (deathSound != null)
            UIAudioManager.Instance.PlaySFX(deathSound, 1f);

        hasKilled = true;

        if (oneUse)
        {
            // Opción: desactivar visual y collider para que deje de ser peligroso
            lr.enabled = false;
            boxCol.enabled = false;

            // Si prefieres destruir el objeto en lugar de desactivarlo, usa:
            // Destroy(gameObject, 0.1f);
        }
    }

    // Si mueves pointA/pointB en el editor en modo Play, puedes llamar manualmente a esto
    public void RebuildStaticLine()
    {
        if (pointA == null || pointB == null) return;
        Start(); // reusa Start para recalcular (simple y seguro para este caso)
    }
}
