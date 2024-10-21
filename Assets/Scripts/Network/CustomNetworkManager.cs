using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    // Lista para almacenar referencias a los jugadores conectados
    public List<PlayerControler> players = new List<PlayerControler>();

    // Método llamado cuando un nuevo jugador se conecta al servidor
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Verifica que estamos en la escena correcta para gestionar los jugadores
        if (!SceneManager.GetActiveScene().name.Equals("OnlineScene")) return;

        // Obtiene el componente PlayerControler del jugador conectado y lo añade a la lista
        PlayerControler player = conn.identity.GetComponent<PlayerControler>();
        players.Add(player);

        // Asigna un nombre al jugador y actualiza el nombre en la interfaz si es necesario
        string playerName = "Player " + players.Count;
        player.NickName = playerName;

        var syncVarPlayers = player.GetComponent<SyncVarPlayers>();
        if (syncVarPlayers != null)
        {
            syncVarPlayers.UpdateDisplayName(playerName);
        }

        Debug.Log($"Jugador {playerName} se ha unido. Total de jugadores: {players.Count}");

        // Inicia el juego si hay 4 jugadores conectados
        if (players.Count == 4)
        {
            GameManager.GetInstance().StartGame();
            StartGame();
        }
    }

    // Método llamado al iniciar el servidor
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Servidor iniciado.");

        // Verifica la existencia del GameManager en la escena
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado en la escena");
        }
    }

    // Método para manejar la respuesta del jugador en el servidor
    [Server]
    public void HandlePlayerAnswer(NetworkConnection conn, bool isCorrect)
    {
        Debug.Log($"Jugador {conn.connectionId} respondió {(isCorrect ? "correctamente" : "incorrectamente")}");
    }

    // Método para iniciar el juego, se llama cuando hay 4 jugadores conectados
    [Server]
    private void StartGame()
    {
        Debug.Log("Iniciando el juego con 4 jugadores.");
        
    }
}
