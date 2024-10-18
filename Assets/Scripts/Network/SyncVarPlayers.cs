using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SyncVarPlayers : NetworkBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider taskSlider;
    private const int maxTasks = 10;
    [SerializeField] private TextMeshProUGUI displayNameTMP = null;

    [Header("Sync Variables")]
    [SyncVar(hook = nameof(OnTaskChange))]
    private int task_completed = 0;
    [SyncVar(hook = nameof(OnNickNameChange))]
    private string nickName = "";

    private void Start()
    {
        // Intenta asignar el Slider automáticamente si no está asignado desde el Inspector
        if (taskSlider == null)
        {
            taskSlider = GetComponentInChildren<Slider>();
            if (taskSlider == null)
            {
                Debug.LogError("No se encontró ningún componente Slider en el hijo de este objeto.");
            }
        }

        // Inicializa el slider al comienzo
        InitializeSlider();

        // Actualiza el nombre en la UI si es local
        if (isLocalPlayer && displayNameTMP != null)
        {
            displayNameTMP.text = nickName;
        }
    }

    private void Update()
    {
        // Permite completar tareas si es el jugador local y se presiona la tecla T
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.T))
        {
            CompleteTask();
        }
    }

    [Server]
    public void SetTaskCompleted(int taskCompleted)
    {
        task_completed = Mathf.Clamp(taskCompleted, 0, maxTasks);
    }

    [Command]
    public void CmdSetTaskCompleted(int taskCompleted)
    {
        SetTaskCompleted(taskCompleted);
    }

    public void CompleteTask()
    {
        CmdSetTaskCompleted(task_completed + 1);
    }

    private void OnTaskChange(int oldTaskCompleted, int newTaskCompleted)
    {
        UpdateSlider(newTaskCompleted);
        Debug.Log($"Tarea completada actualizada: {newTaskCompleted}");

        if (newTaskCompleted >= maxTasks)
        {
            CmdPlayerCompletedAllTasks();
        }
    }
    
    private void OnNickNameChange(string oldNickName, string newNickName)
    {
        if (displayNameTMP != null)
        {
            displayNameTMP.text = newNickName;
        }
    }

    [Command]
    private void CmdPlayerCompletedAllTasks()
    {
        GameManager.GetInstance().PlayerCompletedAllTasks(nickName);
    }

    private void InitializeSlider()
    {
        if (taskSlider != null)
        {
            taskSlider.maxValue = maxTasks;
            taskSlider.value = task_completed;
            taskSlider.interactable = false; // Desactiva la interactividad para los jugadores
        }
    }

    private void UpdateSlider(int value)
    {
        if (taskSlider != null)
        {
            taskSlider.value = value;
        }
    }

    public void UpdateDisplayName(string newName)
    {
        // Actualiza el nombre en la UI
        if (displayNameTMP != null)
        {
            displayNameTMP.text = newName;
        }
        // También actualiza la SyncVar en la red si es necesario
        CmdSetNickName(newName);
    }

    [Command]
    private void CmdSetNickName(string newName)
    {
        nickName = newName;
    }
}
