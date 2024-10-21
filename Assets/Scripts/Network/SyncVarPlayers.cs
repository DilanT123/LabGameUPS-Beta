using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SyncVarPlayers : NetworkBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider taskSlider;
    [SerializeField] private TextMeshProUGUI displayNameTMP;

    private const int maxTasks = 10;

    [SyncVar(hook = nameof(OnTaskChange))]
    private int taskCompleted = 0;

    [SyncVar(hook = nameof(OnNickNameChange))]
    private string nickName = "";

    // Variable para indicar si el juego está en curso
    [SyncVar]
    private bool isGameActive = true;

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

        InitializeSlider();

        // Actualiza el nombre en la UI si es el jugador local
        if (isLocalPlayer && displayNameTMP != null)
        {
            displayNameTMP.text = nickName;
        }
    }

    private void Update()
    {
        // Permite completar tareas solo si el juego está activo y es el jugador local
        //Usado para pruebas
        if (isLocalPlayer && isGameActive && Input.GetKeyDown(KeyCode.T))
        {
            CompleteTask();
        }
    }

    [Server]
    public void SetTaskCompleted(int taskCompleted)
    {
        this.taskCompleted = Mathf.Clamp(taskCompleted, 0, maxTasks);
    }

    [Command]
    public void CmdSetTaskCompleted(int taskCompleted)
    {
        SetTaskCompleted(taskCompleted);
    }

    public void CompleteTask()
    {
        CmdSetTaskCompleted(taskCompleted + 1);
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
            taskSlider.value = taskCompleted;
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
        if (displayNameTMP != null)
        {
            displayNameTMP.text = newName;
        }
        CmdSetNickName(newName);
    }

    [Command]
    private void CmdSetNickName(string newName)
    {
        nickName = newName;
    }

    [Server]
    public void SetGameActive(bool active)
    {
        isGameActive = active;
    }

    private int attemptsRemaining = 3; // Por ejemplo

    public int GetAttemptsRemaining()
    {
        return attemptsRemaining;
    }
}
