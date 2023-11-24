using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Neuromancer;
using TMPro;

public class HotkeyUI : MonoBehaviour {

    [Header("Group Information")]
    [SerializeField] private int groupNumber;
    public int GetGroupNumber {get {return groupNumber;} private set {}}

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private UnitIconUI unitIcon;
    [SerializeField] private Image borderGlow;
    [SerializeField] private GameObject disbandButton;
    private Image background;

    private RectTransform rectTransform;
    private PartyManager partyManager;
    [HideInInspector] public NPCUnit unit;
    private Button button;
    private ReticleController reticle;
    private UnitGroup selectedUnitGroup;
    private UnitOverheadIcons overheadIcons;
    
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        background = GetComponent<Image>();
        overheadIcons = GetComponent<UnitOverheadIcons>();
    }

    private void Start() {
        reticle = ReticleController.current;
        partyManager = PartyManager.current;
        numberText.text = groupNumber.ToString();

        UnitGroupManager.current.OnHotKeyUnitChange.AddListener(Refactor);
        selectedUnitGroup = UnitGroupManager.current.selectedUnitGroup;
        selectedUnitGroup.OnUnitChangeEvent.AddListener(RefactorOutline);

        Refactor(groupNumber, unit);
    }

    private void OnEnable() {
        UnitGroupManager.current?.OnHotKeyUnitChange.AddListener(Refactor);
        selectedUnitGroup?.OnUnitChangeEvent.AddListener(RefactorOutline);
    }

    private void OnDisable() {
        UnitGroupManager.current.OnHotKeyUnitChange.RemoveListener(Refactor);
        selectedUnitGroup?.OnUnitChangeEvent.RemoveListener(RefactorOutline);
    }

    private void Refactor(int index, NPCUnit unit) {
        if(index == groupNumber) {
            this.unit = unit;

            if (unit != null) {
                unitIcon.gameObject.SetActive(true);
                button.interactable = true;
                disbandButton.SetActive(true);
            } else {
                unitIcon.gameObject.SetActive(false);
                button.interactable = false;
                disbandButton.SetActive(false);
            }

            if (selectedUnitGroup != null) {
                RefactorOutline(selectedUnitGroup.units);
            }

            unitIcon.SetUnit(unit);
            overheadIcons.SetUnit(unit);
        }
    }

    private void RefactorOutline(List<Neuromancer.NPCUnit> units) {
        borderGlow.enabled = units.Contains(unit);
    }

    public void HandleClick() {
        UnitGroupManager.current.HandleHotKeySelectAt(groupNumber-1 );
    }

    public void HandleHoverEnter() {
        reticle.HotkeyFocusAction(unit);
    }

    public void HandleHoverExit() {
        reticle.HotkeyUnfocusAction(unit);
    }

    public void Drag() {
        if (unit != null) {
            partyManager.Drag(groupNumber, gameObject);
        }
    }

    public void Drop() {
        partyManager.Drop(groupNumber);
    }

    public void HandleDisbandButton() {
        if(unit != null) {
            UnitGroupManager.current.Disband(unit);
        }
    }
}