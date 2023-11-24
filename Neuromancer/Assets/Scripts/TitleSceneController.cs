using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TitleSceneController : MonoBehaviour {
    [SerializeField] private GameObject deleteConfirmation;
    [SerializeField] private GameObject namePopup;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button newGameButton;
    [SerializeField] private CanvasGroup defaultGroup;
    [SerializeField] private CanvasGroup savesGroup;
    [SerializeField] private string defaultName = "Ray";

    private Tweener defaultGroupTweener;
    private Tweener savesGroupTweener;
    private PlayerInputs inputs;
    [HideInInspector] public UnityEvent onUpdateSaveSlots = new UnityEvent();

    private void Start() {
        defaultGroupTweener = new Tweener(this, x => defaultGroup.alpha = x);
        savesGroupTweener = new Tweener(this, x => savesGroup.alpha = x);

        defaultGroup.alpha = 1f;
        defaultGroup.blocksRaycasts = true;
        savesGroup.alpha = 0f;
        savesGroup.blocksRaycasts = false;

        inputs = PlayerInputManager.playerInputs;
        inputs.AllyAction.Disable();
        inputs.PlayerAction.Disable();
        inputs.CameraAction.Disable();

        Time.timeScale = 0f;
    }

    private void OnDestroy() {
        inputs.AllyAction.Disable();
        inputs.PlayerAction.Disable();
        inputs.CameraAction.Disable();

        Time.timeScale = 1f;
    }

    public void ShowSaveSlots() {
        StartCoroutine(ShowSaveSlotsCoroutine());
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    public void HideSaveSlots() {
        StartCoroutine(HideSaveSlotsCoroutine());
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    private IEnumerator ShowSaveSlotsCoroutine() {
        defaultGroup.blocksRaycasts = false;

        defaultGroupTweener.TweenWithTime(1f, 0f, 0.4f, Tweener.LINEAR);
        while(defaultGroupTweener.IsTweening()) {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        savesGroupTweener.TweenWithTime(0f, 1f, 0.4f, Tweener.LINEAR);
        while(savesGroupTweener.IsTweening()) {
            yield return null;
        }

        savesGroup.blocksRaycasts = true;
    }

    private IEnumerator HideSaveSlotsCoroutine() {
        savesGroup.blocksRaycasts = false;

        savesGroupTweener.TweenWithTime(1f, 0f, 0.4f, Tweener.LINEAR);
        while(defaultGroupTweener.IsTweening()) {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        defaultGroupTweener.TweenWithTime(0f, 1f, 0.4f, Tweener.LINEAR);
        while(defaultGroupTweener.IsTweening()) {
            yield return null;
        }

        defaultGroup.blocksRaycasts = true;
    }

    public void ShowDeleteConfirmation() {
        deleteConfirmation.SetActive(true);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    public void HideDeleteConfirmation() {
        deleteConfirmation.SetActive(false);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    public void ShowNamePopup() {
        namePopup.SetActive(true);
        nameField.text = defaultName;
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    public void HideNamePopup() {
        namePopup.SetActive(false);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    public void NewGame() {
        SaveLoadManager.saveLoadManager.CreateSaveFile(nameField.text);
        SaveLoadManager.saveLoadManager.LoadGame();
        LevelManager.levelManager.LoadLevel(LevelManager.levelManager.lastLevelName, LevelManager.levelManager.lastSpawnPointIndex);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    public void DeleteData() {
        SaveLoadManager.saveLoadManager.DeleteSaveData();
        onUpdateSaveSlots.Invoke();
        HideDeleteConfirmation();
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.SAVE_SFX);
    }

    public void CheckNewGameButton() {
        newGameButton.interactable = nameField.text.Length > 0 && nameField.text.Length <= 15;
    }

    public void PlayCredits() {
        LevelManager.levelManager.LoadLevel(LevelName.CUTSCENE_CREDITS, 0);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    public void QuitGame() {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}