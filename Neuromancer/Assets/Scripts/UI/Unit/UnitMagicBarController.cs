using UnityEngine;
using UnityEngine.UI;

public class UnitMagicBarController : MonoBehaviour {

    [Header("Player MP UI Parameters")]
    [Tooltip("Size of Bar relative to each MP point")]
    [SerializeField] private float pxPerPt = 2f;
    [SerializeField] private float mpTweenTime = 0f;
    [SerializeField] private float mpShadowTweenTime = 0.5f;

    [Header("Player mp UI References")]
    [SerializeField] private RectTransform mpBar;
    [SerializeField] private Slider mpSlider;
    [SerializeField] private Slider mpShadowSlider;
    
    private Tweener mpTweener;
    private Tweener mpShadowTweener;
    private float currentMagic;
    private float shadowMagic;

    private UnitMagic magicController;
    
    private void Awake() {
        magicController = GetComponent<UnitMagic>();
        currentMagic = magicController.GetMagic();

        Reset();

        mpTweener = new Tweener(this, x => mpSlider.value = x, null);
        mpShadowTweener = new Tweener(this, x => mpShadowSlider.value = x, () => mpShadowSlider.value = 0);
    }

    private void Reset() {
        mpBar.sizeDelta = new Vector2(magicController.GetMaxMagic() * pxPerPt, mpBar.sizeDelta.y);
        mpSlider.maxValue = magicController.GetMaxMagic();
        mpShadowSlider.maxValue = magicController.GetMaxMagic();
        mpShadowSlider.value = 0;
    }

    private void Start() {
        magicController.onMagicDrainEvent.AddListener(SetTargetMagic);
        magicController.onMagicRechargeEvent.AddListener(SetTargetMagic);
        magicController.onBeginCastEvent.AddListener(BeginCast);
        magicController.onEndCastEvent.AddListener(EndCast);
        PlayerProgression.playerProgression.onChangeEvent.AddListener(Reset);
    }

    private void OnDestroy() {
        magicController.onMagicDrainEvent.RemoveListener(SetTargetMagic);
        magicController.onMagicRechargeEvent.RemoveListener(SetTargetMagic);
        magicController.onBeginCastEvent.RemoveListener(BeginCast);
        magicController.onEndCastEvent.RemoveListener(EndCast);
        PlayerProgression.playerProgression.onChangeEvent.RemoveListener(Reset);
    }

    private void SetTargetMagic(float targetMagic) {
        mpTweener.TweenWithTime(currentMagic, targetMagic, mpTweenTime, Tweener.JUMP);
        currentMagic = targetMagic;
    }

    private void BeginCast(float targetMagic) {
        currentMagic = targetMagic;
        shadowMagic = currentMagic;
        mpShadowSlider.value = shadowMagic;
    }

    private void EndCast(float targetMagic) {
        currentMagic = targetMagic;
        mpShadowTweener.TweenWithTime(shadowMagic, currentMagic, mpShadowTweenTime, Tweener.QUAD_EASE_OUT);
    }
}
