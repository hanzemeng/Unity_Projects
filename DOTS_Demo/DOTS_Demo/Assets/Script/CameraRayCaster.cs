using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]
public class CameraRayCaster : MonoBehaviour
{
    public delegate void CameraRayCasterCallback(RaycastHit raycastHit);

    private new Camera camera;
    private Dictionary<int, List<CameraRayCasterCallback>> mouseButtonEvents;
    private Dictionary<int, List<CameraRayCasterCallback>> mouseButtonDownEvents;
    private Dictionary<int, List<CameraRayCasterCallback>> mouseButtonUpEvents;

    public float castDistance;
    private Ray currentMouseRay;
    private RaycastHit currentMouseRaycastHit;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        mouseButtonEvents = new Dictionary<int, List<CameraRayCasterCallback>>();
        mouseButtonDownEvents = new Dictionary<int, List<CameraRayCasterCallback>>();
        mouseButtonUpEvents = new Dictionary<int, List<CameraRayCasterCallback>>();
    }

    private void Update()
    {
        currentMouseRay = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(currentMouseRay, out currentMouseRaycastHit, castDistance);

        foreach(KeyValuePair<int, List<CameraRayCasterCallback>> keyValuePair in mouseButtonEvents)
        {
            if(!Input.GetMouseButton(keyValuePair.Key))
            {
                continue;
            }

            foreach(CameraRayCasterCallback callback in keyValuePair.Value)
            {
                callback.Invoke(currentMouseRaycastHit);
            }
        }

        foreach(KeyValuePair<int, List<CameraRayCasterCallback>> keyValuePair in mouseButtonDownEvents)
        {
            if(!Input.GetMouseButtonDown(keyValuePair.Key))
            {
                continue;
            }

            foreach(CameraRayCasterCallback callback in keyValuePair.Value)
            {
                callback.Invoke(currentMouseRaycastHit);
            }
        }

        foreach(KeyValuePair<int, List<CameraRayCasterCallback>> keyValuePair in mouseButtonUpEvents)
        {
            if(!Input.GetMouseButtonUp(keyValuePair.Key))
            {
                continue;
            }

            foreach(CameraRayCasterCallback callback in keyValuePair.Value)
            {
                callback.Invoke(currentMouseRaycastHit);
            }
        }
    }

    public void AddMouseButtonAction(int mouseButtonIndex, CameraRayCasterCallback callback)
    {
        if(!mouseButtonEvents.ContainsKey(mouseButtonIndex))
        {
            mouseButtonEvents[mouseButtonIndex] = new List<CameraRayCasterCallback>();
        }
        mouseButtonEvents[mouseButtonIndex].Add(callback);
    }
    public void AddMouseButtonDownAction(int mouseButtonIndex, CameraRayCasterCallback callback)
    {
        if(!mouseButtonDownEvents.ContainsKey(mouseButtonIndex))
        {
            mouseButtonDownEvents[mouseButtonIndex] = new List<CameraRayCasterCallback>();
        }
        mouseButtonDownEvents[mouseButtonIndex].Add(callback);
    }
    public void AddMouseButtonUpAction(int mouseButtonIndex, CameraRayCasterCallback callback)
    {
        if(!mouseButtonUpEvents.ContainsKey(mouseButtonIndex))
        {
            mouseButtonUpEvents[mouseButtonIndex] = new List<CameraRayCasterCallback>();
        }
        mouseButtonUpEvents[mouseButtonIndex].Add(callback);
    }
}
