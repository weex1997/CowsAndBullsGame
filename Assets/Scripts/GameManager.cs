using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;

[Serializable]
public enum GameMode
{
    Strike,
    Challeng,
    Puzzel
}

public class GameManager : MonoBehaviour, IDataPrisistence
{
    [Header("Game Mode")]
    public GameMode gameMode;

    [Header("Game Rule")]
    public int Lenght; // number of digits
    public int totalAttempts;

    [Header("Strike Mode")]
    //next stage
    [SerializeField] GameObject WinningEffect;
    [SerializeField] CanvasGroup StagesGroup;
    [SerializeField] TMP_Text StagesTextFade;
    [SerializeField] TMP_Text _StagesText;
    int Stage;
    int playerScoreTotal;
    bool stopWinningCorotoin = false;

    [Header("Score")]
    //player max score
    public int _PlayerScore = 10000;

    [Header("UI Elements")]
    [SerializeField] TMP_Text attemptText;
    [SerializeField] TMP_Text PlyerDebug;
    [SerializeField] TMP_Text PlyerName;

    [Header("Window Objects")]
    [SerializeField] GameObject Transtion;

    [Header("Parents Objects")]
    [SerializeField] GameObject InputParent;
    public GameObject Contact;

    [Header("Prefabs")]
    public GameObject PrintCAB;
    [SerializeField] GameObject InputPrefab;

    [Header("Scripts")]
    [SerializeField] keyboardManager keyboard;
    [SerializeField] TimerManager timerManager;

    [Header("Audio")]
    [SerializeField] AudioClip[] ClipEnterMadaButton;
    [SerializeField] AudioClip[] ClipFatorh;
    [SerializeField] AudioClip[] ClipWinn;
    [SerializeField] AudioClip[] Cliplose;


    [Header("Objects")]
    [SerializeField] GameObject lastChildObject;
    public GameObject Corsier;
    [SerializeField] GameObject ScrollView;
    [SerializeField] Transform spwanPoint;
    [SerializeField] GameObject HintTool;
    [SerializeField] GameObject Tutorial;


    public string attemptString;
    public string PuzzelAttempString;
    public bool itsWinning = false;
    public bool Winning = false;
    public bool itsLosing = false;
    public bool Losing = false;
    public bool newRandomNumbers = true;
    public bool HaveHint = false;
    public bool HaveHintRemove = false;
    public int currentAttempt = 0;

    public SerlizableDectionary<int, string> attemptData = new SerlizableDectionary<int, string>();


    [Header("Numbers to geuss")]
    public List<int> hiddenNumberList = new List<int>();
    string hiddenNumberString;

    //privets
    bool delayGuessButton = false;
    int cheackInputEmpty;
    int Rand;

    // Singleton instance.
    public static GameManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance , set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadData(GameData data)
    {
        attemptData = data.TrialData;
        hiddenNumberList = data.hiddenNumber;
        totalAttempts = data.attempts;
        timerManager.currentTime = data.time;
        JasonReader.Instance._stage = data.stage;
        HaveHint = data.HaveHint;
        HaveHintRemove = data.HaveHintRemove;
    }

    public void SaveData(ref GameData data)
    {
        data.TrialData = attemptData;
        data.hiddenNumber = hiddenNumberList;
        data.attempts = totalAttempts;
        data.time = timerManager.currentTime;
        data.stage = JasonReader.Instance._stage;
        data.HaveHint = HaveHint;
        data.HaveHintRemove = HaveHintRemove;
    }

