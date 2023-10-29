using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class MouseInput : MonoBehaviour
{
    private float mouseDownX;
    private float mouseUpX;
    private bool mouseInCollider;
    private bool mouseDownInCollider;

    public UnityEvent onClickEvent;
    public UnityEvent onMouseDownEvent;
    public UnityEvent onMouseUpEvent;
    public UnityEvent onSwipeRightEvent;
    public UnityEvent onSwipeLeftEvent;

    private void Start()
    {
        mouseInCollider = false;
        mouseDownInCollider = false;
    }

    private void OnMouseEnter()
    {
        mouseInCollider = true;
    }
    private void OnMouseExit()
    {
        mouseInCollider = false;
    }

    private void OnMouseDown()
    {
        mouseDownInCollider = true;
        mouseDownX = Input.mousePosition.x;
        onMouseDownEvent.Invoke();
    }
    private void OnMouseUp()
    {
        onMouseUpEvent.Invoke();
        if(!mouseInCollider || !mouseDownInCollider)
        {
            mouseDownInCollider = false;
            return;
        }

        mouseUpX = Input.mousePosition.x;
        mouseDownInCollider = false;
        onClickEvent.Invoke();

        if(mouseUpX-mouseDownX > 100f)
        {
            onSwipeRightEvent.Invoke();
        }
        else if(mouseUpX-mouseDownX < -100f)
        {
            onSwipeLeftEvent.Invoke();
        }
    }

}
