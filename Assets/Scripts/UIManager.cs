using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WeenieWalker
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] GameObject gameQuestionCanvas; 

        [SerializeField] TMP_Text questionTextSpot;
        [SerializeField] Image difficultyImageSpot;
        [SerializeField] List<Sprite> difficultyImages = new List<Sprite>();

        [SerializeField] TMP_Text categoryTextSpot;

        [SerializeField] GameObject booleanAnswerHolder;
        [SerializeField] GameObject multipleAnswerHolder;
        [SerializeField] List<TMP_Text> booleanAnswerSpots = new List<TMP_Text>();
        [SerializeField] List<TMP_Text> multipleAnswerSpots = new List<TMP_Text>();


        private List<string> answers = new List<string>();
        private string storedCorrectAnswer = "";
        private int currentlySelectedOption = -1;
        private bool isMultiple = false;

        private void OnEnable()
        {
            GameManager.OnAskingQuestion += AskQuestion;
            QuestionManager.OnSettingDifficulty += SetDifficultyImage;
            QuestionManager.OnSettingCategory += SetCategory;
            QuestionManager.OnSettingQuestion += SetQuestion;
            QuestionManager.OnSettingAnswers += SetAnswers;
        }

        private void OnDisable()
        {
            GameManager.OnAskingQuestion -= AskQuestion;
            QuestionManager.OnSettingDifficulty -= SetDifficultyImage;
            QuestionManager.OnSettingCategory -= SetCategory;
            QuestionManager.OnSettingQuestion -= SetQuestion;
            QuestionManager.OnSettingAnswers -= SetAnswers;
        }

        public void ResetUI()
        {
            storedCorrectAnswer = "";
            answers.Clear();
            currentlySelectedOption = -1;
            isMultiple = false;
            AskQuestion(false);
        }

        private void AskQuestion(bool isActive)
        {
            gameQuestionCanvas.SetActive(isActive);
        }

        private void SetDifficultyImage(int difficulty)
        {
            if (difficulty < 0)
            {
                throw new System.Exception("NO DIFFICULTY LEVEL HAS BEEN SET");
            }

            difficultyImageSpot.sprite = difficultyImages[difficulty];
        }

        private void SetCategory(string categoryName)
        { 
            categoryTextSpot.text = categoryName;
        }

        private void SetQuestion(string question)
        {
            questionTextSpot.text = question;
        }

        private void SetAnswers(string correctAnswer, List<string> incorrectAnswers)
        {
            storedCorrectAnswer = correctAnswer;
            incorrectAnswers.ForEach(t => answers.Add(t));
            answers.Add(correctAnswer);

            //Determine if multiple or boolean and set answers
            isMultiple = incorrectAnswers.Count > 1;
            if (isMultiple)
            {
                booleanAnswerHolder.SetActive(false);
                multipleAnswerHolder.SetActive(true);

                for (int i = 0; i < multipleAnswerSpots.Count; i++)
                {
                    int chosenAnswer = Random.Range(0, answers.Count);
                    multipleAnswerSpots[i].text = answers[chosenAnswer];
                    answers.RemoveAt(chosenAnswer);
                }
            }
            else
            {
                booleanAnswerHolder.SetActive(true);
                multipleAnswerHolder.SetActive(false);
            }
        }

        public void PickAnswer(int answer)
        {
            currentlySelectedOption = answer;
        }

        public void LockInAnswer()
        {
            if (currentlySelectedOption == -1)
                return;

            bool correctlyGuessed = false;

            if (isMultiple)
            {
                correctlyGuessed = multipleAnswerSpots[currentlySelectedOption].text == storedCorrectAnswer;
            }
            else
            {
                correctlyGuessed = booleanAnswerSpots[currentlySelectedOption].text == storedCorrectAnswer;
            }

            GameManager.Instance.QuestionAnswered(correctlyGuessed);
            ResetUI();
        }
    }
}
