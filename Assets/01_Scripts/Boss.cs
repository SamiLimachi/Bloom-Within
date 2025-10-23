using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int Life;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int dmg)
    {
        Life-=dmg;
        if (Life <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Debug.Log("El jefe ha sido derrotado");
        Destroy(gameObject);
    }
}
