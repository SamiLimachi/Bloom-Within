using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;
    public AudioSource audioSource;
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
}
