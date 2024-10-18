using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;
using TMPro;

public class PopUpFindServersController : MonoBehaviour
{
    [SerializeField] private Button findserverButton;        // Bot�n para buscar servidores
    [SerializeField] private Button closeButton;             // Bot�n para cerrar el popup
    [SerializeField] private List<Button> serverButtons;  // Lista de botones para los servidores
    [SerializeField] private GameObject menuButtons;        // Men� de botones principal

    private NetworkDiscovery networkDiscovery;
    private Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    private void Start()
    {
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();

        findserverButton.onClick.AddListener(FindServers);
        closeButton.onClick.AddListener(ClosePopUp);

        ClearServerList(); // Limpiar botones al iniciar

        // Suscribirse al evento de descubrimiento de servidores
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);

        Debug.Log("Script inicializado. NetworkDiscovery encontrado: " + (networkDiscovery != null));
    }

    // Buscar servidores LAN
    public void FindServers()
    {
        Debug.Log("Buscando servidores...");
        ClearServerList(); // Limpiar la lista antes de buscar de nuevo
        discoveredServers.Clear(); // Limpiar servidores previos

        menuButtons.SetActive(false); // Ocultar el men� principal
        networkDiscovery.StartDiscovery(); // Iniciar la b�squeda de servidores
    }

    private void ClearServerList()
    {
        // Desactiva y limpia todos los botones de servidores
        foreach (var button in serverButtons)
        {
            button.gameObject.SetActive(false); // Ocultar los botones
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Vac�o"; // Reiniciar el texto

            Debug.Log("Bot�n limpiado: " + button.gameObject.name);
        }
    }

    // M�todo que se llama cuando se descubre un servidor
    public void OnDiscoveredServer(ServerResponse info)
    {
        Debug.Log("Servidor descubierto: " + info.EndPoint.Address.ToString());

        // Verificar si el servidor ya fue descubierto
        if (discoveredServers.ContainsKey(info.serverId))
        {
            Debug.LogWarning("Servidor duplicado, ya est� en la lista: " + info.EndPoint.Address.ToString());
            return; // No agregar servidores duplicados
        }

        // Almacena el servidor descubierto
        discoveredServers[info.serverId] = info;

        // Buscar un bot�n vac�o para asignar el servidor
        bool buttonAssigned = false;

        foreach (var button in serverButtons)
        {
            if (!button.gameObject.activeSelf) // Si el bot�n est� inactivo
            {
                button.gameObject.SetActive(true); // Activar el bot�n
                button.GetComponentInChildren<TextMeshProUGUI>().text = info.EndPoint.Address.ToString(); // Mostrar la IP

                Debug.Log("Bot�n asignado con IP: " + info.EndPoint.Address.ToString());

                // A�ade un listener para conectarse al servidor al hacer click
                button.onClick.AddListener(() => ConnectToServer(info));
                buttonAssigned = true;
                break;
            }
        }

        if (!buttonAssigned)
        {
            Debug.LogWarning("No se encontraron botones disponibles para mostrar el servidor.");
        }
    }

    // Metodo para conectarse al servidor seleccionado
    private void ConnectToServer(ServerResponse info)
    {
        Debug.Log("Conectando al servidor: " + info.EndPoint.Address.ToString());

        networkDiscovery.StopDiscovery(); // Detiene la b�squeda
        NetworkManager.singleton.StartClient(info.uri); // Conecta el servidor
    }

    // Cerrar el PopUp
    private void ClosePopUp()
    {
        Debug.Log("Cerrando el PopUp de b�squeda de servidores.");

        gameObject.SetActive(false); // Oculta el PopUp
        menuButtons.SetActive(true); // Muestra el men� principal de nuevo
    }

    private void OnDestroy()
    {
        
        if (networkDiscovery != null)
        {
            networkDiscovery.OnServerFound.RemoveListener(OnDiscoveredServer);
        }
    }
}