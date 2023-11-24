using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputs playerInputs;
    public static PlayerInputManager current;

    [SerializeField] private Toggle switchClickToggle;
    [SerializeField] private TextMeshProUGUI controlsText1;
    [SerializeField] private TextMeshProUGUI controlsText2;
    public bool switchClick {get; private set;}

    private void Awake() {
        if (playerInputs == null) {
            playerInputs = new PlayerInputs();
            current = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        SetSwitchClick(false);
        switchClickToggle.isOn = switchClick;
    }

    public void ToggleSwitchClick() {
        SetSwitchClick(!switchClick);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
    }

    private void SetSwitchClick(bool switchClick) {
        this.switchClick = switchClick;
        UpdateControls();
    }

    private void UpdateControls() {
        if(switchClick) {
            playerInputs.PlayerAction.CastSpell.ApplyBindingOverride("<Mouse>/rightButton");
            playerInputs.AllyAction.AllyControl.ApplyBindingOverride("<Mouse>/leftButton");

            controlsText1.text = controlsText1.text.Replace("Cast Spell: [LMB]", "Cast Spell: [RMB]");
            controlsText1.text = controlsText1.text.Replace("Select Ally: [RMB]", "Select Ally: [LMB]");
            controlsText1.text = controlsText1.text.Replace("Select Mulitple: Drag [RMB]", "Select Mulitple: Drag [LMB]");
            controlsText1.text = controlsText1.text.Replace("Additive Select: [Shift] + [RMB]", "Additive Select: [Shift] + [LMB]");
            controlsText2.text = controlsText2.text.Replace("Target: [RMB]", "Target: [LMB]");
        } else {
            playerInputs.PlayerAction.CastSpell.ApplyBindingOverride(null);
            playerInputs.AllyAction.AllyControl.ApplyBindingOverride(null);

            controlsText1.text = controlsText1.text.Replace("Cast Spell: [RMB]", "Cast Spell: [LMB]");
            controlsText1.text = controlsText1.text.Replace("Select Ally: [LMB]", "Select Ally: [RMB]");
            controlsText1.text = controlsText1.text.Replace("Select Mulitple: Drag [LMB]", "Select Mulitple: Drag [RMB]");
            controlsText1.text = controlsText1.text.Replace("Additive Select: [Shift] + [LMB]", "Additive Select: [Shift] + [RMB]");
            controlsText2.text = controlsText2.text.Replace("Target: [LMB]", "Target: [RMB]");
        }

        HintDictionaryHandler.current.UpdateText();
        PopupHandler.current.UpdateString();
    }
}
