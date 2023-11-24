using UnityEngine;
using System.Collections.Generic;

public class InteractableOutlineController : MonoBehaviour {
    
    private Outline focusOutline;
    
    private void Awake() {
        focusOutline = gameObject.AddComponent<Outline>();

        focusOutline.OutlineMode =  Outline.Mode.OutlineAll;
        focusOutline.OutlineWidth = UIConstants.outlineWidth;
    }

    private void Start() {
        Focus(new List<GameObject>());
        focusOutline.OutlineColor = UIConstants.targetColor;

        ReticleController.current.onUnitFocusChangeEvent.AddListener(Focus);
    }

    private void OnEnable() {
        ReticleController.current?.onUnitFocusChangeEvent.AddListener(Focus);
    }

    private void OnDisable() {
        ReticleController.current?.onUnitFocusChangeEvent.RemoveListener(Focus);
    }

    private void Focus(List<GameObject> focusedObjects) {
        focusOutline.enabled = focusedObjects.Contains(gameObject);
    }
}