    void Start()
    {
        gameMode = GameModeManager.Instance.gameMode;
        timerManager.StartStopwatch();
        AttemptUI(totalAttempts);

        if (gameMode == GameMode.Strike)
        {
            if (!DataPristinceManager.Instance.itsNewGame)
            {
                newRandomNumbers = false;
            }

            //print all saved trials
            for (int i = 0; i < attemptData.Count; i++)
            {
                var item = attemptData.ElementAt(i);
                currentAttempt = item.Key;
                attemptString = item.Value;
                GameObject ContactChild = Instantiate(PrintCAB, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                ContactChild.transform.SetParent(Contact.gameObject.transform, false);
                ContactChild.transform.localScale = new Vector3(1, 1, 1);
                ContactChild.GetComponent<PrintInput>()._attemptString = attemptString;
                ContactChild.GetComponent<PrintInput>()._currentAttempt = currentAttempt;
            }

            Stage = JasonReader.Instance._stage;
            _StagesText.text = "x" + (Stage + 1);
            Lenght = JasonReader.Instance.levels.level[Stage].Digits;
            if (DataPristinceManager.Instance.itsNewGame)
                totalAttempts = JasonReader.Instance.levels.level[Stage].Tries;

            StartTheGame();
            StagesFade();
            AttemptUI(totalAttempts);
            //active hint tools
            HintTool.SetActive(true);

            Tutorial.SetActive(true);

        }

        if (gameMode == GameMode.Challeng)
        {
            Lenght = 4;
            totalAttempts = 1000000;
            StartTheGame();

            //active hint tools
            HintTool.SetActive(false);

            Tutorial.SetActive(false);
        }

        if (gameMode == GameMode.Puzzel)
        {
            Lenght = 4;
            totalAttempts = 7;
            StartTheGame();

            for (int i = 0; i < 5; i++)
            {
                List<int> random = new List<int>();
                PuzzelAttempString = "";
                // give a random numbers
                for (int j = 0; j < Lenght; j++)
                {
                    int RandNum = Random.Range(0, 10);
                    while (random.Contains(RandNum))
                    {
                        RandNum = Random.Range(0, 10);
                    }
                    random.Add(RandNum);
                    PuzzelAttempString += RandNum.ToString();
                    if (PuzzelAttempString == hiddenNumberString)
                        j = 0;
                }
                if (random.Count == 4 && PuzzelAttempString != hiddenNumberString)
                {
                    printRandomGuessForPuzzelMode(PuzzelAttempString);
                }
            }

            //active hint tools
            HintTool.SetActive(false);

            Tutorial.SetActive(false);

        }
    }
    private void Update()
    {
        if (gameMode == GameMode.Strike)
        {
            //winnig
            while (itsWinning)
            {
                itsWinning = false;
                Winning = true;

                Stage++;
                if (Stage < JasonReader.Instance.levels.level.Length)
                {
                    JasonReader.Instance._stage = Stage;
                    stopWinningCorotoin = true;
                    StartCoroutine(WatingStages());
                }
                else
                {
                    Stage = 0;
                    JasonReader.Instance._stage = Stage;

                    SceneManager.LoadScene("WinningWindow", LoadSceneMode.Additive);
                }

                timerManager.StopStopwatch();
                playerScoreTotal = _PlayerScore;
                playerScoreTotal += (int)timerManager.currentTime * 4;
                Debug.Log("this the time minas" + (int)timerManager.currentTime * 4);

                if (PlayerPrefs.HasKey("PlayerScoreStrikeTotal"))
                {
                    playerScoreTotal += PlayerPrefs.GetInt("PlayerScoreStrikeTotal");
                    PlayerPrefs.SetInt("PlayerScoreStrikeTotal", playerScoreTotal);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.SetInt("PlayerScoreStrikeTotal", playerScoreTotal);
                    PlayerPrefs.Save();
                }


                Debug.Log("nextLevel");

                foreach (Transform child in Contact.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<Button>() != null)
                    {
                        child.GetComponent<Button>().enabled = false;
                    }
                }

            }

            //lose
            //losing rulles
            while (itsLosing)
            {
                itsLosing = false;
                Losing = true;

                Debug.Log("Start again");

                SceneManager.LoadScene("LoseingWindow", LoadSceneMode.Additive);

                timerManager.StopStopwatch();
                playerScoreTotal = PlayerPrefs.GetInt("PlayerScoreStrikeTotal");
                playerScoreTotal += (int)timerManager.currentTime * 4;
                Debug.Log("this the time minas" + (int)timerManager.currentTime * 4);

                //playerScore and high score
                int prevusScore = 0;
                if (PlayerPrefs.HasKey("PlayerScoreStrike"))
                {
                    prevusScore = PlayerPrefs.GetInt("PlayerScoreStrike");
                }
                else
                {
                    PlayerPrefs.SetInt("PlayerScoreStrike", 0);
                    PlayerPrefs.Save();
                }
                Debug.Log("prevuss" + prevusScore);
                if (prevusScore < PlayerPrefs.GetInt("PlayerScoreStrikeTotal"))
                {
                    PlayerPrefs.SetInt("PlayerScoreStrike", PlayerPrefs.GetInt("PlayerScoreStrikeTotal"));
                    PlayerPrefs.Save();
                }

                DataPristinceManager.Instance.DeleteData();

                Stage = 0;
                JasonReader.Instance._stage = Stage;
            }
        }

        if (gameMode == GameMode.Challeng)
        {
            //winnig
            while (itsWinning)
            {
                itsWinning = false;
                Winning = true;

                timerManager.StopStopwatch();
                _PlayerScore += (int)timerManager.currentTime * 4;
                Debug.Log("this the time minas" + (int)timerManager.currentTime * 4);

                foreach (Transform child in Contact.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<Button>() != null)
                    {
                        child.GetComponent<Button>().enabled = false;
                    }
                }

                SceneManager.LoadScene("WinningWindow", LoadSceneMode.Additive);
            }
        }

