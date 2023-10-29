using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroductionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    EventSystem eventSystem;
    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        eventSystem.SetSelectedGameObject(null);
    }
}
