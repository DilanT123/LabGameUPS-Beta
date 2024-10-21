using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class ControladorPopUp : NetworkBehaviour
{
    public GameObject popUp, multipleChoiceSection, matchingSection, sequenceSection;
    public Button botonCompletarTarea;
    private SyncVarPlayers syncVarPlayers;
    private string currentTaskType;
    private string currentTaskData;
    private LocalDatabaseManager localDatabaseManager;
    private Button correctButton; // Añadir color al botón correcto

    private void Start()
    {
        botonCompletarTarea?.onClick.AddListener(CompletarTarea);
        OcultarPopUp(); 
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

            // Referencia al botón correcto
            if (i == question.CorrectAnswerIndex)
            {
                correctButton = answerButton; // Guarda el valor del botón correcto
            }
            // Agregar listeners para las respuestas
            int index = i; // Evitar problemas con la variable capturada
            answerButton.onClick.RemoveAllListeners();
            answerButton.onClick.AddListener(() => ResponderOpcion(index, question.CorrectAnswerIndex));
        }
    }

    // Verifica la respuesta correcta
    public void ResponderOpcion(int selectedIndex, int correctIndex)
    {
        bool isCorrect = selectedIndex == correctIndex;
        Debug.Log($"Opción seleccionada: {selectedIndex}, Es correcta: {isCorrect}");

        // Cambiar el color del texto del botón correcto a verde
        if (correctButton != null)
        {
            var correctTextTransform = correctButton.transform.Find("answer" + (correctIndex + 1) + "Text");
            var correctText = correctTextTransform?.GetComponent<TextMeshProUGUI>();
            // Si la respuesta es correcta, cambiar a verde
            if (isCorrect && correctText != null)
            {
                correctText.color = Color.green; // Cambia el color del texto a verde
            }
        }

        // Cambiar el color del texto del botón incorrecto a rojo
        if (!isCorrect)
        {
            var selectedButton = multipleChoiceSection.transform.Find("Answers").GetChild(selectedIndex);
            var selectedTextTransform = selectedButton?.Find("answer" + (selectedIndex + 1) + "Text");
            var selectedText = selectedTextTransform?.GetComponent<TextMeshProUGUI>();

            if (selectedText != null)
            {
                selectedText.color = Color.red; // Cambia el color del texto a rojo
            }
        }

        // Desactivar todos los botones después de seleccionar una respuesta
        DisableAnswerButtons(); // Desactivar botones

        //Si la respuesta es correcta, se completa la tarea y se cierra el popup
        if (isCorrect)
        {
            if (isLocalPlayer)
            {
                CmdSendAnswer(isCorrect);
            }

            // Completar la tarea si la respuesta es correcta
            CompletarTarea();

            // Se inicia una corrutina para ocultar el popup
            StartCoroutine(RestoreButtonColorsAndHide());
        }
        else
        {
            // Para respuestas incorrectas, también ocultamos el popup, puedes modificar esto si quieres esperar más
            StartCoroutine(RestoreButtonColorsAndHide());
        }
    }

    [Command]
    private void CmdSendAnswer(bool isCorrect)
    {
        Debug.Log($"Respuesta recibida en el servidor: {(isCorrect ? "Correcta" : "Incorrecta")}");
    }

    public void SetupMatchingTask(Question question)
    {
        // Implementa la lógica para configurar la tarea de emparejamiento aquí
        SetText(matchingSection, "Term1Text1", "Término 1");
        SetText(matchingSection, "Term2Text1", "Emparejar 1");
    }

    public void SetupSequenceTask(Question question)
    {
        // Implementa la lógica para configurar la tarea de secuencia aquí
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
        SetSectionsActive(false);
        EnableAnswerButtons(); // Habilitar botones al ocultar el popup

    }

    private IEnumerator RestoreButtonColorsAndHide()
    {
        yield return new WaitForSeconds(2); // retraso de 2 segundos

        // Restaurar color original del texto del botón (hex 323232) para todos los textos de respuesta
        var answerTransform = multipleChoiceSection.transform.Find("Answers");

        if (answerTransform != null)
        {
            var answerButtons = answerTransform.GetComponentsInChildren<Button>();
            foreach (var button in answerButtons)
            {
                var index = Array.IndexOf(answerButtons, button); // Obtener el índice del botón
                var answerTextTransform = button.transform.Find("answer" + (index + 1) + "Text");
                var answerText = answerTextTransform?.GetComponent<TextMeshProUGUI>();
                if (answerText != null)
                {
                    answerText.color = new Color32(50, 50, 50, 255); // Color original (hex 323232)
                }
            }
        }

        OcultarPopUp();
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
        else Debug.LogError($"No se encontró el componente TextMeshProUGUI en {childName}.");
    }

    internal void ShowTaskPopup(SyncVarPlayers playerSyncVar, string taskType, string taskData)
    {
        MostrarPopUp(playerSyncVar, taskType, taskData);
    }

    // Método para ocultar el popup
    public void HideTaskPopup()
    {
        OcultarPopUp();
    }

    private void DisableAnswerButtons()
    {
        var answerTransform = multipleChoiceSection.transform.Find("Answers");
        if (answerTransform != null)
        {
            var answerButtons = answerTransform.GetComponentsInChildren<Button>();
            foreach (var button in answerButtons)
            {
                button.interactable = false; // Desactiva todos los botones
            }
        }
    }

    private void EnableAnswerButtons()
    {
        var answerTransform = multipleChoiceSection.transform.Find("Answers");
        if (answerTransform != null)
        {
            var answerButtons = answerTransform.GetComponentsInChildren<Button>();
            foreach (var button in answerButtons)
            {
                button.interactable = true; // Habilita todos los botones
            }
        }
    }

}
