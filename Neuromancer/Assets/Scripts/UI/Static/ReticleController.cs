using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Neuromancer;

using static Neuromancer.Unit;
using static Interactable;
using static PlayerInteractable;

public class ReticleController : MonoBehaviour {

    public static ReticleController current;
    public static string GROUND_LAYER = "Ground";
    public static string DEATH_ZONE_LAYER = "DeathZone";

    [Header("UI Parameters")]
    [SerializeField] private Vector2 reticleOffset = new Vector2(0f, 0.25f);
    [SerializeField] private Vector3 reticleUnitHeightOffset = new Vector3(0f, 1.5f, 0);
    [SerializeField] private float reticleBobHeight = 0.2f;
    [SerializeField] private float reticleBobPeriod = 2f;
    [SerializeField] private float hitOffset = 0.1f;

    [Header("Selection Parameters")]
    [SerializeField] private float selectionBoxDistanceThreshold = 5f;
    [SerializeField] private bool allowSnapping = false;

    [Header("UI References")]
    [SerializeField] private Canvas reticleCanvas;
    [SerializeField] private RectTransform reticleTransform;
    [SerializeField] private Canvas hitCanvas;
    [SerializeField] private RectTransform hitTransform;
    [SerializeField] private RectTransform selectionBoxCanvas;
    [SerializeField] private GameObject selectionBoxPrefab;
    [SerializeField] private Texture2D cursorImage;
    [SerializeField] private GameObject selectEffect;
    [SerializeField] private GameObject targetEffect;
    [SerializeField] private CameraController cameraController;

    [System.NonSerialized] public UnityEvent<List<GameObject>> onUnitFocusChangeEvent = new UnityEvent<List<GameObject>>();
    [System.NonSerialized] public UnityEvent<List<GameObject>, bool> onSelectionEvent = new UnityEvent<List<GameObject>, bool>();
    [System.NonSerialized] public UnityEvent<GameObject> onSelectionByTypeEvent = new UnityEvent<GameObject>();
    [System.NonSerialized] public UnityEvent<Transform> onSpellSelectionEvent = new UnityEvent<Transform>();
    [System.NonSerialized] public UnityEvent<RaycastHit> onTargetEvent = new UnityEvent<RaycastHit>();
    [System.NonSerialized] public UnityAction<Unit> HotkeyFocusAction;
    [System.NonSerialized] public UnityAction<Unit> HotkeyUnfocusAction;
    [System.NonSerialized] public UnityEvent onBeginCastEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent onEndCastEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent<GameObject> onPlayerInteractEvent = new UnityEvent<GameObject>();

    private Camera cam;
    private PlayerInputs inputs;
    private Image reticleImage;
    private Image hitImage;

    private RaycastHit hit;
    private Vector2 selectionBoxCorner;
    private RectTransform selectionBox;
    private GameObject lastHoveredObject;
    private GameObject hoveredObject;
    private bool canSnap;
    private bool forceUpdateOutline;
    private List<GameObject> lastDraggedObjects = new List<GameObject>();
    private List<GameObject> draggedObjects = new List<GameObject>();
    private List<GameObject> lastUIFocusedObjects = new List<GameObject>();
    private List<GameObject> uiFocusedObjects = new List<GameObject>();

    private int hoverLayerMask;
    private int spellLayerMask;

    private bool CheckSnap(GameObject go) {
        return allowSnapping && (IsUnit(go.transform) || go.tag == INTERACT_TAG || go.tag == PlayerInteractable.PLAYER_INTERACTABLE_TAG);
    }
    private bool CheckSelectable(GameObject go) {
        return IsAlly(go.transform);
    }
    private bool CheckPlayerInteractable(GameObject go) {
        return go.tag == PlayerInteractable.PLAYER_INTERACTABLE_TAG;
    }
    private bool CheckTargetable(GameObject go) {
        return IsEnemy(go.transform) || go.tag == INTERACT_TAG || IsHero(go.transform);
    }
    private bool CheckSpellTargetable(GameObject go) {
        return IsUnit(go.transform);
    }

    [System.NonSerialized] public ReticleState state;
    public enum ReticleState {
        DEFAULT, PRESS, DRAG, CASTING, RETICLE_DISABLED
    }

    private void Awake() {
        if (current == null) {
            current = this;
        } else {
            Destroy(gameObject);
        }

        inputs = PlayerInputManager.playerInputs;
        inputs.CameraAction.Enable();
        inputs.AllyAction.Enable();
        inputs.PlayerAction.Enable();
        hoverLayerMask = LayerMask.GetMask(HERO_LAYER_NAME, ENEMY_LAYER_NAME, ALLY_LAYER_NAME, INTERACT_LAYER_NAME, INTERACT_DROP_ZONE_LAYER_NAME, GROUND_LAYER, PLAYER_INTERACTABLE_LAYER, DEATH_ZONE_LAYER);
        spellLayerMask = LayerMask.GetMask(GROUND_LAYER, DEATH_ZONE_LAYER);
        
        Cursor.SetCursor(cursorImage, new Vector2(0, 0), CursorMode.Auto);
    }
    
