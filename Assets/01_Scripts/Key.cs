using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyID; // ID único para cada llave
    public AudioClip clip;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            inventory.AddKey(keyID);
            UIAudioManager.Instance.PlaySFX(clip, 1f);
            Destroy(gameObject);

        }
    }
}
