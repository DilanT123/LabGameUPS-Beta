using Mirror;
using UnityEngine;

public class TaskCompletionManager : NetworkBehaviour
{
    [Command]
    public void CmdNotifyTaskCompletion()
    {
        // Lógica para actualizar el progreso del jugador en el servidor.
        // Puedes usar un SyncVar en el PlayerController para llevar un conteo de tareas completadas.
    }

    public void NotifyTaskCompletion()
    {
        if (isLocalPlayer)
        {
            CmdNotifyTaskCompletion();
        }
    }
}