    private void Start() {
        cam = Camera.main;
        reticleImage = reticleTransform.GetComponent<Image>();
        hitImage = hitTransform.GetComponent<Image>();
        state = ReticleState.DEFAULT;
        forceUpdateOutline = false;

        selectionBoxPrefab.GetComponent<Image>().color = UIConstants.commandColor * new Color(1f, 1f, 1f, 0.25f);
    }

    private void OnEnable() {
        inputs.AllyAction.AllyControl.started += StartClick;
        inputs.AllyAction.AllyControl.canceled += PerformClick;
        inputs.AllyAction.SelectByType.performed += SelectByType;

        inputs.PlayerAction.CastSpell.started += SpellSelect;
        inputs.PlayerAction.CastSpell.started += StartCast;
        inputs.PlayerAction.CastSpell.canceled += EndCast;

        HotkeyFocusAction += HotkeyFocus;
        HotkeyUnfocusAction += HotkeyUnfocus;

        onSelectionEvent.AddListener(PlaySelectSound);
        onSelectionByTypeEvent.AddListener(PlaySelectSound);
    }

    private void OnDisable() {
        inputs.AllyAction.AllyControl.started -= StartClick;
        inputs.AllyAction.AllyControl.canceled -= PerformClick;
        inputs.AllyAction.SelectByType.performed -= SelectByType;

        inputs.PlayerAction.CastSpell.started -= SpellSelect;
        inputs.PlayerAction.CastSpell.started -= StartCast;
        inputs.PlayerAction.CastSpell.canceled -= EndCast;

        HotkeyFocusAction -= HotkeyFocus;
        HotkeyUnfocusAction -= HotkeyUnfocus;

        onSelectionEvent.RemoveListener(PlaySelectSound);
        onSelectionByTypeEvent.RemoveListener(PlaySelectSound);
    }

