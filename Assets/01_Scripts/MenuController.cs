using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuController : MonoBehaviour
{
    [Header("Escenas")]
    public string sceneToLoad = "Historia";

    [Header("Referencias del Menú")]
    public VideoPlayer videoPlayer;
    public GameObject startButton;
    public GameObject exitButton;

    void Start()
    {
        // Ocultar botones al inicio
        if (startButton) startButton.SetActive(false);
        if (exitButton) exitButton.SetActive(false);

        // Asegurar reproducción única
        if (videoPlayer)
        {
            videoPlayer.isLooping = false;
            videoPlayer.loopPointReached += OnVideoEnded;
        }
    }

    void OnVideoEnded(VideoPlayer vp)
    {
        // Mostrar botones cuando termina el video
        if (startButton) startButton.SetActive(true);
        if (exitButton) exitButton.SetActive(true);

        // (opcional) habilitar interactividad explícitamente
        var b = startButton?.GetComponent<Button>();
        if (b) b.interactable = true;
    }

    // ¡Sin chequeo de videoFinished!
    public void LoadHistoria()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}