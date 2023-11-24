using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class WorldMapController : MonoBehaviour {

    public static WorldMapController current;

    [SerializeField] private Camera worldMapCamera;
    [SerializeField] private Camera fogCamera;
    [SerializeField] private GameObject mapGroup;
    [SerializeField] private RenderTexture fogRenderTexture;
    private const string WORLD_MAP_CAM_LOC = "World Map Camera Location";
    
    
    private List<Camera> cameraStack;

    [HideInInspector] public UnityEvent<float> OnWorldMapUpdateEvent = new UnityEvent<float>();
    
    private void Awake() {
        if (current == null) {
            current = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        cameraStack = worldMapCamera.GetComponent<UniversalAdditionalCameraData>().cameraStack;

        worldMapCamera.orthographic = true;
        foreach(Camera cam in cameraStack) {
            cam.orthographic = true;
        }

        DisableWorldMapCamera();
        
        PauseHandler.onPauseEvent.AddListener(EnableWorldMapCamera);
        PauseHandler.onResumeEvent.AddListener(DisableWorldMapCamera);
        LevelManager.levelManager.onNewSceneEvent.AddListener(NewPosition);
    }

    private void OnDestroy() {
        PauseHandler.onPauseEvent.RemoveListener(EnableWorldMapCamera);
        PauseHandler.onResumeEvent.RemoveListener(DisableWorldMapCamera);
        LevelManager.levelManager.onNewSceneEvent.RemoveListener(NewPosition);
    }

    private void NewPosition() {
        // STUB!!! Replace with something you can save/load
        fogRenderTexture.Release();

        GameObject[] locs = GameObject.FindGameObjectsWithTag(WORLD_MAP_CAM_LOC);
        if(locs.Length > 0) {
            mapGroup.SetActive(true);

            WorldMapCameraLocation loc = locs[0].GetComponent<WorldMapCameraLocation>();

            worldMapCamera.orthographicSize = loc.cameraSize;
            worldMapCamera.transform.position = loc.transform.position;
            fogCamera.orthographicSize = loc.cameraSize;
            fogCamera.transform.position = loc.transform.position;
            foreach(Camera cam in cameraStack) {
                cam.orthographicSize = loc.cameraSize;
            }

            OnWorldMapUpdateEvent.Invoke(loc.cameraSize);
        } else {
            mapGroup.SetActive(false);
        }
    }

    public Camera GetWorldMapCamera() {
        return worldMapCamera;
    }

    private void DisableWorldMapCamera() {
        worldMapCamera.gameObject.SetActive(false);
    }

    private void EnableWorldMapCamera(bool isMainMenu) {
        if(isMainMenu) {
            worldMapCamera.gameObject.SetActive(true);
        }
    }
}