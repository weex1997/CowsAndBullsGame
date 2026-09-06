using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMnueButtons : MonoBehaviour
{
    [SerializeField] Animation anim;
    [SerializeField] AnimationClip creditanim;
    [SerializeField] AnimationClip creditanimClose;
    [SerializeField] GameObject CreditText;
    [SerializeField] AnimationClip HTPanimOpen;
    [SerializeField] AnimationClip HTPanimClose;
    [SerializeField] GameObject HTPText;
    [SerializeField] GameObject Transtion;

    [SerializeField] AudioClip[] ClipSettingsButtonOpen;
    [SerializeField] AudioClip[] ClipSettingsButtonClose;
    [SerializeField] AudioClip[] ClipStartGameButton;
    [SerializeField] AudioClip[] ClipChain;
    [SerializeField] AudioClip[] ClipMooingCow;

    [SerializeField] GameObject editname;
    [SerializeField] GameObject playerName;
    [SerializeField] GameObject editnameButton;
    [SerializeField] GameObject submitNameButton;

    public void addHint()
    {
        
    }
    public void addHintRemove()
    {
        
    }
    public void OpenWindow(GameObject window)
    {
        if (!window.activeSelf)
        {
            GameObject[] Windows = GameObject.FindGameObjectsWithTag("Window");
            foreach (GameObject windows in Windows)
            {
                if (windows != window)
                {
                    windows.SetActive(false);
                }
            }
            window.SetActive(true);
            LeanTween.scale(window, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutElastic);
            SoundManager.Instance.RandomSoundEffect(ClipSettingsButtonOpen);

            if (window.name == "SettingWindow")
            {
                PlayerDataManager.Instance.dataUbdated();
            }

        }
        else
        {
            //StartCoroutine(WindowDelay(window));
            LeanTween.scale(window, Vector3.zero, 0f).setEase(LeanTweenType.easeOutElastic);
            SoundManager.Instance.RandomSoundEffect(ClipSettingsButtonClose);
            window.SetActive(false);
        }
    }

    public void OpenWindowWithotAnimation(GameObject window)
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
    //IEnumerator WindowDelay(GameObject Window)
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    Window.SetActive(false);
    //}

    public void CloseWindows()
    {
        SoundManager.Instance.RandomSoundEffect(ClipSettingsButtonClose);
        GameObject[] Windows = GameObject.FindGameObjectsWithTag("Window");
        foreach (GameObject windows in Windows)
        {
            LeanTween.scale(windows, Vector3.zero, 0f).setEase(LeanTweenType.easeOutElastic);
            windows.SetActive(false);

        }
    }

    public void CloseWindowsWithotAnimation()
    {
        SoundManager.Instance.RandomSoundEffect(ClipSettingsButtonClose);
        GameObject[] Windows = GameObject.FindGameObjectsWithTag("Window");
        foreach (GameObject windows in Windows)
        {
            windows.SetActive(false);

        }
    }

    public void OpenScene(int sceneNum)
    {
        Transtion.SetActive(true);
        SoundManager.Instance.RandomSoundEffect(ClipStartGameButton);
        bool start = true;

        StartCoroutine(transtion(sceneNum, start));

    }
    IEnumerator transtion(int sceneNum, bool start)
    {
        while (start)
        {
            Transtion.SetActive(true);
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(sceneNum);
            Time.timeScale = 1;
            yield return new WaitForSeconds(1);
            Transtion.SetActive(false);
            start = false;
        }

    }
    public void MainMenuButtons(GameObject buttonImage)
    {
        buttonImage.gameObject.GetComponent<Image>().enabled = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void Credit()
    {
        if (HTPText.activeSelf)
        {
            StopCoroutine(HTPAnimation());
            anim.clip = HTPanimClose;
            anim.Play();
            HTPText.SetActive(false);
        }
        if (CreditText.activeSelf)
        {
            StopCoroutine(creditAnimation());
            anim.clip = creditanimClose;
            anim.Play();
            CreditText.SetActive(false);
        }
        else
        {
            anim.clip = creditanim;
            StartCoroutine(creditAnimation());

        }
    }
    IEnumerator creditAnimation()
    {
        anim.Play();
        yield return new WaitForSeconds(anim.clip.length);
        CreditText.SetActive(true);
    }
    public void HowToPlay()
    {
        if (CreditText.activeSelf)
        {
            StopCoroutine(creditAnimation());
            anim.clip = creditanimClose;
            anim.Play();
            CreditText.SetActive(false);
        }

        if (HTPText.activeSelf)
        {
            StopCoroutine(HTPAnimation());
            anim.clip = HTPanimClose;
            anim.Play();
            HTPText.SetActive(false);
        }
        else
        {
            anim.clip = HTPanimOpen;
            StartCoroutine(HTPAnimation());
        }
    }
    IEnumerator HTPAnimation()
    {
        anim.Play();
        yield return new WaitForSeconds(anim.clip.length);
        HTPText.SetActive(true);
    }

    public void EditNameButton()
    {
        playerName.SetActive(false);
        editname.SetActive(true);
        editnameButton.SetActive(false);
        submitNameButton.SetActive(true);

    }

    public void submitName()
    {
        playerName.SetActive(true);
        editname.SetActive(false);
        submitNameButton.SetActive(false);
        editnameButton.SetActive(true);
        PlayerDataManager.Instance.UpdateName();

    }
    public void ChainSound()
    {
        SoundManager.Instance.RandomSoundEffect(ClipChain);
    }
    public void MooingCows()
    {
        SoundManager.Instance.RandomSoundEffect(ClipMooingCow);
    }

    public void NewGame()
    {
        DataPristinceManager.Instance.NewGame();
        DataPristinceManager.Instance.itsNewGame = true;
        DataPristinceManager.Instance.runOnce = true;
        
    }

    public void Continue()
    {
        DataPristinceManager.Instance.LoadGame();
        DataPristinceManager.Instance.itsNewGame = false;
        DataPristinceManager.Instance.runOnce = true;
    }

    public void saveGameData()
    {
        DataPristinceManager.Instance.SaveGame();
    }

    public void startWindowRulls(GameObject cuntinue)
    {
        if (DataPristinceManager.Instance.hasData)
            cuntinue.SetActive(true);
        else
            cuntinue.SetActive(false);
    }

    public void GameMode(int gameModeIndex)
    {
        GameModeManager.Instance.gameMode = (GameMode)gameModeIndex;
    }
}
