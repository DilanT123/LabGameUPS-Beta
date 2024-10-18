using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using YourGameNamespace;
public class LocalDatabaseManager : MonoBehaviour
{
    private string connectionString;

    void Awake()
    {
        connectionString = $"URI=file:{System.IO.Path.Combine(Application.streamingAssetsPath, "ChemicalTasksDB.db")}";
        if (!VerifyDatabaseConnection())
        {
            Debug.LogError("No se pudo conectar a la base de datos local.");
        }
    }

    private bool VerifyDatabaseConnection()
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al conectar a la base de datos local: {e.Message}");
            return false;
        }
    }

    public Question GetRandomQuestion(string taskType)
    {
        switch (taskType)
        {
            case "MultipleChoice":
                return GetRandomMultipleChoiceQuestion();
            case "Matching":
                return GetRandomMatchingQuestion();
            case "Sequence":
                return GetRandomSequenceQuestion();
            default:
                Debug.LogError($"Tipo de tarea no reconocido: {taskType}");
                return null;
        }
    }

    private Question GetRandomMultipleChoiceQuestion()
    {
        string query = @"
        SELECT q.QuestionID, q.QuestionText, q.CorrectAnswer, t.TaskType
        FROM Questions q
        JOIN Tasks t ON q.TaskID = t.TaskID
        WHERE t.TaskType = 'MultipleChoice'
        ORDER BY RANDOM()
        LIMIT 1";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var questionID = reader.GetInt32(reader.GetOrdinal("QuestionID"));
                        var questionText = reader["QuestionText"].ToString();
                        var correctAnswer = reader["CorrectAnswer"].ToString();
                        var taskType = reader["TaskType"].ToString();

                        string answersQuery = "SELECT AnswerText, IsCorrect FROM Answers WHERE QuestionID = @QuestionID";
                        using (var answersCommand = new SqliteCommand(answersQuery, connection))
                        {
                            answersCommand.Parameters.AddWithValue("@QuestionID", questionID);
                            using (var answersReader = answersCommand.ExecuteReader())
                            {
                                var answers = new List<string>();
                                var correctAnswers = new List<bool>();

                                while (answersReader.Read())
                                {
                                    answers.Add(answersReader["AnswerText"].ToString());
                                    correctAnswers.Add(answersReader.GetBoolean(answersReader.GetOrdinal("IsCorrect")));
                                }

                                // Mezclar respuestas
                                var random = new System.Random();
                                for (int i = 0; i < answers.Count; i++)
                                {
                                    int swapIndex = random.Next(0, answers.Count);
                                    var tempAnswer = answers[i];
                                    var tempCorrect = correctAnswers[i];

                                    answers[i] = answers[swapIndex];
                                    correctAnswers[i] = correctAnswers[swapIndex];

                                    answers[swapIndex] = tempAnswer;
                                    correctAnswers[swapIndex] = tempCorrect;
                                }

                                int correctIndex = correctAnswers.IndexOf(true);
                                return new Question(questionID, questionText, answers.ToArray(), correctIndex, taskType);
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    private Question GetRandomMatchingQuestion()
    {
        
        Debug.LogWarning("GetRandomMatchingQuestion no está implementado completamente.");
        return null;
    }

    private Question GetRandomSequenceQuestion()
    {
        
        Debug.LogWarning("GetRandomSequenceQuestion no está implementado completamente.");
        return null;
    }
}

public class Question
{
    public int QuestionID { get; private set; }
    public string QuestionText { get; private set; }
    public string[] Answers { get; private set; }
    public int CorrectAnswerIndex { get; private set; }
    public string TaskType { get; private set; }

    public Question(int questionID, string questionText, string[] answers, int correctAnswerIndex, string taskType)
    {
        QuestionID = questionID;
        QuestionText = questionText;
        Answers = answers;
        CorrectAnswerIndex = correctAnswerIndex;
        TaskType = taskType;
    }
}