using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{
    public string doorID; // opcional, por si tenés varias puertas
    private bool isPlayerInRange = false;
    public AudioClip doorClip;
    private Player player;
    public string nextLevel;

    private void Update()
    {
        // Si el jugador está dentro y presiona Enter
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return))
        {
            ActivateDoor();
            
        }
    }

    private void ActivateDoor()
    {
        Debug.Log("Puerta activada: " + doorID);
        UIAudioManager.Instance.PlaySFX(doorClip, 1f);       
        StartCoroutine(LoadNextLevel());
        player.EnterDoor();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Jugador frente a la puerta");
            player=collision.GetComponent<Player>();
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(nextLevel);
    }
}
