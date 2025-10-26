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
        if (collision.CompareTag("Boss"))
        {
            Boss boss = collision.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(player.attackDamage);
                Debug.Log($"Golpe al jefe: -{player.attackDamage} HP");
            }
        }
        else if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Breakable")) // 💥 nuevo tag para paredes
        {
            BreakableWall wall = collision.GetComponent<BreakableWall>();
            if (wall != null)
            {
                wall.TakeHit();
            }
        }
    }
}
