using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class GameCountDownMenuUI : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    public void StartCountDown()
    {
        StartCoroutine(StartCountDownRoutine());
    }

    IEnumerator StartCountDownRoutine()
    {
        UpdateText("Loading...");
        yield return new WaitForSeconds(1f);
        UpdateText("3");
        yield return new WaitForSeconds(0.5f);
        UpdateText("2");
        yield return new WaitForSeconds(0.5f);
        UpdateText("1");
        yield return new WaitForSeconds(0.5f);
        UpdateText("Start");
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);

    }

    public void UpdateText(string text)
    {
        textMeshProUGUI.text = text;
    }
}
