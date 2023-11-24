using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI.Example;

namespace EmeraldAI
{
    //This script will automatically be added to player targets. You can customize the DamagePlayerStandard function
    //or create your own. Ensure that it will be called within the SendPlayerDamage function. This allows users to customize
    //how player damage is received and applied without having to modify any main system scripts. The EmeraldComponent can
    //be used for added functionality such as only allowing blocking if the received AI is using the Melee Weapon Type.
    public class EmeraldAIPlayerDamage : MonoBehaviour
    {
        public List<string> ActiveEffects = new List<string>();
        public bool IsDead = false;
        private PlayerAnimation animationController;
        private EmeraldAIPlayerHealth playerHealth;

        private void Awake() {
            animationController = GetComponent<PlayerAnimation>();
            playerHealth = GetComponent<EmeraldAIPlayerHealth>();
        }

        private void OnEnable() {
            animationController.onDeathReload.AddListener(Revive);
        }

        private void OnDisable() {
            animationController.onDeathReload.RemoveListener(Revive);
        }

        public void SendPlayerDamage(int DamageAmount, Transform Target, EmeraldAISystem EmeraldComponent, bool CriticalHit = false, bool isLast = true)
        {
            if (IsDead) { return; }
            //The standard damage function that sends damage to the Emerald AI demo player
            DamagePlayerStandard(DamageAmount, isLast);

            //Creates damage text on the player's position, if enabled.
            CombatTextSystem.Instance.CreateCombatText(DamageAmount, transform.position, CriticalHit, false, true);
        }

        private void DamagePlayerStandard(int DamageAmount, bool isLast)
        {
            playerHealth.DamagePlayer(DamageAmount, isLast);
            if (playerHealth.CurrentHealth <= 0) { IsDead = true; }
        }

        private void Revive() {
            IsDead = false;
        }
    }
}
