using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public List<PlayerControler> players = new List<PlayerControler>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        if (!SceneManager.GetActiveScene().name.Equals("OnlineScene")) return;

        PlayerControler player = conn.identity.GetComponent<PlayerControler>();
        players.Add(player);

        string playerName = "Player " + players.Count;
        player.NickName = playerName;

        var syncVarPlayers = player.GetComponent<SyncVarPlayers>();
        if (syncVarPlayers != null)
        {
            syncVarPlayers.UpdateDisplayName(playerName);
        }

        Debug.Log($"Jugador {playerName} se ha unido. Total de jugadores: {players.Count}");

        if (players.Count == 4)
        {
            GameManager.GetInstance().StartGame();
            StartGame();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Servidor iniciado.");

        // Asegúrate de que el GameManager esté configurado
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene");
        }
    }

    [Server]
    public void HandlePlayerAnswer(NetworkConnection conn, bool isCorrect)
    {
        Debug.Log($"Jugador {conn.connectionId} respondió {(isCorrect ? "correctamente" : "incorrectamente")}");
    }

    [Server]
    private void StartGame()
    {
        Debug.Log("Iniciando el juego con 4 jugadores.");
        
    }
}
