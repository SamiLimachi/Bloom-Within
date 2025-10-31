using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip clip;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player =other.GetComponent<Player>();
            player.Heal1();
            UIAudioManager.Instance.PlaySFX(clip, 1f);
            Destroy(gameObject);
        }
    }
}
