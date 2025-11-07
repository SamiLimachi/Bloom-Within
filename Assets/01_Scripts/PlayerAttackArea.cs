using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackArea : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🧠 Golpe a jefes
        if (collision.CompareTag("Boss"))
        {
            player.Heal();

            Boss boss = collision.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(player.attackDamage);
                Debug.Log($"Golpe al jefe: -{player.attackDamage} HP");
            }
        }

        // 💥 Destruir proyectiles enemigos
        else if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Proyectil destruido por ataque del jugador");
        }

        // 🧱 Paredes rompibles
        else if (collision.CompareTag("Breakable"))
        {
            BreakableWall wall = collision.GetComponent<BreakableWall>();
            if (wall != null)
            {
                wall.TakeHit();
                Debug.Log("Pared golpeada");
            }
        }

        // 🧫 Enemigos normales (como el Slime)
        else if (collision.CompareTag("Enemy"))
        {
            Slime slime = collision.GetComponent<Slime>();
            if (slime != null)
            {
                player.Heal(); // Curación al golpear enemigo
                slime.TakeDamage(player.attackDamage);
                Debug.Log($"Golpe al Slime: -{player.attackDamage} HP");
            }
        }
    }
}