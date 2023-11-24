using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneMinimapColor {
    public string scenePrefix;
    public Color floorColor;
    public Color obstacleColor;
}

public class MinimapController : MonoBehaviour {

    public static MinimapController current;

    private Camera mainCamera;
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private List<Camera> allCameras;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform minimapFrame;
    [SerializeField] private GameObject minimapContainer;
    [SerializeField] private List<float> minimapRadiusList = new List<float>() {20f};
    [SerializeField] private int radiusIndex = 0;
    [SerializeField] private float cameraYOffset = 100f;

    [SerializeField] private Button zoomInButton;
    [SerializeField] private Button zoomOutButton;
    [SerializeField] private Image floorImage;
    [SerializeField] private Image obstacleImage;
    [SerializeField] private List<SceneMinimapColor> sceneMinimapColors;
    [SerializeField] private SceneMinimapColor defaultMinimapColors;
    [SerializeField] private List<string> disabledMinimapScenes;
    
    
    private Tweener minimapRadiusTweener;
    public static float RESIZE_TIME = 0.25f;

    [HideInInspector] public UnityEvent<float, float> OnMinimapResizeEvent = new UnityEvent<float, float>();
    
    private void Awake() {
        if (current == null) {
            current = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        mainCamera = Camera.main;

        minimapCamera.orthographic = true;
        minimapCamera.aspect = 1f;
        foreach(Camera cam in allCameras) {
            cam.orthographic = true;
            cam.aspect = 1f;
        }

        SetCameraSizes(minimapRadiusList[radiusIndex]);

        minimapRadiusTweener = new Tweener(this, x => SetCameraSizes(x));
        LevelManager.levelManager.onNewSceneEvent.AddListener(NewScene);
    }

    private void OnDestroy() {
        LevelManager.levelManager.onNewSceneEvent.RemoveListener(NewScene);
    }

    private void NewScene() {
        string sceneName = SceneManager.GetActiveScene().name;
        minimapContainer.SetActive(!disabledMinimapScenes.Contains(sceneName));

        foreach(SceneMinimapColor sceneMinimapColor in sceneMinimapColors) {
            if(sceneName.StartsWith(sceneMinimapColor.scenePrefix)) {
                floorImage.color = sceneMinimapColor.floorColor;
                obstacleImage.color = sceneMinimapColor.obstacleColor;
                return;
            }
        }

        floorImage.color = defaultMinimapColors.floorColor;
        obstacleImage.color = defaultMinimapColors.obstacleColor;
    }

    private void SetCameraSizes(float radius) {
        minimapCamera.orthographicSize = radius;
        foreach(Camera cam in allCameras) {
            cam.orthographicSize = radius;
        }
    }

    private void SetZoomButtons() {
        zoomInButton.interactable = (radiusIndex > 0);
        zoomOutButton.interactable = (radiusIndex < minimapRadiusList.Count - 1);
    }

    public void ZoomIn() {
        if(radiusIndex > 0) {
            radiusIndex--;
            minimapRadiusTweener.TweenWithTime(minimapCamera.orthographicSize, minimapRadiusList[radiusIndex], RESIZE_TIME, Tweener.QUAD_EASE_OUT);
            OnMinimapResizeEvent.Invoke(minimapRadiusList[radiusIndex+1], minimapRadiusList[radiusIndex]);

            SetZoomButtons();
        }
    }

    public void ZoomOut() {
        if(radiusIndex < minimapRadiusList.Count - 1) {
            radiusIndex++;
            minimapRadiusTweener.TweenWithTime(minimapCamera.orthographicSize, minimapRadiusList[radiusIndex], RESIZE_TIME, Tweener.QUAD_EASE_OUT);
            OnMinimapResizeEvent.Invoke(minimapRadiusList[radiusIndex-1], minimapRadiusList[radiusIndex]);

            SetZoomButtons();
        }
    }

    private void LateUpdate() {
        minimapCamera.transform.position = playerTransform.position + new Vector3(0f, cameraYOffset, 0f);
        minimapCamera.transform.rotation = Quaternion.Euler(90f, mainCamera.transform.eulerAngles.y, 0f);
        minimapFrame.rotation = Quaternion.Euler(0f, 0f, mainCamera.transform.eulerAngles.y);
    }

    public Camera GetMinimapCamera() {
        return minimapCamera;
    }
}