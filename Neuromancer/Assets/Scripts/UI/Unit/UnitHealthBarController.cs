using UnityEngine;
using UnityEngine.UI;
using EmeraldAI;
using EmeraldAI.Example;

public class UnitHealthBarController : MonoBehaviour {

    [Header("Player HP UI Parameters")]
    [Tooltip("Size of Bar relative to each HP point. For no scaling, 0")]
    [SerializeField] private float pxPerPt = 2f;
    private float hpTweenTime = 0.25f;
    private float hpShadowTweenTime = 1f;
    [SerializeField] private bool moveWithCharacter = true;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private bool scaleBar = true;

    [Header("HP UI References")]
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider hpShadowSlider;
    
    private Tweener hpTweener;
    private Tweener hpShadowTweener;
    private float currentHealth;

    private Neuromancer.Unit unit;
    private EmeraldAISystem eaiSystem; // Only used for enemies
    private EmeraldAIPlayerHealth eaiPlayerHealth; // Only used for player
    private Image hpFill;
    private Image hpShadowFill;
    private Canvas healthCanvas; // Only used if moveWithCharacter enabled;
    private Camera cam;
    private bool takingDamage;
    
    private void Awake() {

        hpFill = hpSlider.fillRect.GetComponent<Image>();
        hpShadowFill = hpShadowSlider.fillRect.GetComponent<Image>();
        takingDamage = false;

        if (moveWithCharacter) {
            healthCanvas = hpBar.GetComponentInParent<Canvas>();
            cam = Camera.main;
        }

        hpTweener = new Tweener(this, x => hpSlider.value = x, () => takingDamage = false);
        hpShadowTweener = new Tweener(this, x => hpShadowSlider.value = x, () => takingDamage = false);
    }

    private void Start() {
        SetUnit(GetComponentInParent<Neuromancer.Unit>());

        if(isPlayer) {
            PlayerProgression.playerProgression.onChangeEvent.AddListener(Reset);
            eaiPlayerHealth?.DamageEvent.AddListener(SetTargetHealth);
            eaiPlayerHealth?.HealEvent.AddListener(SetTargetHealth);
        }
    }

    private void OnDestroy() {
        if(isPlayer) {
            PlayerProgression.playerProgression.onChangeEvent.RemoveListener(Reset);
            eaiPlayerHealth?.DamageEvent.RemoveListener(SetTargetHealth);
            eaiPlayerHealth?.HealEvent.RemoveListener(SetTargetHealth);
        }
    }

    public void SetUnit(Neuromancer.Unit unit) {
        eaiSystem?.DamageEvent.RemoveListener(SetTargetHealth);
        eaiSystem?.OnHealEvent.RemoveListener(SetTargetHealth);
        this.unit?.OnNewUnitTypeEvent.RemoveListener(SetUnitType);

        this.unit = unit;
        if(isPlayer) {
            eaiPlayerHealth = unit?.GetComponent<EmeraldAIPlayerHealth>();
        } else {
            eaiSystem = unit?.GetComponent<EmeraldAISystem>();
        }
        
        eaiSystem?.DamageEvent.AddListener(SetTargetHealth);
        eaiSystem?.OnHealEvent.AddListener(SetTargetHealth);
        unit?.OnNewUnitTypeEvent.AddListener(SetUnitType);

        Reset();
    }

    private void Reset() {
        if (isPlayer ? (eaiPlayerHealth != null) : (eaiSystem != null)) {
            int startingHealth = isPlayer ? eaiPlayerHealth.StartingHealth : eaiSystem.StartingHealth;

            if(scaleBar) {
                hpBar.sizeDelta = new Vector2(startingHealth * pxPerPt, hpBar.sizeDelta.y);
            }
            hpSlider.maxValue = startingHealth;
            hpShadowSlider.maxValue = startingHealth;
        }

        if (unit != null) {
            SetUnitType(true);
            SetTargetHealthInstant();
        }
    }

    private void SetTargetHealthInstant() {
        int targetHealth = isPlayer ? eaiPlayerHealth.CurrentHealth : eaiSystem.CurrentHealth;
        
        hpSlider.value = targetHealth;
        hpShadowSlider.value = targetHealth;

        currentHealth = targetHealth;
    }

    private void SetTargetHealth() {
        int targetHealth = isPlayer ? eaiPlayerHealth.CurrentHealth : eaiSystem.CurrentHealth;
        
        if (hpTweener != null && (targetHealth < currentHealth || !takingDamage)) {
            hpTweener.TweenWithTime(currentHealth, targetHealth, hpTweenTime, Tweener.QUAD_EASE_OUT);
            hpShadowTweener.TweenWithTime(currentHealth, targetHealth, hpShadowTweenTime, Tweener.QUAD_EASE_IN_OUT);
        }

        if(targetHealth < currentHealth) {
            takingDamage = true;
        }

        currentHealth = targetHealth;
    }

    private void SetUnitType(bool success) {
        if(unit != null && moveWithCharacter) {
            hpFill.color = UIConstants.GetColorFromUnitType(unit.transform);
            hpShadowFill.color = UIConstants.GetOppositeColorFromUnitType(unit.transform);
        }
    }

    private void LateUpdate() {
        if (moveWithCharacter) {
            healthCanvas.transform.rotation = cam.transform.rotation;
        }
    }
}