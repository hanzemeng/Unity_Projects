using UnityEngine;
using System.Collections.Generic;
using Neuromancer;

public abstract class PlayerInteractable : MonoBehaviour {

    public static string PLAYER_INTERACTABLE_TAG = "Player Interactable";
    public static string PLAYER_INTERACTABLE_LAYER = "Player Interactable";

    [SerializeField] private float interactableRadius;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private RectTransform icon;

    [SerializeField] private float bobHeight = 0.2f;
    [SerializeField] private float bobPeriod = 2f;
    [SerializeField] private bool forceOutline;

    private Outline focusOutline;
    private bool inRange;
    private Camera cam;
    private Tweener canvasOpacityTweener;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    
    private void Awake() {
        focusOutline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();

        focusOutline.OutlineMode =  Outline.Mode.OutlineAll;
        focusOutline.OutlineWidth = UIConstants.outlineWidth;
        focusOutline.OutlineColor = UIConstants.defaultReticleColor;
    }

    protected void Start() {
        Focus(new List<GameObject>());
        sphereCollider.radius = interactableRadius;

        cam = Camera.main;
        canvas = icon.GetComponentInParent<Canvas>();
        canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasOpacityTweener = new Tweener(this, (x) => canvasGroup.alpha = x);

        inRange = (transform.position -  PlayerController.player.transform.position).magnitude <= interactableRadius;
        focusOutline.enabled = inRange;
        canvasGroup.alpha = inRange ? 1f : 0f;

        if(forceOutline) {
            focusOutline.enabled = true;
        } else {
            ReticleController.current.onUnitFocusChangeEvent.AddListener(Focus);
        }

        ReticleController.current.onPlayerInteractEvent.AddListener(HandleClick);
    }

    private void OnDestroy() {
        ReticleController.current.onUnitFocusChangeEvent.RemoveListener(Focus);
        ReticleController.current.onPlayerInteractEvent.RemoveListener(HandleClick);
    }

    private void Update() {
        if(inRange) {
            canvas.transform.rotation = cam.transform.rotation;
            icon.anchoredPosition = new Vector2(0, Mathf.Abs(bobHeight * Mathf.Sin(Time.time * Mathf.PI / bobPeriod)));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Unit.IsHero(other.transform))
        {
            inRange = true;
            canvasOpacityTweener.TweenWithSpeed(canvasGroup.alpha, 1f, 8f, Tweener.LINEAR);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(Unit.IsHero(other.transform))
        {
            inRange = false;
            canvasOpacityTweener.TweenWithSpeed(canvasGroup.alpha, 0f, 8f, Tweener.LINEAR);
        }
    }

    private void Focus(List<GameObject> focusedObjects) {
        if(!forceOutline) {
            focusOutline.enabled = focusedObjects.Contains(gameObject);
        }
    }

    private void HandleClick(GameObject go) {
        if(inRange && gameObject == go) {
            Trigger();
        }
    }

    protected abstract void Trigger();
}