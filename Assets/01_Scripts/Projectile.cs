using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador golpeado!");
            // Aquí podrías llamar a un método de daño del jugador
        }

        // Evita colisiones con el jefe o sus partes
        if (!other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}
