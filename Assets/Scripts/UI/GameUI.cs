using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{

    public GameObject hudUI;
    public GameCountDownMenuUI countDownUI;
    public GameOverMenuUI whoWinUI;


    IEnumerator Start()
    {

        while (GameManager.GetInstance() == null || GameManager.GetInstance().localPlayer == null)
        {
            yield return null;

            
        }
    }

    public void OnStartGame()
    {
        countDownUI.StartCountDown();
    }

    public void OnShowWinner(string winnerName)
    {
        if (whoWinUI != null)
        {
            whoWinUI.SetNamePlayerWinner(winnerName);
        }
        else
        {
            Debug.LogError("whoWinUI is null in GameUI");
        }
    }
}
