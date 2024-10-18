using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateRoomScript : MonoBehaviour
{
    
    public string gameSceneName = "SampleScene"; // Nombre de la escena del juego

    void Start()
    {
        Button myButton = GameObject.Find("CreateRoomBUTTON").GetComponent<Button>();
        myButton.onClick.AddListener(CreateRoom);
    }

    public void CreateRoom()
    {
        Debug.Log("Creando sala local...");
        // Aquí puedes añadir lógica adicional antes de iniciar el juego
        
        // Cambia a la escena del juego
        SceneManager.LoadScene(gameSceneName);
    }
}