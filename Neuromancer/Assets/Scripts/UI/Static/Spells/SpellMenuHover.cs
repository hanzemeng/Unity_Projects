using UnityEngine.EventSystems;

public class SpellMenuHover : EventTrigger {

    private string info = "";

    public void SetInfo(string info) {
        this.info = info;
    }

    public void ResetInfo() {
        this.info = "";
    }
    
    public override void OnPointerEnter(PointerEventData data) {
        SpellMenuManager.current.HoverInfo(info);
    }

    public override void OnPointerExit(PointerEventData data) {
        SpellMenuManager.current.UnhoverInfo();
    }
}