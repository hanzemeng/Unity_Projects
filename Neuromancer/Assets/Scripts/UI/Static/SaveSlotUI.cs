using UnityEngine;
using TMPro;

public class SaveSlotUI : MonoBehaviour {
    [SerializeField] private int index;
    [SerializeField] private TitleSceneController titleSceneController;
    [SerializeField] private GameObject emptyGroup;
    [SerializeField] private GameObject usedGroup;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI lastSavedText;
    [SerializeField] private string emptyNameText = "Empty";
    [SerializeField] private string lastSavedPrefix = "Last Saved: ";

    private void Start() {
        titleSceneController.onUpdateSaveSlots.AddListener(Refactor);
        Refactor();
    }

    private void OnDestroy() {
        titleSceneController.onUpdateSaveSlots.RemoveListener(Refactor);
    }

    private void Refactor() {
        SaveLoadManager.saveLoadManager.SetActiveSaveFileIndex(index);

        if(SaveLoadManager.saveLoadManager.HasSaveFile()) {
            SaveLoadManager.saveLoadManager.LoadGame();
            emptyGroup.SetActive(false);
            usedGroup.SetActive(true);
            nameText.text = SaveLoadManager.saveLoadManager.activeSaveSlot.saveName;
            lastSavedText.text = lastSavedPrefix + SaveLoadManager.saveLoadManager.activeSaveSlot.saveDate;
            
        } else {
            emptyGroup.SetActive(true);
            usedGroup.SetActive(false);
            nameText.text = emptyNameText;
        }
    }

    public void NewGame() {
        SaveLoadManager.saveLoadManager.SetActiveSaveFileIndex(index);
        titleSceneController.ShowNamePopup();
    }

    public void DeleteData() {
        SaveLoadManager.saveLoadManager.SetActiveSaveFileIndex(index);
        titleSceneController.ShowDeleteConfirmation();
    }

    public void LoadGame() {
        SaveLoadManager.saveLoadManager.SetActiveSaveFileIndex(index);
        SaveLoadManager.saveLoadManager.LoadGame();
        LevelManager.levelManager.LoadLevel(LevelManager.levelManager.lastLevelName, LevelManager.levelManager.lastSpawnPointIndex);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }
}
