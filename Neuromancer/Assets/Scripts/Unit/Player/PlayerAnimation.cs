using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using EmeraldAI.Example;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerSpellCast spellCastController;
    private EmeraldAIPlayerHealth healthController;
    private PlayerController playerController;
    private bool forceGrounded = false;
    [System.NonSerialized] public UnityEvent onDeathReload = new UnityEvent();

    private void Awake() {
        animator = GetComponent<Animator>();
        spellCastController = GetComponent<PlayerSpellCast>();
        healthController = GetComponent<EmeraldAIPlayerHealth>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start() {
        LevelManager.levelManager.onNewSceneLoadAsyncEvent.AddListener(ForceGrounded);
        LevelManager.levelManager.onNewSceneEvent.AddListener(UnforceGroundedDelay);
    }

    private void OnEnable() {
        spellCastController.onPlayerCastSpell.AddListener(TriggerCastAnim);
        healthController.DeathEvent.AddListener(TriggerDeathAnim);
        healthController.damageAnimationEvent.AddListener(TriggerHitAnim);
        spellCastController.onPlayerHoldSpell.AddListener(SetHoldAnim);
        spellCastController.onPlayerReleaseSpell.AddListener(SetUnholdAnim);
        playerController.BeginDashEvent.AddListener(SetDash);
        playerController.EndDashEvent.AddListener(UnsetDash);
        LevelManager.levelManager?.onNewSceneLoadAsyncEvent.AddListener(ForceGrounded);
        LevelManager.levelManager?.onNewSceneEvent.AddListener(UnforceGroundedDelay);
    }

    private void OnDisable() {
        spellCastController.onPlayerCastSpell.RemoveListener(TriggerCastAnim);
        healthController.DeathEvent.RemoveListener(TriggerDeathAnim);
        healthController.damageAnimationEvent.RemoveListener(TriggerHitAnim);
        spellCastController.onPlayerHoldSpell.RemoveListener(SetHoldAnim);
        spellCastController.onPlayerReleaseSpell.RemoveListener(SetUnholdAnim);
        playerController.BeginDashEvent.RemoveListener(SetDash);
        playerController.EndDashEvent.RemoveListener(UnsetDash);
        LevelManager.levelManager.onNewSceneLoadAsyncEvent.RemoveListener(ForceGrounded);
        LevelManager.levelManager.onNewSceneEvent.RemoveListener(UnforceGroundedDelay);
    }

    private void Update() {
        if (playerController.moveVector != Vector3.zero) { animator.SetBool("isMoving", true); }
        else { animator.SetBool("isMoving", false); }

        if (!forceGrounded) {
            if (playerController.AnimationGrounded()) { animator.SetBool("isFalling", false); }
            else { animator.SetBool("isFalling", true); }
        }

        animator.SetFloat("motionX", PlayerController.player.moveVector.x);
        animator.SetFloat("motionY", PlayerController.player.moveVector.z);
    }

    private void TriggerDeathAnim() {
        animator.SetTrigger("die");
        StartCoroutine(TriggerDeathEvent());
    }

    private IEnumerator TriggerDeathEvent() {
        yield return new WaitForSeconds(5);
        LevelManager.levelManager.ReloadCurrent();
        yield return new WaitForSeconds(1.5f);
        onDeathReload.Invoke();
    }

    private void TriggerCastAnim() { animator.SetTrigger("castSpell"); }
    private void SetHoldAnim() { animator.SetBool("isHoldingSpell", true); }
    private void SetUnholdAnim() { animator.SetBool("isHoldingSpell", false); }
    private void TriggerHitAnim() { animator.SetTrigger("onHit"); }
    private void SetDash() { animator.SetBool("isDashing", true); }
    private void UnsetDash() { animator.SetBool("isDashing", false); }

    private void ForceGrounded(string _) {
        forceGrounded = true;
        animator.SetBool("isFalling", false);
    }

    private void UnforceGroundedDelay() { Invoke(nameof(UnforceGrounded), 1f); }
    private void UnforceGrounded() { forceGrounded = false; }
}
