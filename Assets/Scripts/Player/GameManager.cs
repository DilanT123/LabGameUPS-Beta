using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;

using Org.BouncyCastle.Tls.Crypto;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;
    public enum GameState{
        None, 
        GamePause, 
        GameStart, 
        GameOver
    }

    public GameState GetState = GameState.None;

    [HideInInspector]
    public PlayerControler localPlayer;

    public GameUI gameUI;
    public static GameManager GetInstance(){
        return instance;
    }
    private void Awake(){
        instance = this;
    }
    [Server]
    public void StartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        gameUI.OnStartGame();
    }

    [SyncVar]
    private string winnerName = "";

    [Server]
    public void PlayerCompletedAllTasks(string playerName)
    {
        if (string.IsNullOrEmpty(winnerName))
        {
            winnerName = playerName;
            RpcShowWinner(winnerName);
        }
    }

    [ClientRpc]
    private void RpcShowWinner(string winner)
    {
        Debug.Log($"Jugador ganador: {winner}");
        if (gameUI != null)
        {
            gameUI.OnShowWinner(winner);
        }
        else
        {
            Debug.LogError("gameUI is null in GameManager");
        }
    }

    [Command]
    private void CmdGameOver()
    {
        RpcShowWinner(winnerName);
    }

    



}
