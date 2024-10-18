using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class GameMainMenuUI : MonoBehaviour
{
    [SerializeField] Button startHostBtn;
    [SerializeField] Button joinGameBtn;
    [SerializeField] GameObject popUpFindServers;
    [SerializeField] GameObject menuButtons;
    private NetworkDiscovery networkDiscovery; // Cambiar a tipo privado

    private void Start()
    {
        // Obtener el NetworkDiscovery del NetworkManager
        networkDiscovery = NetworkManager.singleton.GetComponent<NetworkDiscovery>();

        // Verificar si el NetworkDiscovery est� asignado
        if (networkDiscovery == null)
        {
            Debug.LogError("NetworkDiscovery no encontrado en el NetworkManager. Aseg�rate de que est� agregado.");
            return; // Salir si no se encontr�
        }

        startHostBtn.onClick.AddListener(() => StartLobby());
        joinGameBtn.onClick.AddListener(ShowPopUp); // Muestra el PopUp para buscar servers locales
    }

    private void StartLobby()
    {
        // Validar que el NetworkDiscovery est� asignado
        if (networkDiscovery == null)
        {
            Debug.LogError("NetworkDiscovery is not assigned. Please assign it in the inspector.");
            return; // Detener la ejecuci�n si no est� asignado
        }

        // Limpiar la lista de servidores descubiertos
        networkDiscovery.StopDiscovery(); // Aseg�rate de que el descubrimiento est� detenido antes de iniciar uno nuevo
        NetworkManager.singleton.StartHost(); // Inicia el host
        networkDiscovery.AdvertiseServer(); // Anuncia el servidor en la red local
    }

    private void ShowPopUp()
    {
        popUpFindServers.SetActive(true); // Muestra el popup
        menuButtons.SetActive(false); // Oculta el panel de botones del men�
    }
}
