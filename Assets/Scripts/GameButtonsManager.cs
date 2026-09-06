using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtonsManager : MonoBehaviour
{
    [SerializeField] AudioClip[] ClipSettingsButtonOpen;
    [SerializeField] AudioClip[] ClipSettingsButtonClose;
    [SerializeField] AudioClip[] ClipHomeButton;
    [SerializeField] AudioClip[] ClipStartGameButton;
    [SerializeField] AudioClip[] ClipXOFatorh;
    public Sprite X;

    public Button HintButton;
    public Button HintRemoveButton;

    public GameObject winningWindow;
    public GameObject LosingWindow;


    // Singleton instance.
    public static GameButtonsManager Instance = null;

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

    void Start()
    {
        if (GameManager.Instance.HaveHint)
            HintButton.interactable = false;
        if (GameManager.Instance.HaveHintRemove)
            HintRemoveButton.interactable = false;

    }

    public void Pause()
    {
        SceneManager.LoadScene("PauseWindow", LoadSceneMode.Additive);

        Time.timeScale = 0;
        SoundManager.Instance.RandomSoundEffect(ClipSettingsButtonOpen);
    }


    public void OpenWindowOrClose(GameObject window)
    {
        if (!window.activeSelf)
        {

            window.SetActive(true);

        }
        else
        {
            window.SetActive(false);

        }
    }
    public void CloseShareWindow(GameObject window)
    {
        window.SetActive(false);
        if (winningWindow != null)
        winningWindow.SetActive(true);
        if (LosingWindow != null)
            LosingWindow.SetActive(true);
        GameManager.Instance.Corsier.SetActive(true);

    }

    public void RnadomNumbers()
    {
        List<int> random = new List<int>();
        keyboardManager.Instance.DeleteALLButton();

        // give a random numbers
        for (int j = 0; j < keyboardManager.Instance.input.Count; j++) // -> 4
        {
            int Rand = Random.Range(0, 10); //-> 5
            while (random.Contains(Rand))
            {
                Rand = Random.Range(0, 10);
            }
            random.Add(Rand);
            keyboardManager.Instance.input[j].text = Rand.ToString();
            keyboardManager.Instance.numberBlockedcurr.Add(Rand.ToString());
            foreach (GameObject button in keyboardManager.Instance.Buttons)
            {
                if (keyboardManager.Instance.numberBlockedcurr.Contains(button.name))
                    button.GetComponent<Image>().color = new Color(0.93f, 0.33f, 0.23f, 0.5f);
                else
                    button.GetComponent<Image>().color = Color.white;
            }
        }

    }
    public void Hint()
    {
        HintButton.interactable = false;
        GameManager.Instance.HaveHint = true;
        if (PlayerPrefs.GetInt("Hint") > 0)
        {
            ShowHint();
            int hint = PlayerPrefs.GetInt("Hint") - 1;
            PlayerPrefs.SetInt("Hint", hint);
            PlayerPrefs.Save();
        }
    }
    public void ShowHint()
    {
        keyboardManager.Instance.DeleteALLButton();
        int Rand = Random.Range(0, keyboardManager.Instance.input.Count);

        keyboardManager.Instance.input[Rand].text = GameManager.Instance.hiddenNumberList[Rand].ToString();
        keyboardManager.Instance.numberBlockedcurr.Add(keyboardManager.Instance.input[Rand].text);

        foreach (GameObject button in keyboardManager.Instance.Buttons)
        {
            if (keyboardManager.Instance.numberBlockedcurr.Contains(button.name))
                button.GetComponent<Image>().color = new Color(0.93f, 0.33f, 0.23f, 0.5f);
            else
                button.GetComponent<Image>().color = Color.white;
        }
    }
    public void HintRemoveNumbers()
    {

        HintRemoveButton.interactable = false;
        GameManager.Instance.HaveHintRemove = true;
        if (PlayerPrefs.GetInt("HintRemove") > 0)
        {
            ShowHintRemoveNumbers();
            int hint = PlayerPrefs.GetInt("HintRemove") - 1;
            PlayerPrefs.SetInt("HintRemove", hint);
            PlayerPrefs.Save();
        }
    }
    public void ShowHintRemoveNumbers()
    {
        int Rand = Random.Range(0, 10); //-> 5
        while (GameManager.Instance.hiddenNumberList.Contains(Rand))
        {
            Rand = Random.Range(0, 10);
        }

        SoundManager.Instance.RandomSoundEffect(ClipXOFatorh);

        foreach (GameObject numText in keyboardManager.Instance.Buttons)
        {
            if (numText.transform.name == Rand.ToString())
            {
                Debug.Log("XXXXX");
                numText.transform.GetChild(1).GetComponent<Image>().enabled = true;
                numText.transform.GetChild(1).GetComponent<Image>().sprite = X;
            }
        }
    }
}
