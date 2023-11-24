using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EmeraldAI.Example
{
    /// <summary>
    /// An example health script that the EmeraldAIPlayerDamage script calls.
    /// Various events can be created and used to cause damage to a 3rd party character controllers via the inspector.
    /// You can also edit the EmeraldAIPlayerDamage script directly and add custom functions.
    /// </summary>
    public class EmeraldAIPlayerHealth : MonoBehaviour
    {
        public int StartingHealth = 100; 
        public int CurrentHealth = 100;
        [Tooltip("Regenerate 1 hp every [frequency] seconds. Set to 0 to disable regen")]
        public float healthRegenFrequency = 1.5f;
        [SerializeField] private float invulnerabilityDuration = 0.3f; [Space]

        public UnityEvent DamageEvent;
        public UnityEvent damageAnimationEvent;
        public UnityEvent DeathEvent;
        public UnityEvent HealEvent;
        private PlayerAnimation animationController;
        private bool invulnerable = false;
        private Flashy flash;
        private Coroutine healthRegenCoroutine;
        
        [HideInInspector]
        public float defenseModifier = 1f;

        private void Awake() {
            animationController = GetComponent<PlayerAnimation>();
            CurrentHealth = StartingHealth;
            if (!TryGetComponent<Flashy>(out flash)) {
                flash = gameObject.AddComponent<Flashy>();
            }
            healthRegenCoroutine = StartCoroutine(HealthRegen());
        }

        private void OnEnable() {
            animationController.onDeathReload.AddListener(Revive);
        }

        private void OnDisable() {
            animationController.onDeathReload.RemoveListener(Revive);
        }

        public void DamagePlayer (int DamageAmount, bool isLast)
        {
            if (invulnerable) { return; }

            CurrentHealth -= (int) (DamageAmount * defenseModifier);
            DamageEvent.Invoke();
            damageAnimationEvent.Invoke();
            flash.Flash(Color.red);
            if (isLast) { MakeInvulnerable(invulnerabilityDuration); }
            if (CurrentHealth <= 0) {
                StopCoroutine(healthRegenCoroutine);
                DeathEvent.Invoke();
            }
        }

        public void HealPlayer(int healAmount) {
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, StartingHealth);
            HealEvent.Invoke();
        }

        private void Revive() {
            CurrentHealth = StartingHealth;
            healthRegenCoroutine = StartCoroutine(HealthRegen());
            HealEvent.Invoke();
        }

        public void MakeInvulnerable(float duration) {
            StartCoroutine(Invulnerable(duration));
        }

        private IEnumerator Invulnerable(float duration) {
            invulnerable = true;
            yield return new WaitForSeconds(duration);
            invulnerable = false;
        }

        private IEnumerator HealthRegen() {
            if (healthRegenFrequency <= 0) { yield break; }
            while (true) {
                if (healthRegenFrequency > 0) {
                    HealPlayer(1);
                    yield return new WaitForSeconds(healthRegenFrequency);
                }
                yield return null;
            }
        }
    }
}
