using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class GameLobbyMenuUI : MonoBehaviour
{
    [SerializeField] Button startGameBtn;
    [SerializeField] Button exitLobbyBtn;

    private void Start()
    {
        if (NetworkServer.active)
        {
            startGameBtn.gameObject.SetActive(true);
            startGameBtn.onClick.AddListener(() => startGame());
        }
        else 
        {
            startGameBtn.gameObject.SetActive(false);
        }

        exitLobbyBtn.onClick.AddListener(() => ExitLobbyBtn() );
    }

    private void ExitLobbyBtn()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
    }


    private void startGame()
    {
        NetworkManager.singleton.ServerChangeScene("OnlineScene");
    }
}
