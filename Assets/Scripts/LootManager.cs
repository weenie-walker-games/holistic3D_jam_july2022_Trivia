using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeenieWalker
{
    public class LootManager : MonoSingleton<LootManager>
    {
        public static event System.Action<int> OnUpdateCoins;
        public static event System.Action<int> OnUpdateGems;
        public static event System.Action<int, bool> OnUpdateHearts;

        [SerializeField] int minCoinAmount = 1;
        [SerializeField] int maxCoinAmount = 5;
        public int Coins { get; private set; } = 0;
        public int Gems { get; private set; } = 0;
        private int hearts;
        [SerializeField] int maxHearts = 3;
        public int newCoinValue = 1;
        private bool correctlyAnswered = false;

        [SerializeField] List<LootChest> correctChests = new List<LootChest>();
        [SerializeField] List<LootChest> incorrectChests = new List<LootChest>();
        [SerializeField] AudioSource collectionSound;

        Coroutine pickRoutine;


        private void OnEnable()
        {
            GameManager.OnAskingQuestion += AskingQuestion;
            GameManager.OnAnsweredQuestion += QuestionAnswered;
            GameManager.OnNewGame += Reset;
            LootChest.OnLootChestPicked += LootChestPicked;
            LootChest.OnGetCoinsEarned += ReturnCoinsEarned;
        }



        private void OnDisable()
        {
            GameManager.OnAskingQuestion += AskingQuestion;
            GameManager.OnAnsweredQuestion -= QuestionAnswered;
            GameManager.OnNewGame -= Reset;
            LootChest.OnLootChestPicked -= LootChestPicked;
            LootChest.OnGetCoinsEarned -= ReturnCoinsEarned;
        }

        private void Reset()
        {
            hearts = maxHearts;
            Coins = 0;
            Gems = 0;

            OnUpdateCoins?.Invoke(Coins);
            OnUpdateGems?.Invoke(Gems);
            OnUpdateHearts?.Invoke(hearts, false);
        }

        private void AskingQuestion(bool isAsking)
        {
            if (isAsking)
            {
                ///At the moment, a correct answer DOES NOT spawn a mimic chest so this functionality is commented out

                //int correctRand = Random.Range(0, correctChests.Count);
                int incorrectRand = Random.Range(0, correctChests.Count);

                //Set all chests to false for isMimic
                correctChests.ForEach(c => c.IsMimic = false);
                incorrectChests.ForEach(c => c.IsMimic = false);

                //Then set the randomly picked one to true
                //correctChests[correctRand].IsMimic = true;
                incorrectChests[incorrectRand].IsMimic = true;
            }
        }

        private void QuestionAnswered(bool isCorrect, string difficulty)
        {
            newCoinValue = Random.Range(minCoinAmount, maxCoinAmount);
            correctlyAnswered = isCorrect;

            if (isCorrect)
            {
                newCoinValue += 2;

                switch (difficulty)
                {
                    case "easy":
                        Gems += 1;
                        newCoinValue *= 2;
                        break;
                    case "medium":
                        Gems += 5;
                        newCoinValue *= 3;
                        break;
                    case "hard":
                        Gems += 10;
                        newCoinValue *= 4;
                        break;
                    default:
                        break;
                }
            }

            OnUpdateGems?.Invoke(Gems);
        }

        private int ReturnCoinsEarned()
        {
            return newCoinValue;
        }

        private void LootChestPicked(bool isMimic)
        {
            if (pickRoutine != null) StopCoroutine(pickRoutine);

            pickRoutine = StartCoroutine(LootBoxSelection(isMimic));
        }

        IEnumerator LootBoxSelection(bool isMimic)
        {
            if (isMimic)
            {
                hearts -= 1;
                yield return new WaitForSeconds(1.5f);
                OnUpdateHearts?.Invoke(hearts, true);

                if (hearts < 0)
                    GameManager.OnGameEnd?.Invoke(true);

            }
            else
            {
                collectionSound.Play();
                Coins += newCoinValue;
                OnUpdateCoins?.Invoke(Coins);
                yield return new WaitForSeconds(3f);
                collectionSound.Stop();
                GameManager.Instance.MoveBackToDefault(correctlyAnswered);
            }
        }

    }
}
