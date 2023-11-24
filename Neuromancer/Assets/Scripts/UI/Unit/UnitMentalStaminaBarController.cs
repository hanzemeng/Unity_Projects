using UnityEngine;
using UnityEngine.UI;
using Neuromancer;

public class UnitMentalStaminaBarController : MonoBehaviour {

    [Header("MS UI Parameters")]
    [Tooltip("Size of Bar relative to each MS point")]
    [SerializeField] private float pxPerPt = 2f;
    [SerializeField] private float msTweenTime = 0.25f;
    [SerializeField] private float msShadowTweenTime = 1f;
    [Tooltip("True for enemies, false for player")]
    [SerializeField] private bool moveWithCharacter = true;

    [Header("HP UI References")]
    [SerializeField] private RectTransform msBar;
    [SerializeField] private Slider msSlider;
    [SerializeField] private Slider msShadowSlider;
    
    private Tweener msTweener;
    private Tweener msShadowTweener;
    private float currentStamina;

    private UnitMentalStamina staminaController;
    private Canvas staminaCanvas; // Only used if moveWithCharacter enabled;
    private Camera cam;
    private Unit unit;
    
    private void Awake() {
        staminaController = GetComponentInParent<UnitMentalStamina>();
        if (moveWithCharacter) {
            staminaCanvas = msBar.GetComponentInParent<Canvas>();
            cam = Camera.main;
        }
        unit = GetComponentInParent<Unit>();
    }

    private void Start() {
        currentStamina = staminaController.GetMentalStamina();

        msBar.sizeDelta = new Vector2(staminaController.GetMaxMentalStamina() * pxPerPt, msBar.sizeDelta.y);
        msSlider.maxValue = staminaController.GetMaxMentalStamina();
        msShadowSlider.maxValue = staminaController.GetMaxMentalStamina();

        msTweener = new Tweener(this, x => msSlider.value = x, null);
        msShadowTweener = new Tweener(this, x => msShadowSlider.value = x, null);
        NewUnitType(true);
    }

    private void OnEnable() {
        staminaController.onStaminaDrainEvent.AddListener(SetTargetStamina);
        staminaController.onStaminaRechargeEvent.AddListener(SetTargetStamina);
        unit.OnNewUnitTypeEvent.AddListener(NewUnitType);
    }

    private void OnDisable() {
        staminaController.onStaminaDrainEvent.RemoveListener(SetTargetStamina);
        staminaController.onStaminaRechargeEvent.RemoveListener(SetTargetStamina);
        unit.OnNewUnitTypeEvent.RemoveListener(NewUnitType);
    }

    private void NewUnitType(bool success) {
        msBar.gameObject.SetActive(!Unit.IsAlly(unit.transform));
    }

    private void SetTargetStamina(float targetStamina) {
        msTweener.TweenWithTime(currentStamina, targetStamina, msTweenTime, Tweener.QUAD_EASE_OUT);
        msShadowTweener.TweenWithTime(currentStamina, targetStamina, msShadowTweenTime, Tweener.QUAD_EASE_IN_OUT);

        currentStamina = targetStamina;
    }

    private void LateUpdate() {
        if (moveWithCharacter) {
            staminaCanvas.transform.rotation = cam.transform.rotation;
        }
    }
}