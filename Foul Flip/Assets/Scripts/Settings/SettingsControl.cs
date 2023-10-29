using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsControl : MonoBehaviour
{
    public static SettingsControl settingsControl;

    private bool isBusy;
    private bool isOpen;

    private const int TWO_BILLION = 2000000000;

    private int selectedDimensionIndex;
    private int selectedRangeIndex;
    private int selectedSeed;

    private SettingsInformation currentSettingsInformation;
    public const string CURRENT_SETTINGS_INFORMATION_SAVE_PATH = "currentSettingsInformation.asset";

    private void Awake()
    {
        if(null != settingsControl)
        {
            Destroy(gameObject);
            return;
        }
        settingsControl = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StopListenInputs();
    }

    public SettingsInformation GetCurrentSettings()
    {
        if(null == currentSettingsInformation)
        {
            string currentSettingsInformationSavePath = Path.Combine(Application.persistentDataPath, CURRENT_SETTINGS_INFORMATION_SAVE_PATH);
            string currentSettingsInformationString = "";
            if(File.Exists(currentSettingsInformationSavePath))
            {
                using(var stream = File.Open(currentSettingsInformationSavePath, FileMode.Open))
                {
                    using(var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        currentSettingsInformationString = reader.ReadString();
                    }
                }
            }
            currentSettingsInformation = SettingsInformation.Base16Decode(currentSettingsInformationString);
            if(null == currentSettingsInformation)
            {
                currentSettingsInformation = ScriptableObject.CreateInstance<SettingsInformation>();
                currentSettingsInformation.dimension = 3;
                currentSettingsInformation.range = 2;
                currentSettingsInformation.seed = UnityEngine.Random.Range(0, TWO_BILLION);
            }
        }
        return currentSettingsInformation;
    }

    private void OnApplicationQuit()
    {
        if(null == currentSettingsInformation)
        {
            return;
        }

        string currentSettingsInformationSavePath = Path.Combine(Application.persistentDataPath, CURRENT_SETTINGS_INFORMATION_SAVE_PATH);
        using (var stream = File.Open(currentSettingsInformationSavePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(currentSettingsInformation.Base16Encode());
            }
        }
    }

    public void OpenSettings()
    {
        StartCoroutine(OpenSettingsCoroutine());
    }
    private IEnumerator OpenSettingsCoroutine()
    {
        isOpen = true;
        isBusy = true;
        currentSettingsInformation = GetCurrentSettings();
        selectedDimensionIndex = currentSettingsInformation.dimension-3;
        selectedRangeIndex = currentSettingsInformation.range-2;
        selectedSeed = currentSettingsInformation.seed;

        SettingsUI.settingsUI.HighlightDimensionOption(selectedDimensionIndex);
        SettingsUI.settingsUI.HighlightRangeOption(selectedRangeIndex);
        SettingsUI.settingsUI.ChangeSeedText(selectedSeed.ToString());

        yield return new WaitForSeconds(SettingsUI.settingsUI.FadeInCanvasGroup());
        StartListenInputs();
        isBusy = false;
    }

    private void StartListenInputs()
    {
        SettingsUserInput.settingsUserInput.ListenDiscardChangesInput();
        SettingsUserInput.settingsUserInput.ListenApplyChangesInput();
        SettingsUserInput.settingsUserInput.ListenDimensionOptionsInput();
        SettingsUserInput.settingsUserInput.ListenRangeOptionsInput();
        SettingsUserInput.settingsUserInput.ListenSeedTextInput(true);

    }
    private void StopListenInputs()
    {
        SettingsUserInput.settingsUserInput.StopListenDiscardChangesInput();
        SettingsUserInput.settingsUserInput.StopListenApplyChangesInput();
        SettingsUserInput.settingsUserInput.StopListenDimensionOptionsInput();
        SettingsUserInput.settingsUserInput.StopListenRangeOptionsInput();
        SettingsUserInput.settingsUserInput.StopListenSeedTextInput(true);
    }

    public void OnDimensionOptionClick(int optionIndex)
    {
        if(isBusy)
        {
            return;
        }

        AudioManager.audioManager.PlayIconClick();
        selectedDimensionIndex = optionIndex;
        SettingsUI.settingsUI.HighlightDimensionOption(selectedDimensionIndex);
    }

    public void OnRangeOptionClick(int optionIndex)
    {
        if(isBusy)
        {
            return;
        }

        AudioManager.audioManager.PlayIconClick();
        selectedRangeIndex = optionIndex;
        SettingsUI.settingsUI.HighlightRangeOption(selectedRangeIndex);
    }

    public void OnSeedTextSelect(string str)
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
    }

    public void OnSeedTextEdit(string newSeed)
    {
        bool hasLetter = false;
        string newSeedTrimed = "";
        foreach(char c in newSeed)
        {
            if(!char.IsNumber(c))
            {
                hasLetter = true;
                continue;
            }
            newSeedTrimed += c;
        }
        
        if(!hasLetter)
        {
            AudioManager.audioManager.PlayKeyType();
        }

        if(newSeedTrimed.Length > 10)
        {
            newSeedTrimed = newSeedTrimed.Substring(newSeedTrimed.Length-10);
        }

        SettingsUserInput.settingsUserInput.StopListenSeedTextInput(false);
        SettingsUI.settingsUI.ChangeSeedText(newSeedTrimed);
        SettingsUserInput.settingsUserInput.ListenSeedTextInput(false);
    }

    public void OnSeedTextEditEnd(string newSeed)
    {
        long newSeedNumber;
        if("" == newSeed)
        {
            newSeedNumber = 0;
        }
        else
        {
            newSeedNumber = Int64.Parse(newSeed);
        }
        if(newSeedNumber >= TWO_BILLION)
        {
            newSeedNumber = TWO_BILLION - 1;
        }

        SettingsUserInput.settingsUserInput.StopListenSeedTextInput(false);
        SettingsUI.settingsUI.ChangeSeedText(newSeedNumber.ToString());
        SettingsUserInput.settingsUserInput.ListenSeedTextInput(false);
        selectedSeed = (int) newSeedNumber;
    }

    public void OnDiscardChangeClick()
    {
        if(isBusy)
        {
            return;
        }

        isOpen = false;
        AudioManager.audioManager.PlayIconClick();
        StopListenInputs();
        SettingsUI.settingsUI.FadeOutCanvasGroup();
        GameControl.gameControl.OnSettingsClose(false);
    }

    public void OnApplyChangeClick()
    {
        if(isBusy)
        {
            return;
        }

        isOpen = false;
        currentSettingsInformation.dimension = selectedDimensionIndex + 3;
        currentSettingsInformation.range = selectedRangeIndex + 2;
        currentSettingsInformation.seed = selectedSeed;

        AudioManager.audioManager.PlayIconClick();
        StopListenInputs();
        SettingsUI.settingsUI.FadeOutCanvasGroup();
        GameControl.gameControl.OnSettingsClose(true);
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
