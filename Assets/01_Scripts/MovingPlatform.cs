using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Transform startPoint;
    public Transform endPoint;
    public float speed = 4f;

    private Vector3 target;

    void Start()
    {
        speed = 4f;
        target = endPoint.position;
    }

    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);


        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            target = target == startPoint.position ? endPoint.position : startPoint.position;
        }
    }
}
