using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class HistoriaController : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Referencia al VideoPlayer
    public Button botonOmitir;       // Referencia al botón

    void Start()
    {
        // Si el botón existe, asigna su función
        if (botonOmitir != null)
            botonOmitir.onClick.AddListener(OmitirHistoria);

        // Si el video existe, cuando termine también vamos a Intro automáticamente
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoTerminado;
    }

    void OmitirHistoria()
    {
        SceneManager.LoadScene("Intro");
    }

    void OnVideoTerminado(VideoPlayer vp)
    {
        SceneManager.LoadScene("Intro");
    }
}