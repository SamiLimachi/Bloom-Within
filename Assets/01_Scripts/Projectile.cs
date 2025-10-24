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
            Player player =other.GetComponent<Player>();
            player.TakeDamage(damage);
            Debug.Log("Jugador golpeado!");
            Destroy(gameObject);
            // Aquí podrías llamar a un método de daño del jugador
        }

        // Evita colisiones con el jefe o sus partes
        
    }
}
