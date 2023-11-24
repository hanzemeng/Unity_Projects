using UnityEngine;
using UnityEngine.SceneManagement;

public class Altar : PlayerInteractable {

    private Canvas altarMenuCanvas;
    private static string ALTAR_MENU_CANVAS_TAG = "Altar Menu Canvas";
    private bool active;
    [SerializeField] private int associatedSpawnPointIndex; 

    private new void Start() {
        base.Start();
        active = false;
        altarMenuCanvas = GameObject.FindWithTag(ALTAR_MENU_CANVAS_TAG).GetComponent<Canvas>();

        PauseHandler.onResumeEvent.AddListener(Resume);
    }

    private void OnDestroy() {
        PauseHandler.onResumeEvent.RemoveListener(Resume);
    }

    protected override void Trigger() {
        PauseHandler.current.Pause(false);
        altarMenuCanvas.enabled = true;
        active = true;
        ProgressionUI.current.Refactor();
        if(LevelManager.levelManager != null) {
            LevelManager.levelManager.lastLevelName = SceneManager.GetActiveScene().name;
            LevelManager.levelManager.lastSpawnPointIndex = associatedSpawnPointIndex;
        }
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.ALTAR_SFX);
    }

    private void Resume() {
        if(active) {
            altarMenuCanvas.enabled = false;
            active = false;
        }
        // AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.ALTAR_SFX);
    }
}