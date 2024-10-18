using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class GameOverMenuUI : MonoBehaviour
{
    private TextMeshProUGUI namePlayerWinnerTMP;

    private void Awake()
    {
        // Buscar el TextMeshProUGUI existente o crear uno nuevo si no existe
        namePlayerWinnerTMP = GetComponentInChildren<TextMeshProUGUI>();
        if (namePlayerWinnerTMP == null)
        {
            GameObject textObject = new GameObject("WinnerNameText");
            textObject.transform.SetParent(transform, false);
            namePlayerWinnerTMP = textObject.AddComponent<TextMeshProUGUI>();
            namePlayerWinnerTMP.alignment = TextAlignmentOptions.Center;
            namePlayerWinnerTMP.fontSize = 36;
        }
    }

    public void SetNamePlayerWinner(string name)
    {
        if (namePlayerWinnerTMP != null)
        {
            namePlayerWinnerTMP.text = "Ganador: " + name;
            // Asegúrate de que el canvas esté activo
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("namePlayerWinnerTMP is null in GameOverMenuUI. Can't set winner name: " + name);
        }
    }

}
