using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;

    public enum GameState
    {
        None,
        GamePause,
        GameStart,
        GameOver
    }

    public GameState GetState = GameState.None; // Estado actual del juego

    [HideInInspector]
    public PlayerControler localPlayer; // Referencia al jugador local

    public GameUI gameUI; // Referencia al UI del juego

    public static GameManager GetInstance()
    {
        return instance; // Retorna la instancia singleton
    }

    private void Awake()
    {
        instance = this; // Inicializa la instancia singleton
    }

    [Server]
    public void StartGame()
    {
        RpcStartGame(); // Inicia el juego en los clientes
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        gameUI.OnStartGame(); // Actualiza la UI para iniciar el juego
    }

    [SyncVar]
    private string winnerName = ""; // Nombre del jugador ganador

    [Server]
    public void PlayerCompletedAllTasks(string playerName)
    {
        if (string.IsNullOrEmpty(winnerName))
        {
            winnerName = playerName; // Establece el ganador si no hay uno
            RpcShowWinner(winnerName); // Muestra al ganador en todos los clientes
        }
    }

    [ClientRpc]
    private void RpcShowWinner(string winner)
    {
        Debug.Log($"Jugador ganador: {winner}");
        if (gameUI != null)
        {
            gameUI.OnShowWinner(winner); // Actualiza la UI con el ganador
        }
        else
        {
            Debug.LogError("gameUI is null in GameManager"); // Error si gameUI es nulo
        }
    }

    [Command]
    private void CmdGameOver()
    {
        RpcShowWinner(winnerName); // Muestra el ganador cuando el juego termina
    }
}
