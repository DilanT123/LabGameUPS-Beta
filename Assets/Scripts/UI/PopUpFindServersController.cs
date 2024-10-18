using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;
using TMPro;

public class PopUpFindServersController : MonoBehaviour
{
    [SerializeField] private Button findserverButton;        // Botón para buscar servidores
    [SerializeField] private Button closeButton;             // Botón para cerrar el popup
    [SerializeField] private List<Button> serverButtons;  // Lista de botones para los servidores
    [SerializeField] private GameObject menuButtons;        // Menú de botones principal

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

        menuButtons.SetActive(false); // Ocultar el menú principal
        networkDiscovery.StartDiscovery(); // Iniciar la búsqueda de servidores
    }

    private void ClearServerList()
    {
        // Desactiva y limpia todos los botones de servidores
        foreach (var button in serverButtons)
        {
            button.gameObject.SetActive(false); // Ocultar los botones
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Vacío"; // Reiniciar el texto

            Debug.Log("Botón limpiado: " + button.gameObject.name);
        }
    }

    // Método que se llama cuando se descubre un servidor
    public void OnDiscoveredServer(ServerResponse info)
    {
        Debug.Log("Servidor descubierto: " + info.EndPoint.Address.ToString());

        // Verificar si el servidor ya fue descubierto
        if (discoveredServers.ContainsKey(info.serverId))
        {
            Debug.LogWarning("Servidor duplicado, ya está en la lista: " + info.EndPoint.Address.ToString());
            return; // No agregar servidores duplicados
        }

        // Almacena el servidor descubierto
        discoveredServers[info.serverId] = info;

        // Buscar un botón vacío para asignar el servidor
        bool buttonAssigned = false;

        foreach (var button in serverButtons)
        {
            if (!button.gameObject.activeSelf) // Si el botón está inactivo
            {
                button.gameObject.SetActive(true); // Activar el botón
                button.GetComponentInChildren<TextMeshProUGUI>().text = info.EndPoint.Address.ToString(); // Mostrar la IP

                Debug.Log("Botón asignado con IP: " + info.EndPoint.Address.ToString());

                // Añade un listener para conectarse al servidor al hacer click
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

        networkDiscovery.StopDiscovery(); // Detiene la búsqueda
        NetworkManager.singleton.StartClient(info.uri); // Conecta el servidor
    }

    // Cerrar el PopUp
    private void ClosePopUp()
    {
        Debug.Log("Cerrando el PopUp de búsqueda de servidores.");

        gameObject.SetActive(false); // Oculta el PopUp
        menuButtons.SetActive(true); // Muestra el menú principal de nuevo
    }

    private void OnDestroy()
    {
        
        if (networkDiscovery != null)
        {
            networkDiscovery.OnServerFound.RemoveListener(OnDiscoveredServer);
        }
    }
}