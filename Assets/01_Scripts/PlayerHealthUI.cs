using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Referencias")]
    public Player player;
    public GameObject heartPrefab;

    [Header("Sprites")]
    public Sprite fullHeart;      // ❤️ CorazonLleno
    public Sprite emptyHeart;     // 🤍 CorazonVacio
    public Sprite criticalHeart;  // 💔 CorazonAgrietado

    [Header("Animación de Peligro")]
    public float blinkSpeed = 3f;
    public int criticalThreshold = 2;

    private List<Image> hearts = new List<Image>();
    private bool isCritical = false;
    private float blinkTimer = 0f;
    private bool blinkVisible = true;

    void Start()
    {
        // Crear corazones según la vida inicial del jugador
        for (int i = 0; i < player.life; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);
            Image img = newHeart.GetComponent<Image>();
            img.sprite = fullHeart;
            hearts.Add(img);
        }
    }

    void Update()
    {
        UpdateHearts();
        UpdateBlinking();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < player.life)
            {
                // Corazón roto si está en modo crítico
                if (player.life <= criticalThreshold && criticalHeart != null)
                    hearts[i].sprite = criticalHeart;
                else
                    hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }

        // activa modo crítico
        isCritical = player.life <= criticalThreshold;
    }

    void UpdateBlinking()
    {
        if (!isCritical) return;

        blinkTimer += Time.deltaTime * blinkSpeed;
        if (blinkTimer >= 1f)
        {
            blinkVisible = !blinkVisible;
            blinkTimer = 0f;
        }

        foreach (var heart in hearts)
        {
            if (heart.sprite == criticalHeart)
            {
                heart.color = blinkVisible ? Color.white : new Color(1, 1, 1, 0.3f);
            }
        }
    }
}