    private void PlaySelectSound(List<GameObject> units, bool dragging) {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_SELECT_SFX);
    }

    private void PlaySelectSound(GameObject unit) {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_SELECT_SFX);
    }

    private void StartClick(InputAction.CallbackContext ctx) {
        switch(state) {
            case ReticleState.DEFAULT:
                state = ReticleState.PRESS;
                selectionBoxCorner = inputs.CameraAction.CursorPosition.ReadValue<Vector2>();
                break;
        }
    }

    private void PerformClick(InputAction.CallbackContext ctx) {
        switch(state) {
            case ReticleState.PRESS:
                state = ReticleState.DEFAULT;
                if(hoveredObject == null) {
                    return;
                }

                if (CheckSelectable(hoveredObject)) {
                    onSelectionEvent.Invoke(new List<GameObject> {hoveredObject}, false);
                    Instantiate(selectEffect, hoveredObject.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up));
                } else if (CheckPlayerInteractable(hoveredObject)) {
                    onPlayerInteractEvent.Invoke(hoveredObject);
                    // Instantiate(selectEffect, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                } else {
                    onTargetEvent.Invoke(hit);
                    if(CheckTargetable(hoveredObject)) {
                        Instantiate(targetEffect, hoveredObject.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up));
                    } else {
                        Instantiate(targetEffect, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    }
                    
                }
                
                break;

            case ReticleState.DRAG:
                Destroy(selectionBox.gameObject);
                onSelectionEvent.Invoke(draggedObjects, true);
                state = ReticleState.DEFAULT;
                forceUpdateOutline = true;
                break;
        }

        state = ReticleState.DEFAULT;
    }

    private void SelectByType(InputAction.CallbackContext ctx) {
        switch(state) {
            case ReticleState.DEFAULT:
            case ReticleState.PRESS:
                if (hoveredObject != null && CheckSelectable(hoveredObject)) {
                    onSelectionByTypeEvent.Invoke(hoveredObject);
                }
                break;
        }

        state = ReticleState.DEFAULT;
    }

    private void SpellSelect(InputAction.CallbackContext ctx) {
        if (state != ReticleState.DEFAULT || (hoveredObject == null) || CheckPlayerInteractable(hoveredObject)) { return; }
        if (CheckSpellTargetable(hoveredObject)) { onSpellSelectionEvent.Invoke(hoveredObject.transform); }
        else { onSpellSelectionEvent.Invoke(reticleTransform); }
    }

    private void HotkeyFocus(Unit unit) {
        if (unit != null) {
            uiFocusedObjects = new List<GameObject>(new GameObject[]{unit.gameObject});
        }
    }

    private void HotkeyUnfocus(Unit unit) {
        uiFocusedObjects = new List<GameObject>();
    }

    private void StartCast(InputAction.CallbackContext ctx) {
        if (state == ReticleState.DEFAULT) {
            if(hoveredObject == null) {
                return;
            }

            if (CheckPlayerInteractable(hoveredObject)) {
                onPlayerInteractEvent.Invoke(hoveredObject);
                // Instantiate(selectEffect, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            } else {
                state = ReticleState.CASTING;
                onBeginCastEvent.Invoke();
            }
        }
    }

    private void EndCast(InputAction.CallbackContext ctx) {
        if (state != ReticleState.CASTING) { return; }
        state = ReticleState.DEFAULT;
        onEndCastEvent.Invoke();
    }

    private void Update() {
        Vector2 mousePos = inputs.CameraAction.CursorPosition.ReadValue<Vector2>();

        if (state == ReticleState.DEFAULT) {
            if (CheckHoveringUI()) {
                state = ReticleState.RETICLE_DISABLED;
                forceUpdateOutline = true;
            }
        } else if (state == ReticleState.RETICLE_DISABLED) {
            if (!CheckHoveringUI()) {
                state = ReticleState.DEFAULT;
                forceUpdateOutline = true;
            }
        } 
        
        CheckHover(mousePos);
        if (state != ReticleState.RETICLE_DISABLED) {
            CheckDrag(mousePos);
        }
        UpdateOutlines();
    }

    private void LateUpdate() {    
        DrawReticle();
    }

    private bool CheckHoveringUI() {
        if(EventSystem.current != null) {
            return EventSystem.current.IsPointerOverGameObject();
        } else {
            return true;
        }
        
    }

    private void CheckHover(Vector2 mousePos) {
        Ray ray = cam.ScreenPointToRay(mousePos);

        switch(state) {
            case ReticleState.DEFAULT:
            case ReticleState.PRESS:
            case ReticleState.DRAG:
            case ReticleState.RETICLE_DISABLED:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hoverLayerMask, QueryTriggerInteraction.Ignore)) {
                    hoveredObject = hit.transform.gameObject;
                    canSnap = CheckSnap(hoveredObject);
                } else {
                    hoveredObject = null;
                    canSnap = false;
                }
                break;

            case ReticleState.CASTING:
                canSnap = false;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, spellLayerMask, QueryTriggerInteraction.Ignore)) {
                    hoveredObject = hit.transform.gameObject;
                } else {
                    hoveredObject = null;
                }
                break;
        }
    }

    private void CheckDrag(Vector2 mousePos) {
        switch(state) {
            case ReticleState.PRESS:
                if (Vector2.Distance(mousePos, selectionBoxCorner) > selectionBoxDistanceThreshold) {
                    state = ReticleState.DRAG;
                    forceUpdateOutline = true;
                    selectionBox = Instantiate(selectionBoxPrefab, selectionBoxCanvas).GetComponent<RectTransform>();
                    goto case ReticleState.DRAG;
                } else {
                    break;
                }
            
            case ReticleState.DRAG:

                Vector3 centerPos = (selectionBoxCorner + mousePos) / 2;
                Vector3 dims = selectionBoxCorner - mousePos;
                dims.x = Mathf.Abs(dims.x);
                dims.y = Mathf.Abs(dims.y);
                dims.z = 1;
                selectionBox.position = centerPos;
                selectionBox.sizeDelta = selectionBoxCanvas.InverseTransformVector(dims);

                // ORTHOGRAPHIC CAMERA
                // float conversionFactor = 
                //     Vector3.Distance(cam.ScreenToWorldPoint(dims), cam.ScreenToWorldPoint(new Vector3(0,0,0))) /
                //     Vector3.Distance(dims, new Vector3(0,0,0));
                
                // RaycastHit[] hits = Physics.BoxCastAll(cam.ScreenToWorldPoint(centerPos),
                //     dims/2 * conversionFactor,
                //     cam.ScreenPointToRay(centerPos).direction, 
                //     cam.transform.rotation,
                //     Mathf.Infinity,
                //     selectableLayerMask,
                //     QueryTriggerInteraction.Ignore);
                // draggedObjects = hits.ToList().Select(hit => hit.transform.gameObject).ToList();

                // PERSPECTIVE CAMERA
                draggedObjects.Clear();
                foreach (Unit unit in UnitGroupManager.current.allUnits.units) {
                    Vector2 screenPos = cam.WorldToScreenPoint(unit.transform.position);
                    if(screenPos.x >= Mathf.Min(selectionBoxCorner.x, mousePos.x) && 
                            screenPos.x <= Mathf.Max(selectionBoxCorner.x, mousePos.x) &&
                            screenPos.y >= Mathf.Min(selectionBoxCorner.y, mousePos.y) &&
                            screenPos.y <= Mathf.Max(selectionBoxCorner.y, mousePos.y)) {
                        draggedObjects.Add(unit.gameObject);
                    }
                }
                
                break;
        }
    }

    private void UpdateOutlines() {
        switch(state) {
            case ReticleState.DEFAULT:
            case ReticleState.PRESS:
                if (forceUpdateOutline || lastHoveredObject != hoveredObject) {
                    onUnitFocusChangeEvent.Invoke(new List<GameObject> {hoveredObject});
                }
                break;
            
            case ReticleState.DRAG:
                if (forceUpdateOutline || lastDraggedObjects.Count != draggedObjects.Count || !lastDraggedObjects.TrueForAll(draggedObjects.Contains)) {
                    onUnitFocusChangeEvent.Invoke(draggedObjects);
                }
                lastDraggedObjects = new List<GameObject>(draggedObjects);
                break;
            
            case ReticleState.RETICLE_DISABLED:
                if (forceUpdateOutline || lastUIFocusedObjects.Count != uiFocusedObjects.Count || !lastUIFocusedObjects.TrueForAll(uiFocusedObjects.Contains)) {
                    onUnitFocusChangeEvent.Invoke(uiFocusedObjects);
                }
                lastUIFocusedObjects = uiFocusedObjects;
                break;
        }

        forceUpdateOutline = false;
        lastHoveredObject = hoveredObject;
    }

    private void DrawReticle() {

        Vector3 targetPos = canSnap ? hit.transform.position : hit.point;

        hitCanvas.gameObject.SetActive(!canSnap);
        // hitCanvas.transform.position = Vector3.Lerp(hitCanvas.transform.position, targetPos + hitOffset * hit.normal, reticleLerpFactor);
        hitCanvas.transform.position = targetPos + hitOffset * hit.normal;
        hitCanvas.transform.rotation = Quaternion.FromToRotation(Vector3.forward, canSnap ? Vector3.up : hit.normal);

        // reticleCanvas.transform.position = Vector3.Lerp(reticleCanvas.transform.position, targetPos + (canSnap ? reticleUnitHeightOffset : new Vector3(0,0,0)), reticleLerpFactor);
        reticleCanvas.transform.position = targetPos + (canSnap ? reticleUnitHeightOffset : new Vector3(0,0,0));
        reticleCanvas.transform.rotation = cam.transform.rotation;
        reticleTransform.anchoredPosition = reticleOffset + new Vector2(0, Mathf.Abs(reticleBobHeight * Mathf.Sin(Time.time * Mathf.PI / reticleBobPeriod)));

        
        switch(state) {
            case ReticleState.DEFAULT:
            case ReticleState.PRESS:
            case ReticleState.CASTING:

                if (hoveredObject != null) {
                    reticleCanvas.enabled = true;
                    hitCanvas.enabled = true;

                    switch(state) {
                        case ReticleState.DEFAULT:
                        case ReticleState.PRESS:

                            if (canSnap && CheckSelectable(hoveredObject)) {
                                reticleImage.color = UIConstants.commandColor;
                                hitImage.color = UIConstants.commandColor;
                            } else if (canSnap && CheckTargetable(hoveredObject)) {
                                reticleImage.color = UIConstants.targetColor;
                                hitImage.color = UIConstants.targetColor;
                            } else {
                                reticleImage.color = UIConstants.defaultReticleColor;
                                hitImage.color = UIConstants.defaultReticleColor;
                            }
                            break;
                            
                        case ReticleState.CASTING:
                            reticleImage.color = UIConstants.castingColor;
                            hitImage.color = UIConstants.castingColor;
                            break;
                    }
                    
                    Cursor.visible = false;

                } else {
                    reticleCanvas.enabled = false;
                    hitCanvas.enabled = false;
                    Cursor.visible = !cameraController.rotating;
                }
                break;

            case ReticleState.DRAG:
                reticleCanvas.enabled = false;
                hitCanvas.enabled = false;
                Cursor.visible = false;
                break;

            case ReticleState.RETICLE_DISABLED:
                reticleCanvas.enabled = false;
                hitCanvas.enabled = false;
                Cursor.visible = !cameraController.rotating;
                break;
        }
    }

    public Transform GetReticleTransform() { return reticleCanvas.transform; }

    //Returns null if raycast not hitting anything
    public GameObject GetTargetObject() { return hoveredObject; }
}
