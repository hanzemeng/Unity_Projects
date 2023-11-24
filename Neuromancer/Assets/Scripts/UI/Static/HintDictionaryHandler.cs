using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class HintDictionaryHandler : MonoBehaviour {

    public static HintDictionaryHandler current;
    [SerializeField] private TextMeshProUGUI dictionaryText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private List<string> hints = new List<string> {
        "Switch to a birds-eye view by pressing the <color=red>F</color> key.",
        "Rotate the camera by holding down the <color=red>mouse scroll wheel or V</color>, then horizontally drag your mouse.",
        "<color=red>{spellButton}</color> casts a <color=red>Neuromancy</color> spell. Keep casting on an enemy until it turns green to <color=red>possess</color> it.",
        "<color=red>{commandButton}</color> on your allies to <color=red>select</color> them and <color=red>target</color> them towards enemies. Hold Shift or drag the cursor to select multiple allies.",
        "With allies selected, <color=red>{commandButton}</color> can also be used to interact with objects. Maybe your allies can help you with the crate.",
        "You can spellcast and control allies anywhere in camera view!",
        "If you want your allies to <b>stay in place</b>, press the Z key to issue a selected ally the <color=red><b>idle</b></color> command!",
        "Sharp units are capable of cutting down vine and bush obstacles",
        "Flying units are capable of crossing bottomless pits and flying to higher points on the map.",
        "Certain statues can be pushed by <color=red>hardy and blunt</color> units."
    };
    
    private int index;

    private void Awake() {
        if(current != null) {
            Destroy(gameObject);
            return;
        } else {
            current = this;
        }
    }

    private void Start() {
        SetIndex(0);
    }

    public void PreviousIndex() {
        if (index > 0) {
            SetIndex(index-1);
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
        }
    }

    public void NextIndex() {
        if (index < hints.Count - 1) {
            SetIndex(index+1);
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_GENERIC_CLICK_SFX);
        }
    }

    public void UpdateText() {
        SetIndex(index);
    }

    private void SetIndex(int newIndex) {
        index = newIndex;
        dictionaryText.text = Regex.Replace(hints[index], "{.*}", new MatchEvaluator(PopupHandler.ReplaceFunction));

        previousButton.interactable = index > 0;
        nextButton.interactable = index < hints.Count - 1;
    }

}
