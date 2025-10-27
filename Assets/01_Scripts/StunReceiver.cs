using System.Collections;
using UnityEngine;

public class StunReceiver : MonoBehaviour
{
    public bool isStunned { get; private set; }
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyStun(float seconds)
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(StunRoutine(seconds));
    }

    IEnumerator StunRoutine(float seconds)
    {
        isStunned = true;
        float end = Time.time + seconds;

        while (Time.time < end)
        {
            if (rb != null)
            {
                // “bloquea” control simple: frena horizontalmente
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            yield return null;
        }

        isStunned = false;
    }
}
