using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PauseHandler : MonoBehaviour {

    public static PauseHandler current;
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private TabButton currentTab;
    [SerializeField] private GameObject quitConfirmation;

    private PlayerInputs inputs;
    private bool paused;
    private float originalTimeScale;
    
    [System.NonSerialized] public static UnityEvent<bool> onPauseEvent = new UnityEvent<bool>();
    [System.NonSerialized] public static UnityEvent onResumeEvent = new UnityEvent();

    private void Awake() {
        if(current != null) {
            Destroy(gameObject);
            return;
        } else {
            current = this;
        }

        paused = false;
        inputs = PlayerInputManager.playerInputs;
        inputs.MenuAction.Enable();
        
        SwitchMenu(currentTab);
    }

    private void Start() {
        menuCanvas.enabled = false;
    }

    private void OnEnable() {
        inputs.MenuAction.Pause.performed += TogglePause;
    }

    private void OnDisable() {
        inputs.MenuAction.Pause.performed -= TogglePause;
    }

    public void Pause(bool isMainMenu) {
        if(!paused) {
            originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            if(isMainMenu) {
                menuCanvas.enabled = true;
            }

            onPauseEvent.Invoke(isMainMenu);
            paused = true;
            
            inputs.AllyAction.Disable();
            inputs.PlayerAction.Disable();
            inputs.CameraAction.Disable();

            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
        }
    }

    public void SwitchMenu(TabButton newTab) {
        currentTab?.GetTabFocus().SetActive(false);
        currentTab?.GetMenu().SetActive(false);

        currentTab = newTab;

        currentTab?.GetTabFocus().SetActive(true);
        currentTab?.GetMenu().SetActive(true);
    }

    public void Resume() {
        menuCanvas.enabled = false;
        Time.timeScale = originalTimeScale;
        if(paused) {
            onResumeEvent.Invoke();
        }
        paused = false;

        inputs.AllyAction.Enable();
        inputs.PlayerAction.Enable();
        inputs.CameraAction.Enable();
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    private void TogglePause(InputAction.CallbackContext ctx) {
        if (!paused) {
            Pause(true);
        } else {
            Resume();
        }
    }

    public void ShowQuitConfirmation() {
        quitConfirmation.SetActive(true);
    }

    public void HideQuitConfirmation() {
        quitConfirmation.SetActive(false);
    }

    public void Quit() {
        LevelManager.levelManager.LoadLevel(LevelName.TITLE, 0);
        AudioManager.instance.PlayBackgroundMusic(AudioManager.SoundResource.BGM_VILLIAGE);
        HideQuitConfirmation();
        Resume();
    }
}
