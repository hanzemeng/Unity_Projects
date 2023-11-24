using UnityEngine;
using System.Collections.Generic;
using Neuromancer;

public class UnitOutlineController : MonoBehaviour {
    
    private Outline typeOutline;
    private Outline focusOutline;
    private Unit unit;
    
    private void Awake() {
        unit = GetComponentInParent<Unit>();
        typeOutline = unit.gameObject.AddComponent<Outline>();
        focusOutline = unit.gameObject.AddComponent<Outline>();

        typeOutline.OutlineMode = Outline.Mode.SilhouetteOnly;
        focusOutline.OutlineMode =  Outline.Mode.OutlineAll;
        focusOutline.OutlineWidth = UIConstants.outlineWidth;
    }

    private void Start() {
        Focus(new List<GameObject>());
        SetOutlineColor(true);

        ReticleController.current.onUnitFocusChangeEvent.AddListener(Focus);
    }

    private void OnEnable() {
        ReticleController.current?.onUnitFocusChangeEvent.AddListener(Focus);
        unit.OnNewUnitTypeEvent.AddListener(SetOutlineColor);
    }

    private void OnDisable() {
        ReticleController.current.onUnitFocusChangeEvent.RemoveListener(Focus);
        unit.OnNewUnitTypeEvent.RemoveListener(SetOutlineColor);
    }

    private void Focus(List<GameObject> focusedObjects) {
        focusOutline.enabled = focusedObjects.Contains(unit.gameObject);
    }

    private void SetOutlineColor(bool success) {
        Color color = UIConstants.GetColorFromUnitType(unit.gameObject.transform);
        focusOutline.OutlineColor = color;
        color.a = 0.25f;
        typeOutline.OutlineColor = color;
    }
}