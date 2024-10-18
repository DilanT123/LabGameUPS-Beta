using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

public class NetworkMapButton : NetworkBehaviour
{
    public GameObject questionPanel;
    public Text questionText;
    public Button[] answerButtons;
    public float closeDelay = 2f;

    private Button mapButton;
    private List<Question> questions;
    private int currentQuestionIndex;

    [System.Serializable]
    public class Question
    {
        public string text;
        public string[] answers;
        public int correctAnswerIndex;
    }

    void Start()
    {
        mapButton = GetComponent<Button>();
        mapButton.onClick.AddListener(OnMapButtonClick);
        
        questionPanel.SetActive(false);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        InitializeQuestions();
    }

    void InitializeQuestions()
    {
        questions = new List<Question>
        {
            new Question
            {
                text = "¿Cuál es el símbolo químico del oro?",
                answers = new string[] { "Ag", "Au", "Fe", "Cu" },
                correctAnswerIndex = 1
            },
            // Puedes añadir más preguntas aquí
        };
    }

    [Command(requiresAuthority = false)]
    void CmdButtonClicked(NetworkConnectionToClient sender = null)
    {
        TargetShowQuestion(sender);
    }

    [TargetRpc]
    void TargetShowQuestion(NetworkConnection target)
    {
        ShowQuestionWindow();
    }

    void OnMapButtonClick()
    {
        if (isLocalPlayer)
        {
            CmdButtonClicked();
        }
    }

    void ShowQuestionWindow()
    {
        questionPanel.SetActive(true);

        currentQuestionIndex = Random.Range(0, questions.Count);
        Question currentQuestion = questions[currentQuestionIndex];

        questionText.text = currentQuestion.text;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.answers[i];
        }
    }

    void OnAnswerSelected(int answerIndex)
    {
        Question currentQuestion = questions[currentQuestionIndex];
        bool isCorrect = answerIndex == currentQuestion.correctAnswerIndex;

        Debug.Log($"Seleccionaste la respuesta: {answerButtons[answerIndex].GetComponentInChildren<Text>().text}");
        Debug.Log(isCorrect ? "¡Correcto!" : "Incorrecto.");

        // Aquí puedes añadir lógica adicional basada en si la respuesta es correcta o no

        Invoke("CloseQuestionWindow", closeDelay);
    }

    void CloseQuestionWindow()
    {
        questionPanel.SetActive(false);
    }
}