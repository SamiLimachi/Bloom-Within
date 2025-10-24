using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Vida del jefe")]
    public int Life = 100;
    public BossHealthBar healthBar;
    public GameObject bossUI; // referencia al grupo (BossUI)

    void Start()
    {
        if (healthBar != null)
            healthBar.SetMaxHealth(Life);

        if (bossUI != null)
            bossUI.SetActive(true); // asegúrate que se muestre al inicio
    }

    public void TakeDamage(int dmg)
    {
        Life -= dmg;
        Debug.Log($"💢 Daño al jefe: -{dmg} | Vida actual: {Life}");

        if (healthBar != null)
            healthBar.SetHealth(Life);

        if (Life <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log("💀 The Watcher ha sido derrotado.");

        // 🔹 Oculta la interfaz de vida
        if (bossUI != null)
            bossUI.SetActive(false);

        // 🔹 Destruye al jefe
        Destroy(gameObject);
    }
}