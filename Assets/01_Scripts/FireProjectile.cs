using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;
    public GameObject impactEffect;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(damage);

            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

}