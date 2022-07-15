using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeenieWalker
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static event Action<bool> OnAskingQuestion;
        public static Action<bool, string> OnAnsweredQuestion;
        public static Action<bool> OnGameEnd;
        public static Action OnNewGame;


        [SerializeField] List<GameObject> moveToTimelines = new List<GameObject>();
        [SerializeField] List<GameObject> moveBackTimelines = new List<GameObject>();
        private GameObject currentTimelineActive = null;

        private void OnEnable()
        {
            OnAnsweredQuestion += QuestionAnswered;
            OnGameEnd += EndGame;
        }

        private void OnDisable()
        {
            OnAnsweredQuestion -= QuestionAnswered;
            OnGameEnd -= EndGame;
        }

        private void Start()
        {

        }

        private void AskQuestion()
        {
            OnAskingQuestion?.Invoke(true);
        }


        private void QuestionAnswered(bool isCorrect, string difficulty)
        {
            MoveToAnswer(isCorrect);
        }

        private void MoveToAnswer(bool isCorrect)
        {
            if (currentTimelineActive != null)
                currentTimelineActive.SetActive(false);

            int option = isCorrect ? 0 : 1;

            currentTimelineActive = moveToTimelines[option];
            currentTimelineActive.SetActive(true);
        }

        public void MoveBackToDefault(bool fromCorrect)
        {
            if (currentTimelineActive != null)
                currentTimelineActive.SetActive(false);

            int option = fromCorrect ? 0 : 1;

            currentTimelineActive = moveBackTimelines[option];
            currentTimelineActive.SetActive(true);

            Invoke("AskQuestion", 2f);
        }

        private void EndGame(bool isGameOver)
        {
            MoveBackToDefault(false);
        }

        public void NewGame()
        {
            OnNewGame?.Invoke();
            Invoke("AskQuestion", 1f);
        }
    }
}
