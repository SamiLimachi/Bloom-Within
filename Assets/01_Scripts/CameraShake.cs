using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private Vector3 originalPos;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        originalPos = transform.localPosition;
    }

    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * intensity;
            float offsetY = Random.Range(-1f, 1f) * intensity;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos; // 🔹 vuelve a su posición original
    }
}