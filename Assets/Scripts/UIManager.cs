using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WeenieWalker
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public static event System.Func<int> OnReturnQuestionStreak;

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
        [SerializeField] GameObject hudCanvas;
        [SerializeField] TMP_Text coinText;
        [SerializeField] TMP_Text heartText;
        [SerializeField] TMP_Text gemText;
        [SerializeField] TMP_Text streakText;

        [Header("MessageCanvases")]
        [SerializeField] GameObject lostHeartCanvas;
        [SerializeField] GameObject endGameCanvas;
        [SerializeField] TMP_Text totalGemsText;
        [SerializeField] TMP_Text totalCoinsText;
        [SerializeField] TMP_Text multiplierText;
        [SerializeField] List<Vector2> multiplierValues = new List<Vector2>();
        Coroutine endGameRoutine;

        private List<string> answers = new List<string>();
        private string storedCorrectAnswer = "";
        private int currentlySelectedOption = -1;
        private bool isMultiple = false;

        private void OnEnable()
        {
            GameManager.OnAskingQuestion += AskQuestion;
            GameManager.OnGameEnd += EndGame;
            GameManager.OnNewGame += ResetUIs;
            GameManager.OnNewGame += ResetQuestionUI;
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
            GameManager.OnGameEnd -= EndGame;
            GameManager.OnNewGame -= ResetUIs;
            GameManager.OnNewGame -= ResetQuestionUI;
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
            ResetUIs();
            ResetQuestionUI();

            SetStreakText();
        }

        private void ResetUIs()
        {

            lostHeartCanvas.SetActive(false);
            endGameCanvas.SetActive(false);
            hudCanvas.SetActive(true);
        }

        private void ResetQuestionUI()
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

            if (correctlyGuessed)
                SetStreakText();
        }

        private void SetStreakText()
        {
            int currentStreak = (int)OnReturnQuestionStreak?.Invoke();
            string streakTextValue = "x" + currentStreak.ToString();
            streakText.text = streakTextValue;

            streakText.gameObject.SetActive(currentStreak >0);
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
            {
                if(amount >= 0)
                    lostHeartCanvas.SetActive(true);
            }
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
            lostHeartCanvas.SetActive(false);
            GameManager.Instance.MoveBackToDefault(false);
        }

        private void EndGame(bool isGameOver)
        {
            hudCanvas.SetActive(false);

            int coins = LootManager.Instance.Coins;
            int gems = LootManager.Instance.Gems;
            totalCoinsText.text = coins.ToString();
            totalGemsText.text = gems.ToString();
            multiplierText.gameObject.SetActive(false);
            endGameCanvas.SetActive(true);

            var streak = OnReturnQuestionStreak?.Invoke();

            if (endGameRoutine != null) StopCoroutine(endGameRoutine);
            endGameRoutine = StartCoroutine(EndGameAnimation(coins, gems, (int)streak));

        }

        IEnumerator EndGameAnimation(int coins, int gems, int streak)
        {
            //Create the multiplier amount
            int pickedMultiplier = EvaluateStreakMulitplier(streak);
            multiplierText.text = "x" + pickedMultiplier.ToString();
            multiplierText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            //convert gems to coins

            if (gems > 0)
            {

                float tempGems = gems;
                float rate = tempGems / 2.0f;
                float tempCoins = coins;

                while (tempGems > 0)
                {
                    float amountReduced = rate * Time.deltaTime;
                    tempGems -= amountReduced;

                    tempCoins += amountReduced * pickedMultiplier;

                    totalGemsText.text = ((int)tempGems).ToString();
                    totalCoinsText.text = ((int)tempCoins).ToString();

                    yield return new WaitForEndOfFrame();
                }

            }
        }

        private int EvaluateStreakMulitplier(int streak)
        {
            int result = 1;

            for (int i = 0; i < multiplierValues.Count; i++)
            {
                if (streak >= multiplierValues[i].x)
                    result = (int) multiplierValues[i].y;
            }

            return result;

        }
    }
}
