using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeenieWalker
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static event Action<bool> OnAskingQuestion;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            OnAskingQuestion?.Invoke(true);
        }


        public void QuestionAnswered(bool isCorrect)
        {
            Debug.Log("Your guess is " + isCorrect);
        }
    }
}
