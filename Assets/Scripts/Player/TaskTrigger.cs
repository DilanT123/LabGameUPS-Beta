using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
    private PlayerControler playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<PlayerControler>();
            if (playerController != null)
            {
                playerController.SetNearTaskTrigger(true);
                Debug.Log("Jugador dentro del rango del trigger.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerController != null)
        {
            playerController.SetNearTaskTrigger(false);
            playerController = null;
            Debug.Log("Jugador fuera del rango del trigger.");
        }
    }

    // Ya no necesitamos el método Update aquí
}