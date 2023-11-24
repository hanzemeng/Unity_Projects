using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellBarManager : MonoBehaviour {
    public static SpellBarManager current;

    private PlayerSpellCast spellHandler;
    private int spellIndex;
    private Image currentSpellImage;
    private PlayerInventory playerInventory;

    [SerializeField] private List<Image> spellImages;
    [SerializeField] private List<Spell> spells;
    [SerializeField] private Color activeColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f);
    
    private void Awake() {
        if (current == null) { current = this; }
        else { Destroy(gameObject); }

        spellIndex = 0;
        spells = new List<Spell>();
    }

    private void Start() {
        foreach(Image image in spellImages) {
            image.color = inactiveColor;
            image.transform.parent.gameObject.SetActive(false);
        }

        playerInventory = PlayerInventory.current;
        spellHandler = PlayerController.player.GetComponent<PlayerSpellCast>();

        playerInventory.onNewSpellEvent.AddListener(NewSpell);
        playerInventory.onInventoryReset.AddListener(ResetUI);
        spellHandler.onPlayerSwitchSpell.AddListener(SetSpell);
    }

    private void OnEnable() {
        playerInventory?.onNewSpellEvent.AddListener(NewSpell);
        playerInventory?.onInventoryReset.AddListener(ResetUI);
        spellHandler?.onPlayerSwitchSpell.AddListener(SetSpell);
    }

    private void OnDisable() {
        playerInventory.onNewSpellEvent.RemoveListener(NewSpell);
        playerInventory.onInventoryReset.RemoveListener(ResetUI);
        spellHandler.onPlayerSwitchSpell.RemoveListener(SetSpell);
    }

    private void NewSpell(Spell spell) {
        Image image = spellImages[spellIndex];
        image.sprite = spell.spellSpecs.sprite;
        image.transform.parent.gameObject.SetActive(true);
        spells.Add(spell);

        spellIndex++;
    }

    private void SetSpell(SpellScriptableObject spellSpecs) {
        if(currentSpellImage != null) {
            currentSpellImage.color = inactiveColor;
        }

        for(int i = 0; i < spells.Count; i++) {
            if(spells[i].spellSpecs == spellSpecs) {
                currentSpellImage = spellImages[i];
                currentSpellImage.color = activeColor;
            }
        }
    }

    public void SpellFrameClick(int index) {
        spellHandler.SelectSpell(index);
    }

    private void ResetUI() {
        spells.Clear();
        spellIndex = 0;
        foreach(Image image in spellImages) {
            image.color = inactiveColor;
            image.transform.parent.gameObject.SetActive(false);
        }
    }
}