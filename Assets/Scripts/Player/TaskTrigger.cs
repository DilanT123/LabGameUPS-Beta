using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class TaskTrigger : NetworkBehaviour
{
    private Dictionary<NetworkIdentity, bool> playerStates = new Dictionary<NetworkIdentity, bool>();

    [SyncVar]
    private int triggerID;

    [SyncVar]
    private string triggerName;

    private static int nextTriggerID = 1;

    // Referencia al objeto del aura
    public GameObject aura; // Asegúrate de asignar el prefab del aura en el Inspector

    private void Awake()
    {
        // Asignar automáticamente el triggerID
        triggerID = nextTriggerID++;

        // Asignar automáticamente el triggerName
        triggerName = $"TaskPoint ({triggerID})";

        // Renombrar el GameObject para que coincida con triggerName
        gameObject.name = triggerName;
    }

    private void Start()
    {
        if (isServer)
        {
            // Asegurarse de que el ID y el nombre están sincronizados en todos los clientes
            RpcSyncTriggerInfo(triggerID, triggerName);
        }

        // Inicializa el aura como desactivada
        SetAuraColor(false);
    }

    [ClientRpc]
    private void RpcSyncTriggerInfo(int id, string name)
    {
        triggerID = id;
        triggerName = name;
        gameObject.name = name;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isServer)
        {
            HandleTriggerEnter(collision);
        }
        else
        {
            CmdHandleTriggerEnter(collision.gameObject);
        }
    }

    private void HandleTriggerEnter(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerControler playerController = collision.GetComponent<PlayerControler>();
            if (playerController != null)
            {
                NetworkIdentity playerIdentity = playerController.GetComponent<NetworkIdentity>();

                if (playerIdentity == null)
                {
                    Debug.LogError($"No se encontró NetworkIdentity en el objeto jugador.");
                    return;
                }

                if (!playerStates.ContainsKey(playerIdentity) || !playerStates[playerIdentity])
                {
                    playerStates[playerIdentity] = true;
                    playerController.SetNearTaskTrigger(true);
                    Debug.Log($"Jugador {playerIdentity.netId} dentro del rango del trigger {triggerName} con ID: {triggerID}.");

                    // Activar aura en verde
                    SetAuraColor(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isServer)
        {
            HandleTriggerExit(collision);
        }
        else
        {
            CmdHandleTriggerExit(collision.gameObject);
        }
    }

    private void HandleTriggerExit(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerControler playerController = collision.GetComponent<PlayerControler>();
            if (playerController != null)
            {
                NetworkIdentity playerIdentity = playerController.GetComponent<NetworkIdentity>();

                if (playerIdentity == null)
                {
                    Debug.LogError($"No se encontró NetworkIdentity en el objeto jugador.");
                    return;
                }

                if (playerStates.ContainsKey(playerIdentity))
                {
                    playerStates[playerIdentity] = false;
                    playerController.SetNearTaskTrigger(false);
                    Debug.Log($"Jugador {playerIdentity.netId} ha salido del rango del trigger {triggerName} con ID: {triggerID}.");

                    // Desactivar el trigger y aura
                    DeactivateTrigger();
                    SetAuraColor(false); // Cambia el color del aura a rojo
                }
            }
        }
    }

    [Command]
    private void CmdHandleTriggerEnter(GameObject playerObject)
    {
        Collider2D collider = playerObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            HandleTriggerEnter(collider);
        }
    }

    [Command]
    private void CmdHandleTriggerExit(GameObject playerObject)
    {
        Collider2D collider = playerObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            HandleTriggerExit(collider);
        }
    }

    public void DeactivateTrigger()
    {
        if (isServer)
        {
            RpcDeactivateTrigger();
        }
        else
        {
            CmdDeactivateTrigger();
        }
    }

    [Command]
    private void CmdDeactivateTrigger()
    {
        RpcDeactivateTrigger();
    }

    [ClientRpc]
    private void RpcDeactivateTrigger()
    {
        gameObject.SetActive(false);
        Debug.Log($"Trigger '{triggerName}' con ID {triggerID} ha sido desactivado.");
    }

    public static void DeactivateTriggerByID(int id)
    {
        TaskTrigger[] allTriggers = FindObjectsOfType<TaskTrigger>();
        foreach (TaskTrigger trigger in allTriggers)
        {
            if (trigger.triggerID == id)
            {
                trigger.DeactivateTrigger();
                return;
            }
        }
        Debug.LogWarning($"No se encontró ningún trigger con ID {id}");
    }

    public void ResetTask()
    {
        playerStates.Clear();
        Debug.Log($"Tareas restablecidas. Estado de todas las tareas borrado para el trigger {triggerName} con ID: {triggerID}.");
    }

    public bool IsPlayerNearTaskTrigger(NetworkIdentity playerIdentity)
    {
        return playerStates.ContainsKey(playerIdentity) && playerStates[playerIdentity];
    }

    public bool CheckTriggerID(int id)
    {
        return id == triggerID;
    }

    private void SetAuraColor(bool isActive)
    {
        if (aura != null)
        {
            // Cambiar el color del aura según el estado
            var renderer = aura.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = isActive ? Color.green : Color.red;
            }
            // Si el aura es un GameObject, puedes activarlo o desactivarlo
            aura.SetActive(isActive);
        }
    }
}
