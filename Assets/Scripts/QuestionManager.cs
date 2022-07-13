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
        }

        private void OnDisable()
        {
            GameManager.OnAskingQuestion -= SetAllQuestionInfo;
        }

        private void Start()
        {

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

    }
}
