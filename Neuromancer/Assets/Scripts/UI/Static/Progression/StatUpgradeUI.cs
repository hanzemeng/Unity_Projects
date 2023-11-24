using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUpgradeUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Slider currentSlider;
    [SerializeField] private Slider nextSlider;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private string label;
    [SerializeField] private bool inverse = false;

    private Tweener sliderTweener;
    private Tweener sliderShadowTweener;
    private CanvasGroup upgradeButtonGroup;

    private float startingValue;

    private float[] values;
    private int[] costs;

    private void Awake() {
        sliderTweener = new Tweener(this, (x) => currentSlider.value = x);
        sliderShadowTweener = new Tweener(this, (x) => nextSlider.value = x);
    }

    private void Start() {
        upgradeButtonGroup = upgradeButton.GetComponent<CanvasGroup>();
    }

    public void Setup(float currentValue, int level, float[] values, int[] costs) {
        startingValue = currentValue - (inverse ? -values.Take(level).Sum() : values.Take(level).Sum());
        currentSlider.maxValue = values.Length + 1;
        nextSlider.maxValue = values.Length + 1;
        nextSlider.value = 0;

        this.values = values;
        this.costs = costs;
    }

    public void Refactor(int level, int neurons) {
        levelText.text = "Lvl " + level;

        float currentValue = startingValue + (inverse ? -values.Take(level).Sum() : values.Take(level).Sum());
        labelText.text = label + ":  " + (inverse ? (1f/currentValue) : currentValue).ToString("0.##");
        sliderTweener?.TweenWithTime(currentSlider.value, level+1, 0.25f, Tweener.QUAD_EASE_OUT);

        if (level < values.Length) {
            float nextStep = inverse ? (1f/(currentValue-values[level]) - 1f/(currentValue)) : values[level];
            labelText.text += " <color=#FFCF00>+" + nextStep.ToString("0.##") + "</color>";
            // sliderShadowTweener.TweenWithTime(nextSlider.value, level+2, 0.25f, Tweener.QUAD_EASE_OUT);

            costText.text = costs[level].ToString();
            bool canAfford = neurons >= costs[level];
            costText.color = canAfford ? new Color(1f, 1f, 1f) : new Color(1f, 0f, 0f);
            upgradeButton.interactable = canAfford;
            upgradeButtonGroup.interactable = canAfford;
            upgradeButtonGroup.blocksRaycasts = canAfford;

            //maxLevelButton.SetActive(false);
        } else {
            // nextSlider.value = nextSlider.maxValue;
            upgradeButton.gameObject.SetActive(false);

            //maxLevelButton.SetActive(true);
        }
    }

    public void PlayUpgradeSound() {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_UPGRADE_SFX);
    }

    public void PlayDenySound() {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_DENY_SFX);
    }
}
