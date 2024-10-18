using System.Collections.Generic;

namespace YourGameNamespace 
{
    public class Question
    {
        public int QuestionID { get; }
        public string QuestionText { get; }
        public string[] Answers { get; }
        public int CorrectAnswerIndex { get; }
        public string TaskType { get; }

        public Question(int questionID, string questionText, string[] answers, int correctAnswerIndex, string taskType)
        {
            QuestionID = questionID;
            QuestionText = questionText;
            Answers = answers;
            CorrectAnswerIndex = correctAnswerIndex;
            TaskType = taskType;
        }
    }
}