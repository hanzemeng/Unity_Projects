using UnityEngine;
using UnityEngine.EventSystems;

public class TransparentOnHover : EventTrigger {
    
    [SerializeField] private float defaultOpacity = 1f;
    [SerializeField] private float hoverOpacity = 0.5f;

    private CanvasGroup canvasGroup;
    private Tweener opacityTweener;

    void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        opacityTweener = new Tweener(this, x => canvasGroup.alpha = x);
    }

    public override void OnPointerEnter(PointerEventData data) {
        opacityTweener.TweenWithTime(canvasGroup.alpha, hoverOpacity, 0.25f, Tweener.LINEAR);
    }

    public override void OnPointerExit(PointerEventData data) {
        opacityTweener.TweenWithTime(canvasGroup.alpha, defaultOpacity, 0.25f, Tweener.LINEAR);
    }
}
