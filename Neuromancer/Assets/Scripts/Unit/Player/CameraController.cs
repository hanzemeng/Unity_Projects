using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Zoom Settings")]
    [SerializeField] private bool overheadViewActive;
    [SerializeField] private Vector3 normalCameraOffset = new Vector3(0f, 50f, -50f);
    [SerializeField] private Vector3 overheadCameraOffset = new Vector3(0f, 80f, -1f);
    [SerializeField] private Vector3 normalCameraDamping = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 overheadCameraDamping = new Vector3(0f, 0f, 0f);
    [SerializeField] private float zoomTweenTime = 0.25f;

    [Header("Camera Rotation Settings")]
    [SerializeField] private float rotationFactor = 0.5f;
    
    public static CameraController current;
    private PlayerInputs inputs;
    public CinemachineVirtualCamera virtualCam;
    private List<Camera> cameraStack;
    private CinemachineOrbitalTransposer transposer;
    private TweenerVector zoomTweener;
    public bool rotating {get; private set;}

    private void Awake() {
        if (current == null) { current = this; }
        else { Destroy(gameObject); }
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        cameraStack = Camera.main.GetComponent<UniversalAdditionalCameraData>().cameraStack;
        inputs = PlayerInputManager.playerInputs;
        inputs.CameraAction.Enable();
        transposer = virtualCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        zoomTweener = new TweenerVector(this, x => transposer.m_FollowOffset = x);
        rotating = false;
        overheadViewActive = false;
        UpdateZoom();
    }
    
    private void Update() {
        CameraRotate();
    }

    private void LateUpdate() {
        CameraStackFollow();
    }

    private void OnEnable() {
        inputs.CameraAction.Zoom.performed += ToggleZoom;
        inputs.CameraAction.RotateMode.started += StartRotate;
        inputs.CameraAction.RotateMode.canceled += StopRotate;
    }

    private void OnDisable() {
        inputs.CameraAction.Zoom.performed -= ToggleZoom;
        inputs.CameraAction.RotateMode.started -= StartRotate;
        inputs.CameraAction.RotateMode.canceled -= StopRotate;
    }

    private void ToggleZoom(InputAction.CallbackContext callbackContext) {
        overheadViewActive = !overheadViewActive;
        UpdateZoom();
    }

    private void UpdateZoom() {
        if (!overheadViewActive) {
            zoomTweener.TweenWithTime(transposer.m_FollowOffset, normalCameraOffset, zoomTweenTime, Tweener.QUAD_EASE_OUT);
            transposer.m_XDamping = normalCameraDamping.x;
            transposer.m_YDamping = normalCameraDamping.y;
            transposer.m_ZDamping = normalCameraDamping.z;
        } else {
            zoomTweener.TweenWithTime(transposer.m_FollowOffset, overheadCameraOffset, zoomTweenTime, Tweener.QUAD_EASE_OUT);
            transposer.m_XDamping = overheadCameraDamping.x;
            transposer.m_YDamping = overheadCameraDamping.y;
            transposer.m_ZDamping = overheadCameraDamping.z;
        }
    }

    private void StartRotate(InputAction.CallbackContext callbackContext) {
        rotating = true;
    }

    private void StopRotate(InputAction.CallbackContext callbackContext) {
        rotating = false;
    }

    private void CameraStackFollow() {
        foreach (Camera camera in cameraStack) {
            camera.fieldOfView = Camera.main.fieldOfView;
        }
    }

    private void CameraRotate() {
        if (rotating) {
            float rotateAmount = inputs.CameraAction.Rotate.ReadValue<Vector2>().x * rotationFactor;
            CameraRotateRawDelta(rotateAmount);
        }
    }

    public void CameraRotateRawDelta(float delta) {
        transposer.m_XAxis.Value += delta;
    }
}