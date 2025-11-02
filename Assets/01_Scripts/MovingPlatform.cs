using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 4f;
    public float waitTime = 1f;

    private Vector3 target;
    private bool isWaiting;

    void Start()
    {
        target = endPoint.position;
    }

    void Update()
    {
        if (!isWaiting)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                StartCoroutine(ChangeTargetAfterDelay());
            }
        }

        // Asegura que siempre mantenga su Z en 0 (importante en 2D)
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    IEnumerator ChangeTargetAfterDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        target = (target == startPoint.position) ? endPoint.position : startPoint.position;
        isWaiting = false;
    }

    // --- Aquí está la parte importante ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
