using UnityEngine;
using UnityEngine.UI;
using Neuromancer;

public class UnitMarkerController : MonoBehaviour {

    [SerializeField] private Image unitMarker;
    [SerializeField] private MapIcon mapIcon;
    [SerializeField] private SpriteRenderer minimapIcon;
    
    private NPCUnit unit;
    private Canvas canvas;
    
    private void Awake() {
        unit = GetComponentInParent<NPCUnit>();
        canvas = unitMarker.GetComponentInParent<Canvas>();
        canvas.enabled = false;
    }

    private void Start() {
        SetUnitType(true);
    }

    private void OnEnable() {
        unit.OnNewUnitTypeEvent.AddListener(SetUnitType);
        unit.OnUnitSelectionChanged.AddListener(Select);
    }

    private void OnDisable() {
        unit.OnNewUnitTypeEvent.RemoveListener(SetUnitType);
        unit.OnUnitSelectionChanged.AddListener(Select);
    }

    private void SetUnitType(bool success) {
        Color c = UIConstants.GetColorFromUnitType(unit.transform);
        unitMarker.color = c;
        
        if (!unit.unitPrefab.isBoss && Unit.IsEnemy(unit.transform)) {
            mapIcon.SetDisplayRadius(30f);
        } else {
            mapIcon.SetDisplayRadius(float.PositiveInfinity);
        }

        minimapIcon.color = new Color(c.r, c.g, c.b, minimapIcon.color.a);
    }

    private void Select(bool selected) {
        canvas.enabled = selected;
    }
}