using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsUserInput : MonoBehaviour
{
    public static SettingsUserInput settingsUserInput;

    public List<MouseInput> dimensionOptions;
    public List<MouseInput> rangeOptions;

    public MouseInput discardChanges;
    public MouseInput applyChanges;
    public TMP_InputField seedText;

    private void Awake()
    {
        if(null != settingsUserInput)
        {
            Destroy(gameObject);
            return;
        }
        settingsUserInput = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ListenDimensionOptionsInput()
    {
        for(int i=0; i<dimensionOptions.Count; i++)
        {
            int index = i;
            dimensionOptions[i].onClickEvent.AddListener(() => SettingsControl.settingsControl.OnDimensionOptionClick(index));
            dimensionOptions[i].transform.GetComponent<Collider>().enabled = true;
        }
    }
    public void StopListenDimensionOptionsInput()
    {
        for(int i=0; i<dimensionOptions.Count; i++)
        {
            dimensionOptions[i].onClickEvent.RemoveAllListeners();
            dimensionOptions[i].transform.GetComponent<Collider>().enabled = false;
        }
    }

    public void ListenRangeOptionsInput()
    {
        for(int i=0; i<rangeOptions.Count; i++)
        {
            int index = i;
            rangeOptions[i].onClickEvent.AddListener(() => SettingsControl.settingsControl.OnRangeOptionClick(index));
            rangeOptions[i].transform.GetComponent<Collider>().enabled = true;
        }
    }
    public void StopListenRangeOptionsInput()
    {
        for(int i=0; i<rangeOptions.Count; i++)
        {
            rangeOptions[i].onClickEvent.RemoveAllListeners();
            rangeOptions[i].transform.GetComponent<Collider>().enabled = false;
        }
    }

    public void ListenDiscardChangesInput()
    {
        discardChanges.onClickEvent.AddListener(() => SettingsControl.settingsControl.OnDiscardChangeClick());
        discardChanges.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenDiscardChangesInput()
    {
        discardChanges.onClickEvent.RemoveAllListeners();
        discardChanges.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenApplyChangesInput()
    {
        applyChanges.onClickEvent.AddListener(() => SettingsControl.settingsControl.OnApplyChangeClick());
        applyChanges.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenApplyChangesInput()
    {
        applyChanges.onClickEvent.RemoveAllListeners();
        applyChanges.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenSeedTextInput(bool changeComponent)
    {
        seedText.onSelect.AddListener((x) => SettingsControl.settingsControl.OnSeedTextSelect(x));
        seedText.onValueChanged.AddListener((x) => SettingsControl.settingsControl.OnSeedTextEdit(x));
        seedText.onEndEdit.AddListener((x) => SettingsControl.settingsControl.OnSeedTextEditEnd(x));
        if(changeComponent)
        {
            seedText.enabled = true;
        }
    }
    public void StopListenSeedTextInput(bool changeComponent)
    {
        seedText.onSelect.RemoveAllListeners();
        seedText.onValueChanged.RemoveAllListeners();
        seedText.onEndEdit.RemoveAllListeners();
        if(changeComponent)
        {
            seedText.enabled = false;
        }
    }
}
