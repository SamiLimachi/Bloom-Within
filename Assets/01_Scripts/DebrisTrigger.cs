using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisTrigger : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject debrisPrefab; // prefab del escombro
    public Transform[] spawnPoints; // puntos de caída
    public float delayBetweenDebris = 0.4f; // intervalo entre cada escombro

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (collision.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(SpawnDebrisSequence());
        }
    }

    IEnumerator SpawnDebrisSequence()
    {
        foreach (Transform point in spawnPoints)
        {
            Instantiate(debrisPrefab, point.position, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenDebris);
        }
    }
}
