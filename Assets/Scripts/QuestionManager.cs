using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeenieWalker
{
    public class QuestionManager : MonoBehaviour
    {
        public static event Action<string> OnSettingDifficulty;
        public static event Action<string> OnSettingCategory;
        public static event Action<string> OnSettingQuestion;
        public static event Action<string, List<string>> OnSettingAnswers;


        int streak = 0;
        int longestStreak = 0;

        #region TestingVariables
        public int responseCode;
        public string questionType = "multiple";
        public string difficultyString = "medium";
        public string categoryString = "History";
        public string questionString = "What was the bloodiest single-day battle during the American Civil War?";
        public string correctAnswerString = "The Battle of Antietam";
        public List<string> incorrectAnswerList = new List<string>() {
            "The Siege of Vicksburg",
            "The Battle of Gettysburg",
            "The Battles of Chancellorsville" };
        #endregion

        private void OnEnable()
        {
            GameManager.OnAskingQuestion += SetAllQuestionInfo;
            GameManager.OnAnsweredQuestion += AnsweredQuestion;
            GameManager.OnNewGame += Reset;
            UIManager.OnReturnQuestionStreak += ReturnStreak;
        }

        private void OnDisable()
        {
            GameManager.OnAskingQuestion -= SetAllQuestionInfo;
            GameManager.OnAnsweredQuestion -= AnsweredQuestion;
            GameManager.OnNewGame -= Reset;
            UIManager.OnReturnQuestionStreak -= ReturnStreak;
        }

        private void Start()
        {
            Reset();
        }

        private void Reset()
        {
            streak = 0;
            longestStreak = 0;
        }

        private void SetAllQuestionInfo(bool isAsking)
        {
            if (isAsking)
            {
                OnSettingCategory?.Invoke(categoryString);
                OnSettingDifficulty?.Invoke(difficultyString);
                OnSettingQuestion?.Invoke(questionString);
                OnSettingAnswers?.Invoke(correctAnswerString, incorrectAnswerList);
            }
        }

        private void AnsweredQuestion(bool isCorrect, string difficulty)
        {
            if (isCorrect)
            {
                streak++;

            }
            else
            {
                streak = 0;
            }

            if (streak > longestStreak) longestStreak = streak;
        }

        private int ReturnStreak()
        {
            Debug.Log("Returning " + longestStreak);
            return longestStreak;
        }

    }
}
