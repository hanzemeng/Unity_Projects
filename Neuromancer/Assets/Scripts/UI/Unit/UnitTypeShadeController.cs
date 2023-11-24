using UnityEngine;
using Neuromancer;

public class UnitTypeShadeController : MonoBehaviour {

    private Unit unit;
    [SerializeField] new private Renderer renderer;
    private Material material;

    TweenerColor colorTweener;
    
    private void Awake() {
        unit = GetComponentInParent<Unit>();
        material = renderer.material;
        colorTweener = new TweenerColor(this, res => material.color = res);
    }

    private void Start() {
        SetOutlineColor(false);
    }

    private void OnEnable() {
        unit.OnNewUnitTypeEvent.AddListener(SetOutlineColor);
    }

    private void OnDisable() {
        unit.OnNewUnitTypeEvent.RemoveListener(SetOutlineColor);
    }

    private void SetOutlineColor(bool tween) {
        Color color = UIConstants.GetShadedColor(UIConstants.GetColorFromUnitType(unit.gameObject.transform));
        
        if (tween) {
            colorTweener.TweenWithTime(material.color, color, 0.5f, Tweener.LINEAR);
        } else {
            material.color = color;
        }
    }
}