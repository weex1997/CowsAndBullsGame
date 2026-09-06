using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Totorilas : MonoBehaviour
{
    public List<GameObject> TutorialsGameObjects = new List<GameObject>();

    public GameObject Tutorial;
    public GameObject Tutorial_circle;
    public GameObject Tutorial_square;
    public TMP_Text Tutorial_text;
    public GameObject finger;
    public GameObject dark_background;
    public TimerManager timerManager;
    public GameObject Attempt;

    const string Table = "Tutorial";
    const int LineCount = 24;

    int clickNum = 0;
    bool ready;

    void OnEnable()
    {
        Time.timeScale = 0;
        timerManager.StopStopwatch();
        Attempt.SetActive(true);
        Attempt.GetComponent<RectTransform>().SetAsLastSibling();
        finger.SetActive(false);
        Tutorial_circle.SetActive(false);
        Tutorial_square.SetActive(false);
        dark_background.SetActive(true);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        StartCoroutine(Init());
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        Attempt.SetActive(false);
        Time.timeScale = 1;
        timerManager.StartStopwatch();
        clickNum = 0;
        ready = false;
    }

    IEnumerator Init()
    {
        yield return LocalizationSettings.InitializationOperation;
        RefreshText();
        ready = true;
    }

    void OnLocaleChanged(Locale locale) => RefreshText();

    string Line(int i) =>
        LocalizationSettings.StringDatabase.GetLocalizedString(Table, $"line_{i:00}");

    void RefreshText()
    {
        Tutorial_text.text = (clickNum == 0)
            ? LocalizationSettings.StringDatabase.GetLocalizedString(Table, "intro")
            : Line(clickNum - 1);

        ApplyDirection();
    }

    void ApplyDirection()
    {
        bool rtl = LocalizationSettings.SelectedLocale != null
                && LocalizationSettings.SelectedLocale.Identifier.Code == "ar";
        
        Tutorial_text.isRightToLeftText = rtl;
        Tutorial_text.alignment = rtl ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
        
    }

    void Update()
    {
        if (!ready) return;

        if (Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            dark_background.SetActive(false);
            finger.SetActive(true);

            if (TutorialsGameObjects.Count > clickNum)
            {
                RectTransform target = TutorialsGameObjects[clickNum].GetComponent<RectTransform>();
                bool isRound = target.sizeDelta.x - target.sizeDelta.y < 10;

                Tutorial_circle.SetActive(isRound);
                Tutorial_square.SetActive(!isRound);

                GameObject shape = isRound ? Tutorial_circle : Tutorial_square;
                shape.GetComponent<RectTransform>().sizeDelta = target.sizeDelta * 1.15f;

                Tutorial_text.text = Line(clickNum);
                Tutorial.transform.position = TutorialsGameObjects[clickNum].transform.position;
                clickNum++;
            }
            else if (LineCount > clickNum)
            {
                finger.SetActive(false);
                Tutorial_circle.SetActive(false);
                Tutorial_square.SetActive(false);
                dark_background.SetActive(true);
                Tutorial_text.text = Line(clickNum);
                clickNum++;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}