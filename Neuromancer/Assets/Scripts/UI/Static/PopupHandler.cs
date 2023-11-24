using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

public class PopupHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public static PopupHandler current;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    private Tweener opacityTweener;

    private IEnumerator autoHideCoroutine;
    public bool hoveringOver = false;
    private string currentString;

    public void OnPointerEnter(PointerEventData eventData) {
        hoveringOver = true;
        StopCoroutine(autoHideCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData) {
        hoveringOver = false;
        autoHideCoroutine = AutoHideCoroutine();
        StartCoroutine(autoHideCoroutine);
    }
    
    private void Awake() {
        if(null == current)
        {
            current = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start() {
        opacityTweener = new Tweener(this, x => canvasGroup.alpha = x);
        Hide();
    }

    public void Show(string s) {
        currentString = s;
        text.text = Regex.Replace(s, "{.*}", new MatchEvaluator(ReplaceFunction));

        opacityTweener.TweenWithTime(canvasGroup.alpha, 1f, 0.25f, Tweener.LINEAR);
        canvasGroup.blocksRaycasts = true;

        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_PING_SFX);

        if(autoHideCoroutine != null) {
            StopCoroutine(autoHideCoroutine);
        }
        
        autoHideCoroutine = AutoHideCoroutine();
        StartCoroutine(autoHideCoroutine);
    }

    public void UpdateString() {
        if(currentString != null) {
            text.text = Regex.Replace(currentString, "{.*}", new MatchEvaluator(ReplaceFunction));
        }
    }

    public static string ReplaceFunction(Match m) {
        switch(m.Value) {
            case "{spellButton}":
                return PlayerInputManager.current.switchClick ? "Right-click" : "Left-click";
            case "{commandButton}":
                return PlayerInputManager.current.switchClick ? "Left-click" : "Right-click";
            default:
                return m.Value;
        }
    }

    private IEnumerator AutoHideCoroutine() {
        yield return new WaitForSeconds(10f);
        Hide();
    }

    public void Hide() {
        opacityTweener.TweenWithTime(canvasGroup.alpha, 0f, 0.25f, Tweener.LINEAR);
        canvasGroup.blocksRaycasts = false;
    }

}
