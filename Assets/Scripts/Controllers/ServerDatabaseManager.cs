using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;




public class ServerDatabaseManager : NetworkBehaviour
{
    private string connectionString;

    public override void OnStartServer()
    {
        base.OnStartServer();
        connectionString = $"URI=file:{Application.dataPath}/Database/ChemicalTasksDB.db"; // Asegúrate de que la ruta sea correcta
        Debug.Log(VerifyDatabaseConnection()
            ? "Servidor iniciado y conectado a la base de datos."
            : "Servidor iniciado, pero no se pudo conectar a la base de datos.");
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
            Debug.LogError($"Error al conectar a la base de datos: {e.Message}");
            return false;
        }
    }

    [Command]
    public void CmdRequestQuestion(string taskType, NetworkConnectionToClient sender = null)
    {
        switch (taskType)
        {
            case "MultipleChoice": SendMultipleChoiceQuestion(sender); break;
            case "Matching": SendMatchingQuestion(sender); break;
            case "Sequence": SendSequenceQuestion(sender); break;
            default: Debug.LogError($"Tipo de tarea no reconocido: {taskType}"); break;
        }
    }

    private void SendMultipleChoiceQuestion(NetworkConnection sender) =>
        ExecuteQuery("SELECT QuestionText, CorrectAnswer FROM Questions WHERE TaskID = 1",
            reader => TargetSendMultipleChoiceQuestion(sender, reader.GetString(0),
                new System.Collections.Generic.List<string> { "Answer 1", "Answer 2", "Answer 3" }));

    private void SendMatchingQuestion(NetworkConnection sender) =>
        ExecuteQuery("SELECT Term1, Term2 FROM MatchingTasks WHERE TaskID = 1",
            reader => TargetSendMatchingQuestion(sender, reader.GetString(0), reader.GetString(1)));

    private void SendSequenceQuestion(NetworkConnection sender) =>
        ExecuteQuery("SELECT Step1, Step2, Step3 FROM SequenceTasks WHERE TaskID = 1",
            reader => TargetSendSequenceQuestion(sender, reader.GetString(0), reader.GetString(1), reader.GetString(2)));

    private void ExecuteQuery(string query, Action<SqliteDataReader> action)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) action(reader);
                        else Debug.LogWarning("No se encontraron datos para la tarea especificada.");
                    }
                }
            }
        }
        catch (Exception e) { Debug.LogError($"Error al ejecutar la consulta: {e.Message}"); }
    }

    [TargetRpc]
    public void TargetSendMultipleChoiceQuestion(NetworkConnection target, string questionText, System.Collections.Generic.List<string> answers) =>
        SetupTask(target, "MultipleChoice", questionText, answers.ToArray());

    [TargetRpc]
    public void TargetSendMatchingQuestion(NetworkConnection target, string term1, string term2) =>
        SetupTask(target, "Matching", $"{term1};{term2}");

    [TargetRpc]
    public void TargetSendSequenceQuestion(NetworkConnection target, string step1, string step2, string step3) =>
        SetupTask(target, "Sequence", $"{step1};{step2};{step3}");

    private void SetupTask(NetworkConnection target, string taskType, params object[] args)
    {
        var popUpManager = NetworkClient.localPlayer.GetComponent<ControladorPopUp>();
        if (popUpManager != null)
        {
            var method = popUpManager.GetType().GetMethod($"Setup{taskType}Task");
            method?.Invoke(popUpManager, args);
        }
        else Debug.LogError("ControladorPopUp no encontrado en el cliente.");
    }
}
