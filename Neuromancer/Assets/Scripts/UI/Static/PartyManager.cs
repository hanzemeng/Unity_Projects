using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour {
    
    public static PartyManager current;

    [SerializeField] private List<HotkeyUI> hotkeys = new List<HotkeyUI>();

    [SerializeField] private Transform hotkeyDragCanvas;
    [SerializeField] private CanvasGroup failDropCanvas;
    [SerializeField] private Image background;
    [SerializeField] private GameObject partyText;

    private bool dragging;
    private RectTransform rectTransform;
    private int sourceHotkeyNum;
    private CanvasGroup sourceDragHotkey;
    private GameObject dragHotkey;
    private PlayerInputs inputs;

    // Spell #1: Spell from which the buff was dragged (or null if dragged from unset buffs)
    // Spell #2: Spell to which the buff was dropped (or null if dragged from unset buffs)
    [System.NonSerialized] public UnityEvent<int, int> onHotkeyDragEvent = new UnityEvent<int, int>();

    private const float baseHeight = 60;
    private const float hotKeyHeight = 75;

    private void Awake() {
        if (current == null) { current = this; }
        else { Destroy(gameObject); }
    }

    private void Start() {
        dragging = false;
        rectTransform = GetComponent<RectTransform>();
        
        inputs = PlayerInputManager.playerInputs;
        inputs.MenuAction.Enable();

        UnitGroupManager.current.OnNewMaxAllyCountEvent.AddListener(NewMaxAllyCount);
    }

    private void Update() {
        if(dragging) {
            dragHotkey.transform.position = inputs.MenuAction.CursorPosition.ReadValue<Vector2>();
        }
    }

    private void OnEnable() {
        PauseHandler.onPauseEvent.AddListener(FailedDrop);
        PauseHandler.onResumeEvent.AddListener(FailedDrop);
        UnitGroupManager.current?.OnNewMaxAllyCountEvent.AddListener(NewMaxAllyCount);
    }

    private void OnDisable() {
        PauseHandler.onPauseEvent.RemoveListener(FailedDrop);
        PauseHandler.onResumeEvent.RemoveListener(FailedDrop);
        UnitGroupManager.current.OnNewMaxAllyCountEvent.RemoveListener(NewMaxAllyCount);
    }

    private void NewMaxAllyCount(int newCount) {
        StopDragging();
        foreach (HotkeyUI hotkey in hotkeys) {
            hotkey.gameObject.SetActive(hotkey.GetGroupNumber <= newCount);
        }
        rectTransform?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight + newCount * hotKeyHeight);
    }

    public void Drag(int num, GameObject hotkey) {
        dragging = true;
        sourceHotkeyNum = num;

        dragHotkey = Instantiate(hotkey, hotkeyDragCanvas);
        dragHotkey.GetComponent<HotkeyUI>().enabled = false;
        dragHotkey.GetComponent<CanvasGroup>().blocksRaycasts = false;
        failDropCanvas.blocksRaycasts = true;
        
        sourceDragHotkey = hotkey.GetComponent<CanvasGroup>();
        sourceDragHotkey.alpha = 0f;
    }

    public void Drop(int num) {
        onHotkeyDragEvent.Invoke(sourceHotkeyNum, num);
        StopDragging();
    }

    private void StopDragging() {
        if(dragging) {
            dragging = false;
            sourceDragHotkey.alpha = 1f;
            sourceHotkeyNum = 0;
            Destroy(dragHotkey);
            dragHotkey = null;
            sourceDragHotkey = null;
            failDropCanvas.blocksRaycasts = false;
        }
    }

    public void FailedDrop() {
        if(dragging) {
            StopDragging();
        }
    }

    public void FailedDrop(bool _) {
        FailedDrop();
    }

    public void ActivateUI() {
        background.enabled = true;
        partyText.SetActive(true);
    }
}