        if (gameMode == GameMode.Puzzel)
        {
            //winnig
            while (itsWinning)
            {
                itsWinning = false;
                Winning = true;

                timerManager.StopStopwatch();
                _PlayerScore -= (int)timerManager.currentTime * 4;

                foreach (Transform child in Contact.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<Button>() != null)
                    {
                        child.GetComponent<Button>().enabled = false;
                    }
                }

                SceneManager.LoadScene("WinningWindow", LoadSceneMode.Additive);

            }

            //lose
            //losing rulles
            while (itsLosing)
            {
                itsLosing = false;
                Losing = true;

                timerManager.StopStopwatch();

                SceneManager.LoadScene("LoseingWindow", LoadSceneMode.Additive);

                foreach (Transform child in Contact.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<Button>() != null)
                    {
                        child.GetComponent<Button>().enabled = false;
                    }
                }

            }
        }
    }

    //start the game after know the length to gnarate the numbers and the inputs
    void StartTheGame()
    {
        if (newRandomNumbers)
            randomNumbers();

        PlyerName.text = PlayerPrefs.GetString("PlayerName");
        StartCoroutine(transtion());

        //instantiate inputs
        for (int j = 0; j < Lenght; j++)
        {
            GameObject IndexObject = Instantiate(InputPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            IndexObject.transform.SetParent(InputParent.gameObject.transform, false);
            IndexObject.transform.localScale = new Vector3(1, 1, 1);
            IndexObject.name = "Input_" + j;
            keyboard.input.Add(IndexObject.GetComponent<TMP_Text>());
        }

    }

    #region RandomNumbers
    //cows & bulls genarate random numbers
    public void randomNumbers()
    {
        // give a random numbers
        for (int j = 0; j < Lenght; j++) // -> 4
        {
            Rand = Random.Range(0, 9); //-> 5
            while (hiddenNumberList.Contains(Rand))
            {
                Rand = Random.Range(0, 9);
            }
            hiddenNumberList.Add(Rand);
            Debug.Log(hiddenNumberList[j]);
        }
        foreach (int num in hiddenNumberList)
        {
            hiddenNumberString += num.ToString();
        }
    }
    #endregion

    #region Guess Button
    public void CheckCowsAndBulls()
    {

        attemptString = "";
        cheackInputEmpty = 0;

        foreach (TMP_Text input in keyboard.input)
        {
            if (input.text == "")
            {
                cheackInputEmpty++;
            }
        }

        if (cheackInputEmpty == 0 && delayGuessButton == false)
        {
            delayGuessButton = true;

            SoundManager.Instance.RandomSoundEffect(ClipEnterMadaButton);

            //restart corser postion
            Corsier.transform.position = new Vector2(keyboard.input[0].gameObject.transform.position.x, keyboard.input[0].gameObject.transform.position.y);


            //Scoring
            Debug.Log("Score: " + _PlayerScore);
            _PlayerScore -= 50;

            AttemptUI(totalAttempts -= 1);

            currentAttempt++;

            foreach (TMP_Text trial in keyboard.input)
            {
                attemptString += trial.text;
            }


            //save all trials on dectinary
            attemptData.Add(currentAttempt, attemptString);

            ScrollView.GetComponent<Transform>().position = new Vector3(ScrollView.GetComponent<Transform>().position.x, spwanPoint.position.y - 0.6f, ScrollView.GetComponent<Transform>().position.z);
            LeanTween.moveY(ScrollView, spwanPoint.position.y, 0.4f);

            StartCoroutine(Scroll());

            SoundManager.Instance.RandomSoundEffect(ClipFatorh);

            GameObject ContactChild = Instantiate(PrintCAB, new Vector3(0, 0, 0), Quaternion.identity);
            ContactChild.transform.SetParent(Contact.gameObject.transform, false);
            ContactChild.transform.localScale = new Vector3(1, 1, 1);
            ContactChild.GetComponent<PrintInput>()._attemptString = attemptString;
            ContactChild.GetComponent<PrintInput>()._currentAttempt = currentAttempt;
        }

        else
        {
            Debug.Log("You must enter 4 Digit");
        }
    }

    void printRandomGuessForPuzzelMode(string attemp)
    {
        currentAttempt++;
        AttemptUI(totalAttempts -= 1);

        GameObject ContactChild = Instantiate(PrintCAB, new Vector3(0, 0, 0), Quaternion.identity);
        ContactChild.transform.SetParent(Contact.gameObject.transform, false);
        ContactChild.transform.localScale = new Vector3(1, 1, 1);
        ContactChild.GetComponent<PrintInput>()._attemptString = attemp;
        ContactChild.GetComponent<PrintInput>()._currentAttempt = currentAttempt;
    }
    IEnumerator Scroll()
    {
        yield return new WaitForSeconds(1);
        delayGuessButton = false;
    }

    #endregion

    //scene transtion
    IEnumerator transtion()
    {
        bool start = true;
        while (start)
        {

            yield return new WaitForSeconds(0.7f);
            Transtion.SetActive(false);
            start = false;
        }

    }

    public void AttemptUI(int attempt)
    {
        totalAttempts = attempt;
        if (attemptText != null && gameMode != GameMode.Challeng)
            attemptText.text = attempt.ToString();
        if (attemptText != null && gameMode == GameMode.Challeng)
        {
            attemptText.text = "∞";
            attemptText.fontSize = 40;
        }

    }

    IEnumerator WatingStages()
    {
        while (stopWinningCorotoin)
        {
            stopWinningCorotoin = false;
            SoundManager.Instance.RandomSoundEffect(ClipWinn);
            WinningEffect.SetActive(true);
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
    }

    void StagesFade()
    {
        StagesTextFade.text = "Stage " + (JasonReader.Instance._stage + 1);
        StagesGroup.gameObject.SetActive(true);
        LeanTween.alphaCanvas(StagesGroup, 1, 2.5f);
        LeanTween.scale(StagesGroup.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 2.5f).setOnComplete(StagesAnimationFinish);

    }
    void StagesAnimationFinish()
    {
        StagesGroup.gameObject.SetActive(false);
    }
}