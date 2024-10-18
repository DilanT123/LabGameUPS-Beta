using UnityEngine;
using TMPro; // Importamos TextMeshPro
using Mirror;
using YourGameNamespace;
public class ClientQuestionManager : MonoBehaviour 
{
    public TextMeshProUGUI questionText, answer1Text, answer2Text, answer3Text, answer4Text;
    private LocalDatabaseManager databaseManager;

    void Start()
    {
        databaseManager = GetComponent<LocalDatabaseManager>(); 
        if (databaseManager == null)
        {
            Debug.LogError("LocalDatabaseManager no encontrado. Asegúrate de que esté en el mismo GameObject.");
            return;
        }
        RequestQuestion();
    }

    void RequestQuestion()
    {
        Question question = databaseManager.GetRandomQuestion("MultipleChoice");
        if (question != null)
        {
            DisplayQuestion(question);
        }
        else
        {
            Debug.LogError("No se pudo obtener una pregunta de la base de datos local.");
        }
    }

    void DisplayQuestion(Question question)
    {
        questionText.text = question.QuestionText;
        TextMeshProUGUI[] answerTexts = { answer1Text, answer2Text, answer3Text, answer4Text };
        for (int i = 0; i < question.Answers.Length && i < answerTexts.Length; i++)
        {
            answerTexts[i].text = question.Answers[i];
        }
    }
}