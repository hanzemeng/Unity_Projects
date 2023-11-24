using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SpellMenuItem : MonoBehaviour {

    public Spell spell {get; private set;}
    private bool draggable;
    
    [SerializeField] private Image spellImage;
    [SerializeField] private TextMeshProUGUI spellName;
    [SerializeField] private List<Image> buffImages;

    [SerializeField] private Sprite emptyBuffSlot;
    
    private void Awake() {
        draggable = true;
    }

    private void OnEnable() {
        spell?.onBuffChangeEvent.AddListener(Refactor);
    }

    private void OnDisable() {
        spell?.onBuffChangeEvent.RemoveListener(Refactor);
    }

    public void SetSpell(Spell spell) {
        spell?.onBuffChangeEvent.RemoveListener(Refactor);
        this.spell = spell;
        spell.onBuffChangeEvent.AddListener(Refactor);
        Refactor(spell.GetBuffs());
    }

    private void Refactor(List<Buff> buffs) {
        if (spell == null) { return; }
        spellImage.sprite = spell.spellSpecs.sprite;
        spellName.text = spell.spellSpecs.name;
        spellImage.GetComponent<SpellMenuHover>().SetInfo(spell.spellSpecs.info);

        for(int i=0; i < buffImages.Count; i++) {
            Image buffImage = buffImages[i];

            if (i < buffs.Count) {
                buffImage.gameObject.SetActive(true);
                buffImage.sprite = buffs[i].icon;
                buffImage.GetComponent<SpellMenuHover>().SetInfo(buffs[i].description);
            } else if (i < buffs.Count + spell.GetBuffSlot()) {
                buffImage.gameObject.SetActive(true);
                buffImage.sprite = emptyBuffSlot;
                buffImage.GetComponent<SpellMenuHover>().ResetInfo();
                buffImage.sprite = emptyBuffSlot;
            } else {
                buffImage.gameObject.SetActive(false);
                buffImage.GetComponent<SpellMenuHover>().ResetInfo();
            }
        }
    }

    public void Drag(int i) {
        if(!draggable) {
            return;
        }

        List<Buff> buffs = spell.GetBuffs();
        if(i < buffs.Count) {
            SpellMenuManager.current.Drag(spell, buffs[i], buffImages[i]);
        }
    }

    public void Drop(int i) {
        if(!draggable) {
            return;
        }

        if (spell == null) {
            SpellMenuManager.current.DropOnUnset(i);
            return;
        }
        if(i >= spell.GetBuffs().Count) {
            SpellMenuManager.current.DropOnSpell(spell);
        } else {
            SpellMenuManager.current.FailedDrop();
        }
    }

    public void DisableDragging() {
        draggable = false;
    }
}