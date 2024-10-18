using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class PopUpFindServersController : MonoBehaviour
{
    // Referencias al UI en la jerarquía
    [SerializeField] private Button findserverButton;        // Botón para buscar servidores
    [SerializeField] private Button closeButton;             // Botón para cerrar el popup
    [SerializeField] private GameObject serverButtonPrefab;  // Prefab del botón del servidor
    [SerializeField] private Transform content;              // Contenedor para los botones de los servidores
    [SerializeField] private GameObject menuButtons;        // Referencia al menú de botones principal

    private NetworkDiscovery networkDiscovery;
    private Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    private void Start()
    {
        // Obtener referencia al NetworkDiscovery
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();

        // Configuración de los botones de la UI
        findserverButton.onClick.AddListener(FindServers);
        closeButton.onClick.AddListener(ClosePopUp);

        // Limpiar cualquier servidor previo descubierto
        ClearServerList();
    }

    // Buscar servidores LAN
    public void FindServers()
    {
        ClearServerList();
        discoveredServers.Clear();
        menuButtons.SetActive(false); // Ocultar los botones del menú principal
        networkDiscovery.StartDiscovery(); // Iniciar la búsqueda de servidores
    }

    private void ClearServerList()
    {
        // Destruir todos los botones de servidores previos
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    // Método que se llama cuando se descubre un servidor
    public void OnDiscoveredServer(ServerResponse info)
    {
        // Almacena en el servidor descubierto
        discoveredServers[info.serverId] = info;

        // Crea un nuevo botón para el servidor encontrado
        GameObject newButton = Instantiate(serverButtonPrefab, content);
        newButton.GetComponentInChildren<Text>().text = info.EndPoint.Address.ToString();

        // Añadir un listener al botón para conectarse al servidor al hacer clic
        newButton.GetComponent<Button>().onClick.AddListener(() => ConnectToServer(info));
    }

    // Conectarse al servidor seleccionado
    private void ConnectToServer(ServerResponse info)
    {
        // Detener la búsqueda y conectar
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    // Cerrar el PopUp
    private void ClosePopUp()
    {
        gameObject.SetActive(false); // Oculta el PopUp
        menuButtons.SetActive(true); // Mostrar nuevamente los botones del menú
    }
}
