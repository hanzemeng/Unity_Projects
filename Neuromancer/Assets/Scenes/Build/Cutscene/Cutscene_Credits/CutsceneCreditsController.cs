using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCreditsController : MonoBehaviour {

    [SerializeField] private List<CanvasGroup> panelList;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float holdTime;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float gapTime;
    private CanvasGroup currentPanel;
    private int index;
    private bool isEnding;

    private Tweener panelTweener;
    private bool transitionDone;

    private void Awake() {
        isEnding = false;
        // Destroy(GameObject.Find("Essentials"));
    }

    private void Start() {
        panelTweener = new Tweener(this, x => currentPanel.alpha = x, () => transitionDone = true);

        index = 0;
        StartCoroutine(ShowPanelCoroutine());
    }

    private void Update() {
        if(Input.anyKey && !isEnding) {
            EndGame();
        }
    }

    private IEnumerator ShowPanelCoroutine() {
        if(index >= panelList.Count) {
            if(!isEnding) {
                EndGame();
            }
            yield break;
        }

        currentPanel = panelList[index];
        
        panelTweener.TweenWithTime(0f, 1f, fadeInTime, Tweener.LINEAR);
        while(!transitionDone) {
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        panelTweener.TweenWithTime(1f, 0f, fadeOutTime, Tweener.LINEAR);
        while(!transitionDone) {
            yield return null;
        }

        yield return new WaitForSeconds(gapTime);
        index++;
        StartCoroutine(ShowPanelCoroutine());
    }

    private void EndGame() {
        isEnding = true;
        LevelManager.levelManager.LoadLevel(LevelName.TITLE, 0);
    }
}
