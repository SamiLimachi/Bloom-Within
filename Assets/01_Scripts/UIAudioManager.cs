using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;
    public AudioSource audioSource;
    public AudioSource sfxAS;
    public AudioClip clickSound;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }
    public void PlaySFX(AudioClip clip, float duration = 0.3f)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        // ?? destruye el AudioSource despu�s de un fragmento corto
        Destroy(source, duration);
    }
}
