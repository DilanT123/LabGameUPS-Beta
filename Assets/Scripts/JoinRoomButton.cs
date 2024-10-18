using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class JoinRoomButton : MonoBehaviour
{
    public InputField txtIP; // Campo de entrada para la dirección IP
    public InputField txtPuerto; // Campo de entrada para el puerto

    private TcpClient tcpClient;
    private NetworkStream clientStream;
    private byte[] messageBuffer = new byte[4096];

    public void JoinRoom()
    {
        string ipAddress = txtIP.text;
        int port;

        if (!Int32.TryParse(txtPuerto.text, out port))
        {
            Debug.LogError("Puerto inválido");
            return;
        }

        try
        {
            tcpClient = new TcpClient(ipAddress, port);
            clientStream = tcpClient.GetStream();
            Debug.Log("Conectado al servidor");

            // Aquí puedes enviar mensajes al servidor si es necesario
            // Ejemplo: Enviar un mensaje al servidor
            string message = "Hola servidor!";
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            clientStream.Write(messageBytes, 0, messageBytes.Length);

            // Aquí puedes implementar la lógica para recibir mensajes del servidor
            // Puedes usar un hilo dedicado o un bucle de lectura en Update()
            // Ejemplo básico:
            // StartCoroutine(ReceiveMessage());
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al conectar al servidor: " + ex.Message);
        }
    }

    // Puedes implementar métodos adicionales para recibir mensajes del servidor si es necesario

    private void OnDestroy()
    {
        // Asegúrate de cerrar la conexión al destruir el objeto
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }
}
