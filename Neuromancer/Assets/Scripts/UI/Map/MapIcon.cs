using UnityEngine;

public class MapIcon : MonoBehaviour {
    
    [SerializeField] private bool rotate;
    [SerializeField] private float scaledSize;
    [SerializeField] private float displayRadius = float.PositiveInfinity;
    private Camera minimapCamera;
    private Camera worldMapCamera;
    private Tweener resizeTweener;
    
    [SerializeField] private SpriteRenderer minimapIcon;
    [SerializeField] private GameObject worldMapIcon;

    private static bool constantIconSize = true;
    private static float worldMapScale = 0.5f;

    private void Awake() {
        minimapIcon.color = new Color(1f, 1f, 1f, 0f);
    }

    private void Start() {
        minimapCamera = MinimapController.current.GetMinimapCamera();
        worldMapCamera = WorldMapController.current.GetWorldMapCamera();
        resizeTweener = new Tweener(this, SetMinimapSize);

        SetWorldMapSize(worldMapCamera.orthographicSize);
        SetMinimapSize(minimapCamera.orthographicSize);

        MinimapController.current.OnMinimapResizeEvent.AddListener(OnMinimapResize);
        WorldMapController.current.OnWorldMapUpdateEvent.AddListener(SetWorldMapSize);

        CheckDistance();
    }

    private void OnDestroy() {
        MinimapController.current.OnMinimapResizeEvent.RemoveListener(OnMinimapResize);
        WorldMapController.current.OnWorldMapUpdateEvent.RemoveListener(SetWorldMapSize);
    }

    private void OnMinimapResize(float oldRadius, float newRadius) {
        if (constantIconSize) {
            resizeTweener.TweenWithTime(oldRadius, newRadius, MinimapController.RESIZE_TIME, Tweener.QUAD_EASE_OUT);
        }
    }

    private void SetMinimapSize(float x) {
        if(minimapIcon != null) {
            minimapIcon.transform.parent = null;
            minimapIcon.transform.localScale = x * scaledSize * Vector3.one;
            minimapIcon.transform.parent = transform;
        }
    }

    private void SetWorldMapSize(float x) {
        if(worldMapCamera != null && worldMapIcon != null) {
            worldMapIcon.transform.parent = null;
            worldMapIcon.transform.localScale = x * worldMapScale * scaledSize * Vector3.one;
            worldMapIcon.transform.parent = transform;

            if(!rotate) {
                worldMapIcon.transform.rotation = worldMapCamera.transform.rotation;
            }
        }
    }

    private void Update() {
        CheckDistance();
    }

    private void CheckDistance() {
        float dist = (transform.position - PlayerController.player.transform.position).magnitude;
        Color c = minimapIcon.color;
        minimapIcon.color = new Color(c.r, c.g, c.b, Mathf.Clamp(-(dist - displayRadius)/2f, 0f, 1f));
    }

    private void LateUpdate() {
        if (minimapIcon != null && !rotate) {
            minimapIcon.transform.rotation = minimapCamera.transform.rotation;
        }
    }

    public void SetDisplayRadius(float displayRadius) {
        this.displayRadius = displayRadius;
    }
}
