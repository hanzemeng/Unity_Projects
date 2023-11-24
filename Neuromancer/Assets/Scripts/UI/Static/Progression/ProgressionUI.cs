using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using EmeraldAI.Example;

public class ProgressionUI : MonoBehaviour {
    private PlayerProgression progression;
    public static ProgressionUI current;
    

    [SerializeField] private List<TextMeshProUGUI> neuronTexts;
    [SerializeField] private TextMeshProUGUI savedText;
    [SerializeField] private StatUpgradeUI hpUpgradeUI, hpRegenUpgradeUI, mpUpgradeUI, mpRegenUpgradeUI, alliesUpgradeUI;

    private Tweener neuronTextTweener;
    private TweenerColor savedTweener;

    private bool hasSetUp ;

    private void Awake() {
        if(current != null) {
            Destroy(this);
        } else {
            current = this;
        }
        hasSetUp = false;
    }

    private void Start() {
        progression = PlayerProgression.playerProgression;
        
        neuronTextTweener = new Tweener(this, x =>  { foreach (TextMeshProUGUI neuronText in neuronTexts) { neuronText.text = ((int) x).ToString(); }} );
        savedTweener = new TweenerColor(this, c => savedText.color = c);
        savedText.color = new Color(1f, 1f, 1f, 0f);

        progression.onChangeEvent.AddListener(Refactor);
        SaveLoadManager.saveLoadManager.onSaveEvent.AddListener(Saved);
        Refactor();
    }

    private void OnDestroy() {
        progression.onChangeEvent.RemoveListener(Refactor);
        SaveLoadManager.saveLoadManager.onSaveEvent.RemoveListener(Saved);
    }

    private void OnDisable() {
        savedText.color = new Color(1f, 1f, 1f, 0f);
    }

    private void Setup() {
        hasSetUp = true;

        hpUpgradeUI.Setup(PlayerController.player.GetComponent<EmeraldAIPlayerHealth>().StartingHealth, 
            progression.playerMaxHealthLevel, progression.playerMaxHealthEffect.Select((x) => (float) x).ToArray(), progression.playerMaxHealthCost);
        hpRegenUpgradeUI.Setup(PlayerController.player.GetComponent<EmeraldAIPlayerHealth>().healthRegenFrequency,
            progression.playerHealthRegenerationLevel, progression.playerHealthRegenerationEffect, progression.playerHealthRegenerationCost);
        mpUpgradeUI.Setup(PlayerController.player.GetComponent<UnitMagic>().maxMagic,
            progression.playerMaxManaLevel, progression.playerMaxManaEffect, progression.playerMaxManaCost);
        mpRegenUpgradeUI.Setup(PlayerController.player.GetComponent<UnitMagic>().magicRegen,
            progression.playerManaRegenerationLevel, progression.playerManaRegenerationEffect, progression.playerManaRegenerationCost);
        alliesUpgradeUI.Setup(UnitGroupManager.current.maxAllies,
            progression.playerMaxAllyCountLevel, progression.playerMaxAllyCountEffect.Select((x) => (float) x).ToArray(), progression.playerMaxAllyCountCost);

    }

    public void Refactor() {
        if(!hasSetUp) {
            Setup();
        }

        neuronTextTweener.TweenWithTime(float.Parse(neuronTexts[0].text), progression.skillPoint, 0.2f, Tweener.QUAD_EASE_OUT);

        hpUpgradeUI.Refactor(progression.playerMaxHealthLevel, progression.skillPoint);
        hpRegenUpgradeUI.Refactor(progression.playerHealthRegenerationLevel, progression.skillPoint);
        mpUpgradeUI.Refactor(progression.playerMaxManaLevel, progression.skillPoint);
        mpRegenUpgradeUI.Refactor(progression.playerManaRegenerationLevel, progression.skillPoint);
        alliesUpgradeUI.Refactor(progression.playerMaxAllyCountLevel, progression.skillPoint);
    }

    private void Saved() {
        StartCoroutine(SavedCoroutine());
    }

    private IEnumerator SavedCoroutine() {
        savedText.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        savedTweener.TweenWithTime(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f), 1f, Tweener.LINEAR);
    }
}
