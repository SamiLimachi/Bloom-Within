using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ElectricLine : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public int segments = 10;          // cantidad de puntos entre A y B
    public float noiseAmount = 0.05f;  // cuánto tiembla el rayo
    public float noiseSpeed = 10f;     // velocidad de movimiento
    private LineRenderer lr;
    private Vector3[] points;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments;
        points = new Vector3[segments];
    }

    void Update()
    {
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector3 basePos = Vector3.Lerp(pointA.position, pointB.position, t);

            // Ruido aleatorio (vibración)
            float offset = Mathf.PerlinNoise(Time.time * noiseSpeed, i * 0.1f) * 2 - 1;
            basePos.y += offset * noiseAmount;

            points[i] = basePos;
        }

        lr.SetPositions(points);
    }
}
