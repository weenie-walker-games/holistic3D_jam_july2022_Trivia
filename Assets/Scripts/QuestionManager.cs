using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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

        string sessionToken = "";
        Coroutine sessionRoutine;
        Coroutine settingRoutine;
        Coroutine questionsRoutine;
        QuestionDataPull questions = null;
        QuestionData currentQuestion = null;
        int currentQuestionNumber = 0;
        int maxQuestions = 50;  //This comes from the API
        bool isGettingQuestions = false;

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

            if (questionsRoutine != null) StopCoroutine(questionsRoutine);

            questionsRoutine = StartCoroutine(GetQuestions(false));

        }

        private void SetAllQuestionInfo(bool isAsking)
        {
            if (settingRoutine != null) StopCoroutine(settingRoutine);

            settingRoutine = StartCoroutine(SettingQuestions(isAsking));

        }

        IEnumerator SettingQuestions(bool isAsking)
        {
            if (questions == null)
            {
                if (questionsRoutine != null) StopCoroutine(questionsRoutine);

                questionsRoutine = StartCoroutine(GetQuestions(isAsking));

                isGettingQuestions = true;
                yield return null;
            }

            while (isGettingQuestions)
            {
                yield return new WaitForSeconds(1f);
            }

            if (isAsking)
            {
                currentQuestion = questions.results[currentQuestionNumber];


                //Clear the incorrect answers
                incorrectAnswerList.Clear();


                //Get the conversions
                difficultyString = Encoding.UTF8.GetString(Convert.FromBase64String(currentQuestion.difficulty));
                questionType = Encoding.UTF8.GetString(Convert.FromBase64String(currentQuestion.type));
                categoryString = Encoding.UTF8.GetString(Convert.FromBase64String(currentQuestion.category));
                questionString = Encoding.UTF8.GetString(Convert.FromBase64String(currentQuestion.question));
                correctAnswerString = Encoding.UTF8.GetString(Convert.FromBase64String(currentQuestion.correct_answer));
                for (int i = 0; i < currentQuestion.incorrect_answers.Count; i++)
                {
                    incorrectAnswerList.Add(Encoding.UTF8.GetString(Convert.FromBase64String(currentQuestion.incorrect_answers[i])));
                }

                //Raise all the events to set the question data
                OnSettingCategory?.Invoke(categoryString);
                OnSettingDifficulty?.Invoke(difficultyString);
                OnSettingQuestion?.Invoke(questionString);
                OnSettingAnswers?.Invoke(correctAnswerString, incorrectAnswerList);

                currentQuestionNumber++;
                if (currentQuestionNumber == maxQuestions)
                    questions = null;
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
            return longestStreak;
        }

        IEnumerator GetQuestions(bool isAsking)
        {
            //Get a session token if one is not already stored
            if (sessionToken == "")
            {
                GetSessionToken();
                yield return new WaitForSeconds(0.1f);

                if (sessionToken == "") GameManager.Instance.NewGame();
            }

            string uri = $"https://opentdb.com/api.php?amount={maxQuestions}&encode=base64&token={sessionToken}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                //Request and wait for the desired page
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.InProgress:
                        break;
                    case UnityWebRequest.Result.Success:
                        string result = webRequest.downloadHandler.text;
                        questions = QuestionDataPull.CreateFromJSON(result);
                        break;
                    case UnityWebRequest.Result.ConnectionError:
                        GameManager.Instance.NewGame();
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        GameManager.Instance.NewGame();
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        GameManager.Instance.NewGame();
                        break;
                    default:
                        break;
                }

                isGettingQuestions = false;
                SetAllQuestionInfo(isAsking);
            }

            yield return new WaitForEndOfFrame();
        }

        private void GetSessionToken()
        {
            if (sessionRoutine != null) StopCoroutine(sessionRoutine);
            sessionRoutine = StartCoroutine(SessionToken("https://opentdb.com/api_token.php?command=request"));

        }

        IEnumerator SessionToken(string uri)
        {
            using(UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                //Request and wait for the desired page
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.InProgress:
                        break;
                    case UnityWebRequest.Result.Success:
                        string result = webRequest.downloadHandler.text;
                        SessionInfo sessionInfo = new SessionInfo();
                        sessionInfo = SessionInfo.CreateFromJSON(result);
                        sessionToken = sessionInfo.token;
                        break;
                    case UnityWebRequest.Result.ConnectionError:
                        GameManager.Instance.NewGame();
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        GameManager.Instance.NewGame();
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        GameManager.Instance.NewGame();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class QuestionDataPull
    {
        public int response_code;
        public List<QuestionData> results;

        public static QuestionDataPull CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<QuestionDataPull>(jsonString);
        }
    }

    [System.Serializable]
    public class QuestionData
    {
        public string category;
        public string type;
        public string difficulty;
        public string question;
        public string correct_answer;
        public List<string> incorrect_answers;
    }

    [System.Serializable]
    public class SessionInfo
    {
        public int response_code;
        public string response_message;
        public string token;

        public static SessionInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<SessionInfo>(jsonString);
        }
    }
}
