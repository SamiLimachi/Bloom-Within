using UnityEngine;

public class WarningPulse : MonoBehaviour
{
    private SpriteRenderer sr;
    private float time;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        time += Time.deltaTime * 4f;
        float alpha = Mathf.PingPong(time, 0.5f) + 0.3f; // oscila entre 0.3–0.8
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
    }
}
