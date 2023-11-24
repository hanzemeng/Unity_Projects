using System.Collections.Generic;
using EmeraldAI;
using UnityEngine;

public class SpellMissile : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f;
    private GameObject explosionSound;
    private int TARGET_MASK;

    private void Awake() {
        TARGET_MASK = LayerMask.GetMask(Neuromancer.Unit.HERO_LAYER_NAME, Neuromancer.Unit.ENEMY_LAYER_NAME, Neuromancer.Unit.ALLY_LAYER_NAME);
    }

    public void Launch(Transform center, MissileSpell spell) {
        explosionSound = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_MISSILE_EXPLOSION, transform);
        AudioManager.instance.PlayMusic(explosionSound);
        Transform target = null;
        if (Neuromancer.Unit.IsUnit(center)) { target = center; }
        else {
            Collider[] cols = Physics.OverlapCapsule(transform.position, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), radius, TARGET_MASK);
            if (cols.Length > 0) { target = SelectTarget(cols); }
        }
        if (!target) { return; }
        if (Neuromancer.Unit.IsEnemy(target)) {
            UnitMentalStamina msController = target.GetComponent<UnitMentalStamina>();
            msController.DrainMentalStamina(spell.spellSpecs.drainAmount * PlayerController.player.spellDamageModifier);
            Neuromancer.NPCUnit npcUnit = target.GetComponent<Neuromancer.NPCUnit>();
            if (npcUnit != null && npcUnit.EmeraldComponent.CurrentTarget == null)
            {
                npcUnit.IssueCommand(new Command(CommandType.ATTACK_TARGET, PlayerController.player.transform));
            }
        }
        List<Buff> buffs = spell.GetBuffs();
        if (buffs != null) {
            foreach (Buff b in buffs) {
                target.gameObject.GetComponent<UnitStatus>()?.AddBuff(b, PlayerController.player.transform);
                target.gameObject.GetComponent<PlayerStatus>()?.AddBuff(b, PlayerController.player.transform);
            }
        }
    }

    private Transform SelectTarget(Collider[] colliders) {
        int minHp = 999;
        float minMs = 999;
        Transform targetAlly = null;
        Transform targetEnemy = null;
        foreach (Collider c in colliders) {
            Transform t = c.transform;
            if (Neuromancer.Unit.IsAlly(t)) {
                int hp = t.GetComponent<EmeraldAISystem>().CurrentHealth;
                if (hp < minHp) {
                    minHp = hp;
                    targetAlly = t;
                }
            }
            else if (Neuromancer.Unit.IsEnemy(t)) {
                float ms = t.GetComponent<UnitMentalStamina>().GetMentalStamina();
                if (ms < minMs) {
                    minMs = ms;
                    targetEnemy = t;
                }
            }
        }
        if (targetEnemy != null) { return targetEnemy; }
        else if (targetAlly != null) { return targetAlly; }
        else { return colliders[0].transform; }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, radius);
    }
}