using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SpellMenuManager : MonoBehaviour {
    
    public static SpellMenuManager current;
    private PlayerInventory playerInventory;

    [SerializeField] private List<Image> unsetBuffImages;
    
    [SerializeField] private List<SpellMenuItem> spellMenuItems;
    [SerializeField] private List<SpellMenuItem> spellPreviewMenuItems;
    [SerializeField] private Transform spellDragCanvas;
    [SerializeField] private GameObject buffSlot;

    [SerializeField] private Sprite emptyBuffSlot;
    [SerializeField] private TextMeshProUGUI infoText;  
    [SerializeField] private string defaultInfo = "Hover over a Spell \n or Buff for Info...";  

    private int spellMenuIndex = 0;

    private bool dragging;
    private Spell sourceSpell;
    private Buff dragBuff;
    private GameObject maskBuffImage;
    private GameObject dragBuffImage;
    private PlayerInputs inputs;

    // Spell #1: Spell from which the buff was dragged (or null if dragged from unset buffs)
    // Spell #2: Spell to which the buff was dropped (or null if dragged from unset buffs)
    
    private void Awake() {
        if (current == null) { current = this; }
        else { Destroy(gameObject); }
    }

    private void Start() {
        playerInventory = PlayerInventory.current;
        playerInventory.onNewSpellEvent.AddListener(NewSpell);
        playerInventory.onInventoryReset.AddListener(ResetUI);
        playerInventory.onUnsetBuffChangeEvent.AddListener(RefactorUnset);
        PauseHandler.onPauseEvent.AddListener(FailedDrop);
        PauseHandler.onResumeEvent.AddListener(FailedDrop);
        dragging = false;
        
        inputs = PlayerInputManager.playerInputs;
        inputs.MenuAction.Enable();

        infoText.text = defaultInfo;

        foreach(SpellMenuItem spellMenuItem in spellMenuItems) {
            spellMenuItem.gameObject.SetActive(false);
        }

        foreach(SpellMenuItem spellMenuItem in spellPreviewMenuItems) {
            spellMenuItem.gameObject.SetActive(false);
            spellMenuItem.DisableDragging();
        }
        RefactorUnset(playerInventory.GetUnsetBuffs());
    }

    private void Update() {
        if(dragging) {
            dragBuffImage.transform.position = inputs.MenuAction.CursorPosition.ReadValue<Vector2>();
        }
    }

    private void OnEnable() {
        playerInventory?.onNewSpellEvent.AddListener(NewSpell);
        playerInventory?.onInventoryReset.AddListener(ResetUI);
        playerInventory?.onUnsetBuffChangeEvent.AddListener(RefactorUnset);
        PauseHandler.onPauseEvent.AddListener(FailedDrop);
        PauseHandler.onResumeEvent.AddListener(FailedDrop);
    }

    private void OnDisable() {
        playerInventory.onNewSpellEvent.RemoveListener(NewSpell);
        playerInventory.onInventoryReset.RemoveListener(ResetUI);
        playerInventory.onUnsetBuffChangeEvent.RemoveListener(RefactorUnset);
        PauseHandler.onPauseEvent.RemoveListener(FailedDrop);
        PauseHandler.onResumeEvent.RemoveListener(FailedDrop);
    }

    public void NewSpell(Spell spell) {
        SpellMenuItem spellMenuItem = spellMenuItems[spellMenuIndex];
        spellMenuItem.gameObject.SetActive(true);
        spellMenuItem.SetSpell(spell);

        spellMenuItem = spellPreviewMenuItems[spellMenuIndex];
        spellMenuItem.gameObject.SetActive(true);
        spellMenuItem.SetSpell(spell);

        spellMenuIndex++;
    }

    private void RefactorUnset(List<Buff> unsetBuffs) {
        for(int i=0; i < unsetBuffImages.Count; i++) {
            Image buffImage = unsetBuffImages[i];

            if (i < unsetBuffs.Count) {
                buffImage.sprite = unsetBuffs[i].icon;
                buffImage.GetComponent<SpellMenuHover>().SetInfo(unsetBuffs[i].description);
            } else {
                buffImage.sprite = emptyBuffSlot;
                buffImage.GetComponent<SpellMenuHover>().ResetInfo();
            }
        }
    }

    public void Drag(Spell spell, Buff buff, Image buffImage) {
        dragging = true;
        sourceSpell = spell;
        dragBuff = buff;

        maskBuffImage = Instantiate(buffSlot, spellDragCanvas);
        maskBuffImage.transform.position = buffImage.gameObject.transform.position;

        dragBuffImage = Instantiate(buffSlot, spellDragCanvas);
        dragBuffImage.GetComponent<Image>().sprite = buffImage.sprite;
    }

    public void DragFromUnset(int i) {
        List<Buff> buffs = playerInventory.GetUnsetBuffs();
        if(i < buffs.Count) {
            Drag(null, buffs[i], unsetBuffImages[i]);
        }
    }

    private void StopDragging() {
        dragging = false;
        sourceSpell = null;
        dragBuff = null;
        Destroy(maskBuffImage);
        Destroy(dragBuffImage);
    }

    public void FailedDrop() {
        if(dragging) {
            StopDragging();
        }
    }

    public void FailedDrop(bool _) {
        FailedDrop();
    }

    public void DropOnSpell(Spell targetSpell) {
        if(dragging) {
            playerInventory.BuffDragged(sourceSpell, targetSpell, dragBuff);
            StopDragging();
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_BUFF_SFX);
        }
    }

    public void DropOnUnset(int i) {
        if(dragging) {
            if(i >= playerInventory.GetUnsetBuffs().Count) {
                playerInventory.BuffDragged(sourceSpell, null, dragBuff);
                StopDragging();
                AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_DEBUFF_SFX);
            } else {
                FailedDrop();
            }
        }
    }

    public int MaxUnsetBuffs() { return unsetBuffImages.Count; }

    public void HoverInfo(string info) {
        infoText.text = info == "" ? defaultInfo : info;
    }

    public void UnhoverInfo() {
        infoText.text = defaultInfo;
    }

    private void ResetUI() {
        spellMenuIndex = 0;
        foreach(SpellMenuItem spellMenuItem in spellMenuItems) {
            spellMenuItem.gameObject.SetActive(false);
        }

        foreach(SpellMenuItem spellMenuItem in spellPreviewMenuItems) {
            spellMenuItem.gameObject.SetActive(false);
            spellMenuItem.DisableDragging();
        }

        for(int i=0; i < unsetBuffImages.Count; i++) {
            Image buffImage = unsetBuffImages[i];
            buffImage.sprite = emptyBuffSlot;
            buffImage.GetComponent<SpellMenuHover>().ResetInfo();
        }
    }
}