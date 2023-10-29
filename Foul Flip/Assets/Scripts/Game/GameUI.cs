using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI gameUI;

    public GameObject nodePrefab;

    public float nodesCanvasFadingTime;
    public Transform nodesParent;
    private float nodesParentScale;
    private List<GameObject> nodes;
    private List<TweenerColor> nodesColorsTweeners;

    public TMP_Text flipText;
    public TMP_Text timeText;
    public TMP_Text streakText;
    private TweenerColor flipTextTweener;
    private TweenerColor timeTextTweener;
    private TweenerColor streakTextTweener;

    public Image facebookIcon;
    public Image twitterIcon;
    private bool facebookIconShouldFlicker;
    private bool twitterIconShouldFlicker;

    public float solvedCanvasFadingTime;
    public Transform solvedParent;

    public TMP_Text seedText;

    public TMP_Text swipeText;

    private void Awake()
    {
        if(null != gameUI)
        {
            Destroy(gameObject);
            return;
        }
        gameUI = this;
        DontDestroyOnLoad(gameObject);

        flipTextTweener = new TweenerColor(this, x=>flipText.color=x);
        timeTextTweener = new TweenerColor(this, x=>timeText.color=x);
        streakTextTweener = new TweenerColor(this, x=>streakText.color=x);
    }

    public List<GameObject> CreateNodes(LevelInformation levelInformation)
    {
        int dimension = levelInformation.dimension;
        if(dimension*dimension != levelInformation.directionsInts.Count)
        {
            Debug.LogError($"In {levelInformation.name}, dimension of {dimension} but has {levelInformation.directionsInts.Count} nodes");
            return null;
        }

        List<Transform> oldChildren = new List<Transform>();
        for(int i=0; i<nodesParent.childCount; i++)
        {
            oldChildren.Add(nodesParent.GetChild(i));
        }
        foreach(Transform child in oldChildren)
        {
            Destroy(child.gameObject);
        }

        nodes = new List<GameObject>();

        float heightIncrement = -ScreenResolution.SCREEN_WIDTH/(float)dimension;
        float widthIncrement = ScreenResolution.SCREEN_WIDTH/(float)dimension;
        float currentHeight = ScreenResolution.SCREEN_WIDTH/2f + heightIncrement/2f;
        float initialWidth = -ScreenResolution.SCREEN_WIDTH/2f + widthIncrement/2f;
        float scaleRatio = 3f/(float)dimension;

        nodesParentScale = (ScreenResolution.SCREEN_WIDTH/2f * (1f-1f/3f) + 125f) / (currentHeight+125f*scaleRatio);

        for(int i=0; i<dimension; i++)
        {
            float currentWidth = initialWidth;
            for(int j=0; j<dimension; j++)
            {
                nodes.Add(Instantiate(nodePrefab, nodesParent));
                RectTransform rectTransform = nodes[nodes.Count-1].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(currentWidth, currentHeight);
                rectTransform.localScale = new Vector3(scaleRatio,scaleRatio,1f);
                currentWidth += widthIncrement;
            }
            currentHeight += heightIncrement;
        }
        

        nodesColorsTweeners = new List<TweenerColor>();
        for(int i=0; i<dimension*dimension; i++)
        {
            int mask = 0x00000001;
            int directionsInt = levelInformation.directionsInts[i];
            for(int j=0; j<DirectionUtility.DIRECTION_COUNT; j++)
            {
                if(0 != (directionsInt&mask))
                {
                    nodes[i].transform.GetChild(1+j).gameObject.SetActive(true); // +1 beacuse the first child is CircleOuter (background)
                }
                else
                {
                    nodes[i].transform.GetChild(1+j).gameObject.SetActive(false);
                }
                mask <<= 1;
            }

            nodes[i].transform.GetChild(0).GetComponent<Image>().color = NodeColorUtility.IntToColor(levelInformation.nodesValues[i]);

            int index = i;
            nodesColorsTweeners.Add(new TweenerColor(this, x=>nodes[index].transform.GetChild(0).GetComponent<Image>().color=x));
        }

        return nodes;
    }

    public void ChangeNodeColorTo(int nodeIndex, int nodeColorIndex)
    {
        ChangeNodeColorTo(nodeIndex, NodeColorUtility.IntToColor(nodeColorIndex));
    }
    public void ChangeNodeColorTo(int nodeIndex, Color nodeColor)
    {
        nodesColorsTweeners[nodeIndex].TweenWithTime(nodes[nodeIndex].transform.GetChild(0).GetComponent<Image>().color, nodeColor, 0.25f, Tweener.LINEAR);
    }

    public void ShootArrow(int sourceNodeIndex, int destinationNodeIndex)
    {
        StartCoroutine(ShootArrowCoroutine(sourceNodeIndex, destinationNodeIndex));
    }
    private IEnumerator ShootArrowCoroutine(int sourceNodeIndex, int destinationNodeIndex)
    {
        if(sourceNodeIndex == destinationNodeIndex)
        {
            yield break;
        }

        Vector3 sourcePosition = nodes[sourceNodeIndex].GetComponent<RectTransform>().localPosition;
        Vector3 destinationPosition = nodes[destinationNodeIndex].GetComponent<RectTransform>().localPosition;
        Vector3 positionOffset = destinationPosition - sourcePosition;

        RectTransform arrowTransform = Instantiate(nodes[sourceNodeIndex].transform.GetChild(1).gameObject, nodesParent).GetComponent<RectTransform>();
        arrowTransform.localRotation = Quaternion.Euler(new Vector3(0f,0f, -90f+Mathf.Atan2(positionOffset.y, positionOffset.x) * Mathf.Rad2Deg));
        arrowTransform.gameObject.SetActive(true);
        arrowTransform.localScale = nodes[sourceNodeIndex].transform.GetComponent<RectTransform>().localScale;
        Image arrowImage = arrowTransform.transform.GetComponent<Image>();
        Color sourceColor = arrowImage.color;
        Color destinationColor = new Color(arrowImage.color.r,arrowImage.color.g, arrowImage.color.b, 0f);

        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += 2f*Time.deltaTime;
            arrowTransform.localPosition = Vector3.Lerp(sourcePosition, destinationPosition, lerpAmount);
            arrowImage.color = Color.Lerp(sourceColor, destinationColor, lerpAmount);
            yield return null;
        }
        Destroy(arrowTransform.gameObject);
    }

    public float FadeOutNodesCanvasGroup()
    {
        StartCoroutine(FadeOutNodesCanvasGroupCoroutine());
        return nodesCanvasFadingTime;
    }
    private IEnumerator FadeOutNodesCanvasGroupCoroutine()
    {
        RectTransform rectTransform = nodesParent.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = nodesParent.GetComponent<CanvasGroup>();

        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += Time.deltaTime / nodesCanvasFadingTime;
            rectTransform.localScale = Vector3.Lerp(new Vector3(nodesParentScale,nodesParentScale,1f), new Vector3(4f,4f,4f), lerpAmount);
            canvasGroup.alpha = Mathf.Lerp(1f,0f,lerpAmount);
            yield return null;
        }
        rectTransform.localScale = new Vector3(3f,3f,3f);
        canvasGroup.alpha = 0f;
    }

    public float FadeInNodesCanvasGroup()
    {
        StartCoroutine(FadeInNodesCanvasGroupCoroutine());
        return nodesCanvasFadingTime;
    }
    private IEnumerator FadeInNodesCanvasGroupCoroutine()
    {
        RectTransform rectTransform = nodesParent.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = nodesParent.GetComponent<CanvasGroup>();

        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += Time.deltaTime / nodesCanvasFadingTime;
            rectTransform.localScale = Vector3.Lerp(new Vector3(4f,4f,4f), new Vector3(nodesParentScale,nodesParentScale,1f), lerpAmount);
            canvasGroup.alpha = Mathf.Lerp(0f,1f,lerpAmount);
            yield return null;
        }
        rectTransform.localScale = new Vector3(nodesParentScale,nodesParentScale,1f);
        canvasGroup.alpha = 1f;
    }

    public void ChangeFlipText(int newValue, Color startColor)
    {
        flipText.text = $"FLIP: {newValue}";
        flipTextTweener.TweenWithTime(startColor, new Color(1f,1f,1f,1f), 0.5f, Tweener.LINEAR);
    }
    public void ChangeTimeText(int newValue, Color startColor)
    {
        int hour = newValue/3600;
        newValue %= 3600;
        int minute = newValue/60;
        newValue %= 60;
        timeText.text = $"TIME: {hour}:{minute}:{newValue}";
        timeTextTweener.TweenWithTime(startColor, new Color(1f,1f,1f,1f), 0.5f, Tweener.LINEAR);
    }
    public void ChangeStreakText(int newValue, Color startColor)
    {
        streakText.text = $"STREAK: {newValue}";
        streakTextTweener.TweenWithTime(startColor, new Color(1f,1f,1f,1f), 0.5f, Tweener.LINEAR);
    }

    public void StartFlickerFacebookIcon()
    {
        facebookIconShouldFlicker = true;
        StartCoroutine(StartFlickerFacebookIconCoroutine());
    }
    public void StopFlickerFacebookIcon()
    {
        facebookIconShouldFlicker = false;
    }
    private IEnumerator StartFlickerFacebookIconCoroutine()
    {
        while(facebookIconShouldFlicker)
        {
            float lerpAmount = 0f;
            while(lerpAmount < 1f)
            {
                lerpAmount += 2f*Time.deltaTime;
                facebookIcon.color = Color.Lerp(new Color(1f,1f,1f,1f), new Color(0f,1f,0f,1f), lerpAmount);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            lerpAmount = 0f;
            while(lerpAmount < 1f)
            {
                lerpAmount += Time.deltaTime;
                facebookIcon.color = Color.Lerp(new Color(0f,1f,0f,1f), new Color(1f,1f,1f,1f), lerpAmount);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StartFlickerTwitterIcon()
    {
        twitterIconShouldFlicker = true;
        StartCoroutine(StartFlickerTwitterIconCoroutine());
    }
    public void StopFlickerTwitterIcon()
    {
        twitterIconShouldFlicker = false;
    }
    private IEnumerator StartFlickerTwitterIconCoroutine()
    {
        while(twitterIconShouldFlicker)
        {
            float lerpAmount = 0f;
            while(lerpAmount < 1f)
            {
                lerpAmount += 2f*Time.deltaTime;
                twitterIcon.color = Color.Lerp(new Color(1f,1f,1f,1f), new Color(0f,1f,0f,1f), lerpAmount);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            lerpAmount = 0f;
            while(lerpAmount < 1f)
            {
                lerpAmount += Time.deltaTime;
                twitterIcon.color = Color.Lerp(new Color(0f,1f,0f,1f), new Color(1f,1f,1f,1f), lerpAmount);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public float FadeOutSolvedCanvasGroup()
    {
        StartCoroutine(FadeOutSolvedCanvasGroupCoroutine());
        return solvedCanvasFadingTime;
    }
    private IEnumerator FadeOutSolvedCanvasGroupCoroutine()
    {
        RectTransform rectTransform = solvedParent.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = solvedParent.GetComponent<CanvasGroup>();

        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += Time.deltaTime / solvedCanvasFadingTime;
            canvasGroup.alpha = Mathf.Lerp(1f,0f,lerpAmount);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    public float FadeInSolvedCanvasGroup()
    {
        StartCoroutine(FadeInSolvedCanvasGroupCoroutine());
        return solvedCanvasFadingTime;
    }
    private IEnumerator FadeInSolvedCanvasGroupCoroutine()
    {
        RectTransform rectTransform = solvedParent.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = solvedParent.GetComponent<CanvasGroup>();

        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += Time.deltaTime / solvedCanvasFadingTime;
            rectTransform.localScale = Vector3.Lerp(new Vector3(3f,3f,1f), new Vector3(1f,1f,1f), lerpAmount);
            canvasGroup.alpha = Mathf.Lerp(0f,1f,lerpAmount);
            yield return null;
        }
        rectTransform.localScale = new Vector3(1f,1f,1f);
        canvasGroup.alpha = 1f;
    }

    public void ChangeSeedText(int newSeed)
    {
        seedText.text = $"SEED: {newSeed}";
    }

    public void AnimateSwipeText()
    {
        StartCoroutine(AnimateSwipeTextCoroutine());
    }
    private IEnumerator AnimateSwipeTextCoroutine()
    {
        string originalText = swipeText.text;
        string showingText;
        string frontInsertTextLeft = "<color=#00ff00ff>";
        string frontInsertTextRight = "<color=#ff0000ff>";
        string backInsertText = "</color>";
        int n = originalText.Length;
        int middleIndex =  originalText.IndexOf('-') + 2;
        int highlightSize = 4;
        int currentHighlightIndex;

        while(true)
        {
            currentHighlightIndex = middleIndex;
            while(currentHighlightIndex < n)
            {
                showingText = originalText;
                showingText = showingText.Insert(currentHighlightIndex, frontInsertTextRight);
                showingText = showingText.Insert(Mathf.Min(showingText.Length, currentHighlightIndex+frontInsertTextRight.Length+highlightSize), backInsertText);
                swipeText.text = showingText;
                currentHighlightIndex++;

                yield return new WaitForSeconds(0.1f);
            }
            showingText = originalText;
            swipeText.text = showingText;
            yield return new WaitForSeconds(3f);

            currentHighlightIndex = middleIndex;
            while(currentHighlightIndex >= 0)
            {
                showingText = originalText;
                showingText = showingText.Insert(currentHighlightIndex+1, backInsertText);
                showingText = showingText.Insert(Mathf.Max(0, currentHighlightIndex-highlightSize+1), frontInsertTextLeft);
                swipeText.text = showingText;
                currentHighlightIndex--;

                yield return new WaitForSeconds(0.1f);
            }
            showingText = originalText;
            swipeText.text = showingText;
            yield return new WaitForSeconds(3f);
        }
    }
}
