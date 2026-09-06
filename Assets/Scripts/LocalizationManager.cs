using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    public TMP_Dropdown dropDown;

    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        int index = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        dropDown.SetValueWithoutNotify(Mathf.Max(0, index));

        dropDown.onValueChanged.AddListener(OnLocaleChanged);
    }

    public void OnLocaleChanged(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}