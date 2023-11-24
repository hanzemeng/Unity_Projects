using UnityEngine;
using UnityEngine.UI;

public class MinimapToggle : MonoBehaviour {

    [SerializeField] private bool showing = true;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite showSprite;
    [SerializeField] private Sprite hideSprite;
    
    [SerializeField] private Vector3 showPosition;
    [SerializeField] private Vector3 hidePosition;
    private TweenerVector positionTweener;
    private RectTransform rectTransform;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        positionTweener = new TweenerVector(this, x => rectTransform.anchoredPosition = x);

        buttonImage.sprite = showing ? hideSprite : showSprite;
        rectTransform.anchoredPosition = showing ? showPosition : hidePosition;
    }

    public void ToggleMap() {
        showing = !showing;
        buttonImage.sprite = showing ? hideSprite : showSprite;
        positionTweener.TweenWithTime(rectTransform.anchoredPosition, showing ? showPosition : hidePosition, 0.25f, Tweener.QUAD_EASE_OUT);
    }
}