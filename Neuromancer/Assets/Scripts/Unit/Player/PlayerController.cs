using System.Collections;
using UnityEngine;
using EmeraldAI;
using EmeraldAI.Example;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player;

    [Header("Control Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float slerpRatio = 0.1f;
    [SerializeField] private float gravity = -5f;
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 0.6f;
    [SerializeField] private float dashFXOffset = 1.5f;
    [SerializeField] private float dashCost = 10f;
    [SerializeField] private GameObject dashEffect;
    [Header("Grounded Check Settings")]
    [SerializeField] private Vector3 wallCheck = new Vector3(1f, -0.1f, 0f);
    [SerializeField] private float slipSpeed = 1f;

    private PlayerInputs inputs;
    private CharacterController controller;
    private EmeraldAIPlayerHealth healthController;
    private UnitMagic magicController;
    private PlayerAnimation animationController;
    private Transform mainCamera;
    
    public Vector3 moveVector;
    public float spellDamageModifier = 1f;
    private Neuromancer.Unit unit;
    private bool isDead = false;
    private bool isDashing = false;
    private bool dashEnabled = false;
    private float dashCooldownLeft = 0f;
    private int GROUND_MASK;
    private string defaultLayer = Neuromancer.Unit.HERO_LAYER_NAME;

    // base parameters - to be reverted back when god mode is disabled:
    private float baseSpeed;
    private float baseDashCooldown;
    private float baseDashTime;
    private int baseCurrentHealth;
    private float baseMagicRegen;
    private float baseSlerpRatio;
    private LayerMask baseLayer;
    private string baseDefaultLayer = Neuromancer.Unit.HERO_LAYER_NAME;
    private bool isGodModeEnabled = false;

    [HideInInspector] public UnityEvent BeginDashEvent = new UnityEvent();
    [HideInInspector] public UnityEvent EndDashEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<bool> GodModeEvent = new UnityEvent<bool>();    // new event to invoke god mode.

    private void Awake() {
        if (player == null) { player = this; }
        else { Destroy(gameObject); }
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
        healthController = GetComponent<EmeraldAIPlayerHealth>();
        magicController = GetComponent<UnitMagic>();
        animationController = GetComponent<PlayerAnimation>();
        inputs = PlayerInputManager.playerInputs;
        inputs.PlayerAction.Enable();
        
        unit = GetComponent<Neuromancer.Unit>();
        GROUND_MASK = LayerMask.GetMask("Ground");

        // Base parameters - revert back when disabling GodMode
        baseSpeed = speed;
        baseDashTime = dashTime;
        baseDashCooldown = dashCooldown;
        baseCurrentHealth = healthController.CurrentHealth;
        baseMagicRegen = magicController.magicRegen;
        baseLayer = gameObject.layer;
        baseSlerpRatio = slerpRatio;
        baseDefaultLayer = Neuromancer.Unit.HERO_LAYER_NAME;
        isGodModeEnabled = false;
    }

    private void OnEnable() {
        healthController.DeathEvent.AddListener(PlayerDie);
        animationController.onDeathReload.AddListener(Revive);
        GodModeEvent.AddListener(GOD);
        inputs.PlayerAction.Dash.performed += Dash;
    }

    private void OnDisable() {
        healthController.DeathEvent.RemoveListener(PlayerDie);
        animationController.onDeathReload.RemoveListener(Revive);
        GodModeEvent.AddListener(GOD);
        inputs.PlayerAction.Dash.performed -= Dash;
    }

    private void Update() {
        Movement();
        if (isDead) { inputs.PlayerAction.Disable(); }
        if (dashCooldownLeft > 0f) { dashCooldownLeft -= Time.deltaTime; }
        else { dashCooldownLeft = 0f; }
    }

    // I can't play legit
    private void GOD(bool godToggle) {
        if (godToggle)
        {
            isGodModeEnabled = true;
            dashCooldown = 0f;
            dashTime = 0.2f;
            speed = 20f;
            healthController.CurrentHealth = 999;
            magicController.magicRegen = 100f;
            slerpRatio = 0.7f;
            gameObject.layer = LayerMask.NameToLayer("NoCollide");
            defaultLayer = "NoCollide";
        }
        else
        {
            isGodModeEnabled = false;
            dashCooldown = baseDashCooldown;
            dashTime = baseDashTime;
            speed = baseSpeed;
            healthController.CurrentHealth = baseCurrentHealth;
            magicController.magicRegen = baseMagicRegen;
            slerpRatio = baseSlerpRatio;
            gameObject.layer = baseLayer;
            defaultLayer = baseDefaultLayer;
        }
    }

    private void Movement() {
        if (!isDashing) {
            Vector2 inputVector = inputs.PlayerAction.Movement.ReadValue<Vector2>();
            if (inputVector != Vector2.zero) {
                moveVector = Quaternion.Euler(0, mainCamera.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);
            }
            else { moveVector = new Vector3(0f, 0f, 0f); }
            Vector3 direction = moveVector;
            if (!GetGrounded()) {
                direction += new Vector3(0, gravity, 0);
                SlipChecker();
            }
            else { direction += new Vector3(0, -1, 0); }
            controller.Move(direction * Time.deltaTime * speed);
            transform.forward = Vector3.Slerp(transform.forward, new Vector3(direction.x, 0, direction.z), slerpRatio);
        }
    }

    public void EnableDash(bool enable) { dashEnabled = enable; }
    public bool GetDashEnabled() { return dashEnabled; }

    private void Dash(InputAction.CallbackContext callbackContext) {
        if (!dashEnabled && !isGodModeEnabled) { return; }
        if(!isDashing && (dashCooldownLeft <= 0) && (magicController.GetMagic() >= dashCost)) {
            gameObject.layer = LayerMask.NameToLayer(Neuromancer.Unit.HERO_NO_COLLIDE_LAYER_NAME);
            StartCoroutine(DashCoroutine());
            healthController.MakeInvulnerable(dashTime + 0.1f);
        }
    }

    private IEnumerator DashCoroutine() {
        dashCooldownLeft = dashCooldown;
        isDashing = true;
        BeginDashEvent.Invoke();
        magicController.DrainMagic(dashCost);

        Vector2 inputVector = inputs.PlayerAction.Movement.ReadValue<Vector2>();
        if (inputVector != Vector2.zero) {
            moveVector = Quaternion.Euler(0, mainCamera.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);
        }
        else { moveVector = transform.forward; }
        
        transform.forward = moveVector;
        Vector3 dashEffectPosition = new Vector3(transform.position.x, transform.position.y + dashFXOffset, transform.position.z);
        Instantiate(dashEffect, dashEffectPosition, transform.rotation);

        for (float dashTimeLeft = dashTime; dashTimeLeft > 0; dashTimeLeft -= Time.deltaTime) {
            controller.Move(moveVector * Time.deltaTime * dashSpeed);
            yield return null;
        }

        isDashing = false;
        gameObject.layer = LayerMask.NameToLayer(defaultLayer);
        EndDashEvent.Invoke();
    }

    private void PlayerDie() {
        inputs.PlayerAction.Disable();
        isDead = true;
        UnitGroupManager.current.enabled = false;
        unit.transform.tag = Neuromancer.Unit.DEAD_TAG;
        gameObject.layer = LayerMask.NameToLayer(Neuromancer.Unit.DEAD_LAYER_NAME);
        GetComponent<PlayerStatus>().ClearBuff();
    }

    private void Revive() {
        UnitGroupManager.current.enabled = true;
        inputs.PlayerAction.Enable();
        unit.transform.tag = Neuromancer.Unit.HERO_TAG;
        gameObject.layer = LayerMask.NameToLayer(Neuromancer.Unit.HERO_LAYER_NAME);
        isDead = false;
        if (!EmeraldAISystem.IgnoredTargetsList.Contains(transform)) { return; }
        EmeraldAISystem.IgnoredTargetsList.RemoveAll(Transform => Transform == transform);
    }

    public float GetSpeed() { return speed; }
    public void SetSpeed(float speed) { this.speed = speed; }
    public float GetBaseSpeed() { return baseSpeed; }

    private bool GetGrounded() {
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 10f, GROUND_MASK)) {
            float distanceToGround = hit.distance;
            if (distanceToGround < 1f) { return true; }
        }
        return false;
    }

    private void SlipChecker() {
        RaycastHit hit;
        Vector3 raySpawnPos = transform.position + Vector3.up * wallCheck.y;
        Vector3 forward = transform.forward * wallCheck.x;
        Vector3 back = -transform.forward * wallCheck.x;
        Vector3 right = transform.right * wallCheck.x;
        Vector3 left = -transform.right * wallCheck.x;
        Ray frontRay = new Ray(raySpawnPos, forward);
        Ray backRay = new Ray(raySpawnPos, back);
        Ray rightRay = new Ray(raySpawnPos, right);
        Ray leftRay = new Ray(raySpawnPos, left);

        if (Physics.Raycast(frontRay, out hit, wallCheck.x, GROUND_MASK)) {
            Slip(transform.forward);
        }

        if (Physics.Raycast(backRay, out hit, wallCheck.x, GROUND_MASK) ||
            Physics.Raycast(rightRay, out hit, wallCheck.x, GROUND_MASK) ||
            Physics.Raycast(leftRay, out hit, wallCheck.x, GROUND_MASK)) {
            Slip(hit.normal);
        }
    }

    private void Slip(Vector3 slipDir) {
        controller.Move(((slipDir * slipSpeed) + Vector3.down) * Time.deltaTime);
    }

    public bool AnimationGrounded() { return GetGrounded() || controller.isGrounded; }

    private void OnDrawGizmos() {
        Vector3 raySpawnPos = transform.position + Vector3.up * wallCheck.y;
        Vector3 forward = transform.forward * wallCheck.x;
        Vector3 back = -transform.forward * wallCheck.x;
        Vector3 right = transform.right * wallCheck.x;
        Vector3 left = -transform.right * wallCheck.x;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raySpawnPos, forward);
        Gizmos.DrawRay(raySpawnPos, right);
        Gizmos.DrawRay(raySpawnPos, back);
        Gizmos.DrawRay(raySpawnPos, left);
    }
}