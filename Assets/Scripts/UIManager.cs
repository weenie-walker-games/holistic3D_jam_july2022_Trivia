using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WeenieWalker
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [Header("Question Related")]
        [SerializeField] GameObject gameQuestionCanvas; 

        [SerializeField] TMP_Text questionTextSpot;
        [SerializeField] Image difficultyImageSpot;
        [SerializeField] List<Sprite> difficultyImages = new List<Sprite>();
        string difficultyString;

        [SerializeField] TMP_Text categoryTextSpot;

        [SerializeField] GameObject booleanAnswerHolder;
        [SerializeField] GameObject multipleAnswerHolder;
        [SerializeField] List<TMP_Text> booleanAnswerSpots = new List<TMP_Text>();
        [SerializeField] List<TMP_Text> multipleAnswerSpots = new List<TMP_Text>();

        [Header("HUD Related")]
        [SerializeField] TMP_Text coinText;
        [SerializeField] TMP_Text heartText;
        [SerializeField] TMP_Text gemText;
        [SerializeField] GameObject messageCanvas;


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
            LootManager.OnUpdateCoins += SetCoins;
            LootManager.OnUpdateGems += SetGems;
            LootManager.OnUpdateHearts += SetHearts;
        }

        private void OnDisable()
        {
            GameManager.OnAskingQuestion -= AskQuestion;
            QuestionManager.OnSettingDifficulty -= SetDifficultyImage;
            QuestionManager.OnSettingCategory -= SetCategory;
            QuestionManager.OnSettingQuestion -= SetQuestion;
            QuestionManager.OnSettingAnswers -= SetAnswers;
            LootManager.OnUpdateCoins -= SetCoins;
            LootManager.OnUpdateGems -= SetGems;
            LootManager.OnUpdateHearts -= SetHearts;
        }

        private void Start()
        {
            messageCanvas.SetActive(false);

        }

        public void ResetQuestionUI()
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

        private void SetDifficultyImage(string difficulty)
        {
            difficultyString = difficulty;
            int difficultyAmount = ConvertDifficulty(difficulty);

            if (difficultyAmount < 0)
            {
                throw new System.Exception("NO DIFFICULTY LEVEL HAS BEEN SET");
            }

            difficultyImageSpot.sprite = difficultyImages[difficultyAmount];
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

            GameManager.OnAnsweredQuestion(correctlyGuessed, difficultyString);
            ResetQuestionUI();
        }


        private void SetCoins(int amount)
        {
            coinText.text = amount.ToString();
        }

        private void SetGems(int amount)
        {
            gemText.text = amount.ToString();
        }

        private void SetHearts(int amount, bool isLosingALife)
        {
            heartText.text = amount.ToString();
            if (isLosingALife)
                messageCanvas.SetActive(true);
        }


        private int ConvertDifficulty(string difficulty)
        {
            int result = -1;
            switch (difficulty)
            {
                case "hard":
                    result = 2;
                    break;
                case "medium":
                    result = 1;
                    break;
                case "easy":
                    result = 0;
                    break;
                default:
                    result = -1;
                    break;
            }

            return result;
        }

        public void CloseCanvas()
        {
            messageCanvas.SetActive(false);
            GameManager.Instance.MoveBackToDefault(false);
        }
    }
}
