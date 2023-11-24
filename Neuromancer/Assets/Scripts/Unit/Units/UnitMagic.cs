using UnityEngine;
using UnityEngine.Events;

public class UnitMagic : MonoBehaviour
{
    [SerializeField] public float magic, maxMagic, magicRegen;

    [System.NonSerialized] public UnityEvent<float> onMagicDrainEvent = new UnityEvent<float>();
    [System.NonSerialized] public UnityEvent<float> onMagicRechargeEvent = new UnityEvent<float>();
    [System.NonSerialized] public UnityEvent<bool> onZeroMagicEvent = new UnityEvent<bool>();
    [System.NonSerialized] public UnityEvent<float> onBeginCastEvent = new UnityEvent<float>();
    [System.NonSerialized] public UnityEvent<float> onEndCastEvent = new UnityEvent<float>();

    private PlayerInputs inputs;
    private bool casting;
    private ReticleController reticleController;
    private PlayerAnimation animationController;

    private void Awake() {
        magic = maxMagic;
        inputs = PlayerInputManager.playerInputs;
        inputs.PlayerAction.Enable();
        animationController = GetComponent<PlayerAnimation>();
    }

    private void Start() {
        reticleController = ReticleController.current;
        reticleController.onBeginCastEvent.AddListener(BeginCast);
        reticleController.onEndCastEvent.AddListener(EndCast);
    }

    private void OnEnable() {
        onMagicDrainEvent.AddListener(CheckZeroMagic);
        reticleController?.onBeginCastEvent.AddListener(BeginCast);
        reticleController?.onEndCastEvent.AddListener(EndCast);
        animationController.onDeathReload.AddListener(Revive);
    }

    private void OnDisable() {
        onMagicDrainEvent.RemoveListener(CheckZeroMagic);
        reticleController.onBeginCastEvent.RemoveListener(BeginCast);
        reticleController.onEndCastEvent.RemoveListener(EndCast);
        animationController.onDeathReload.RemoveListener(Revive);
    }

    private void Update() {
        if (magicRegen > 0 && !casting) {
            RechargeMagic(magicRegen * Time.deltaTime);
        }
    }

    private void BeginCast() {
        casting = true;
        onBeginCastEvent.Invoke(magic);
    }

    private void EndCast() {
        casting = false;
        onEndCastEvent.Invoke(magic);
    }
    
    public void DrainMagic(float drainAmount) {
        magic = Mathf.Clamp(magic - drainAmount, 0, maxMagic);
        onMagicDrainEvent.Invoke(magic);
    }

    public void RechargeMagic(float rechargeAmount) {
        magic = Mathf.Clamp(magic + rechargeAmount, 0, maxMagic);
        onMagicRechargeEvent.Invoke(magic);
    }

    private void CheckZeroMagic(float magic) {
        if (magic <= 0) {
            onZeroMagicEvent.Invoke(true);
        }
    }
    
    public float GetMagic() { return magic; }
    public float GetMaxMagic() { return maxMagic; }
    public bool IsCasting() { return casting; }

    private void Revive() {
        magic = maxMagic;
    }
}
