using UnityEngine;

public class TheAbandonedAttackArea : MonoBehaviour
{
    private TheMirrorPlayerBoss boss;

    void Start()
    {
        boss = GetComponentInParent<TheMirrorPlayerBoss>();

        // Evita que el boss se golpee a sí mismo
        Collider2D bossCollider = boss.GetComponent<Collider2D>();
        Collider2D attackCollider = GetComponent<Collider2D>();
        if (bossCollider != null && attackCollider != null)
        {
            Physics2D.IgnoreCollision(bossCollider, attackCollider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Golpea solo al jugador
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(boss.attackDamage);
                Debug.Log($"☠️ {boss.name} golpea al jugador: -{boss.attackDamage} HP");
            }
        }
        // Opcional: destruye proyectiles
        else if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Breakable"))
        {
            BreakableWall wall = collision.GetComponent<BreakableWall>();
            if (wall != null)
                wall.TakeHit();
        }
    }
}
