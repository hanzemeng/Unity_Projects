using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapDrag : EventTrigger {
    private RectTransform rectTransform;
    private bool dragging;
    private Vector2 lastPos;
    private PlayerInputs inputs;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();

        inputs = PlayerInputManager.playerInputs;
        inputs.MenuAction.Enable();
    }

    public override void OnBeginDrag(PointerEventData data) {
        lastPos = GetRelativePos(data);
        dragging = true;
    }

    public override void OnDrag(PointerEventData data) {
        if (dragging) {
            Vector2 newPos = GetRelativePos(data);
            CameraController.current.CameraRotateRawDelta(Mathf.Rad2Deg * (Mathf.Atan2(newPos.y, newPos.x) - Mathf.Atan2(lastPos.y, lastPos.x)));
            lastPos = newPos;
            Debug.Log(lastPos);
        }
    }

    public override void OnEndDrag(PointerEventData data) {
        dragging = false;
    }

    private Vector2 GetRelativePos(PointerEventData data) {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out localPoint);
        return localPoint;
    }
}