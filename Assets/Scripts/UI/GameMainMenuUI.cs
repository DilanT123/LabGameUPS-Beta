using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameMainMenuUI : MonoBehaviour
{
    [SerializeField] Button startHostBtn;   
    [SerializeField] Button joinGameBtn;
    [SerializeField] GameObject popUpFindServers;
    [SerializeField] GameObject menuButtons;



    private void Start()
    {
        startHostBtn.onClick.AddListener(() => StartLobby());
        joinGameBtn.onClick.AddListener(ShowPopUp); // Muestra el PopUp para buscar servers locales
    }
    private void StartLobby()
    {
        NetworkManager.singleton.StartHost();
    }

    private void ShowPopUp()
    {
        popUpFindServers.SetActive(true); // Muestra el popup
        menuButtons.SetActive(false); // Oculta el panel de botones del menú
    }

}
