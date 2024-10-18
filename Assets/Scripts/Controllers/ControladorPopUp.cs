using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class ControladorPopUp : NetworkBehaviour
{
    public GameObject popUp, multipleChoiceSection, matchingSection, sequenceSection;
    public Button botonCompletarTarea;
    private SyncVarPlayers syncVarPlayers;
    private string currentTaskType;
    private string currentTaskData;
    private LocalDatabaseManager localDatabaseManager;

    private void Start()
    {
        botonCompletarTarea?.onClick.AddListener(CompletarTarea);
        OcultarPopUp(); // Inicia con el popup oculto
        localDatabaseManager = FindObjectOfType<LocalDatabaseManager>();
        if (localDatabaseManager == null)
        {
            Debug.LogError("LocalDatabaseManager no encontrado en la escena.");
        }
    }

    public void MostrarPopUp(SyncVarPlayers playerSyncVar, string taskType, string taskData)
    {
        syncVarPlayers = playerSyncVar;
        currentTaskType = taskType;
        currentTaskData = taskData;
        popUp.SetActive(true);
        SetSectionsActive(false);

        Question question = localDatabaseManager.GetRandomQuestion(taskType);
        if (question != null)
        {
            SetupQuestionUI(question);
        }
        else
        {
            Debug.LogError("No se pudo obtener una pregunta de la base de datos local.");
        }
    }

    private void SetupQuestionUI(Question question)
    {
        switch (question.TaskType)
        {
            case "MultipleChoice":
                SetupTask(multipleChoiceSection, () => SetupMultipleChoiceTask(question));
                break;
            case "Matching":
                SetupTask(matchingSection, () => SetupMatchingTask(question));
                break;
            case "Sequence":
                SetupTask(sequenceSection, () => SetupSequenceTask(question));
                break;
        }
    }

    private void SetupTask(GameObject section, Action setup)
    {
        section.SetActive(true);
        setup();
    }

    private void SetupMultipleChoiceTask(Question question)
    {
        Debug.Log("Configurando tarea de opción múltiple.");

        // Configuración del texto de la pregunta
        var questionTextTransform = multipleChoiceSection.transform.Find("QuestionText");
        if (questionTextTransform == null)
        {
            Debug.LogError("QuestionText GameObject no encontrado en MultipleChoice section.");
            return;
        }

        var questionText = questionTextTransform.GetComponent<TextMeshProUGUI>();
        if (questionText == null)
        {
            Debug.LogError("Component TextMeshProUGUI no encontrado en QuestionText GameObject.");
            return;
        }

        questionText.text = question.QuestionText;
        Debug.Log("Pregunta actualizada a: " + question.QuestionText);

        // Configuración de las respuestas
        var answersTransform = multipleChoiceSection.transform.Find("Answers");
        if (answersTransform == null)
        {
            Debug.LogError("Answers GameObject no encontrado en MultipleChoice section.");
            return;
        }

        var answerButtons = answersTransform.GetComponentsInChildren<Button>();
        if (answerButtons.Length != question.Answers.Length)
        {
            Debug.LogError("El número de botones de respuesta no coincide con el número de respuestas.");
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            var answerButton = answerButtons[i];
            var answerTextTransform = answerButton.transform.Find("answer" + (i + 1) + "Text");
            if (answerTextTransform != null)
            {
                var answerText = answerTextTransform.GetComponent<TextMeshProUGUI>();
                if (answerText != null)
                {
                    answerText.text = question.Answers[i];
                }
                else
                {
                    Debug.LogError("Component TextMeshProUGUI no encontrado en el texto del botón de respuesta.");
                }
            }
            else
            {
                Debug.LogError("Texto de respuesta no encontrado en el botón de respuesta " + (i + 1));
            }

            // Agregar listeners para las respuestas
            int index = i; // Evitar problemas con la variable capturada
            answerButton.onClick.RemoveAllListeners();
            answerButton.onClick.AddListener(() => ResponderOpcion(index, question.CorrectAnswerIndex));
        }
    }



    public void ResponderOpcion(int selectedIndex, int correctIndex)
    {
        bool isCorrect = selectedIndex == correctIndex;
        Debug.Log($"Opción seleccionada: {selectedIndex}, Es correcta: {isCorrect}");
        if (isLocalPlayer)
        {
            CmdSendAnswer(isCorrect);
        }
        if (isCorrect)
        {
            CompletarTarea();
        }
        OcultarPopUp();
    }

    [Command]
    private void CmdSendAnswer(bool isCorrect)
    {
        Debug.Log($"Respuesta recibida en el servidor: {(isCorrect ? "Correcta" : "Incorrecta")}");
        // Implementa aquí la lógica adicional del servidor si es necesario
        // se supone revisa directamente desde la base,el servidor no tiene que ver, lo unico que hará 
        // el servidor el recibir afirmativos subidas de puntos,y sincronizar
    }

    public void SetupMatchingTask(Question question)
    {
        // Implementa la lógica para configurar la tarea de emparejamiento aquí
        // Usa la información de 'question' en lugar de 'taskData'
        SetText(matchingSection, "Term1Text1", "Término 1");
        SetText(matchingSection, "Term2Text1", "Emparejar 1");
    }

    public void SetupSequenceTask(Question question)
    {
        // Implementa la lógica para configurar la tarea de secuencia aquí
        // Usa la información de 'question' en lugar de 'taskData'
        var stepTexts = sequenceSection.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < stepTexts.Length; i++)
            stepTexts[i].text = $"Paso {i + 1}";
    }

    public void CompletarTarea()
    {
        if (syncVarPlayers != null && NetworkClient.localPlayer?.GetComponent<SyncVarPlayers>() == syncVarPlayers)
        {
            syncVarPlayers.CompleteTask();
            Debug.Log("CompletarTarea method called from ControladorPopUp.");
        }
        else
        {
            Debug.LogError("SyncVarPlayers no encontrado o el jugador local no está asignado.");
        }
    }

    public void OcultarPopUp()
    {
        popUp.SetActive(false);
        currentTaskType = null;
        currentTaskData = null;
        SetSectionsActive(false); // También desactiva todas las secciones
    }

    private void SetSectionsActive(bool active)
    {
        multipleChoiceSection.SetActive(active);
        matchingSection.SetActive(active);
        sequenceSection.SetActive(active);
    }

    private void SetText(GameObject section, string childName, string text)
    {
        var textComponent = section.transform.Find(childName)?.GetComponent<TextMeshProUGUI>();
        if (textComponent != null) textComponent.text = text;
    }
}