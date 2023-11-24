using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAOE : MonoBehaviour
{
    private AOESpell spell;
    private List<Buff> buffs = new List<Buff>();
    private List<Transform>[] appliedBuff;
    private bool activated = false;
    private ParticleSystem particle;
    private ParticleSystem parentParticle;
    private ParticleSystem.MainModule mainModule;
    [Tooltip("How much the particles' alpha fade as timer goes out")]
    [SerializeField] private float fadeAmount = 0.3f;
    private GameObject channelSound;
    private GameObject activeSound;

    private void Awake() {
        parentParticle = GetComponent<ParticleSystem>();
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
        mainModule = particle.main;
        Color color = mainModule.startColor.color;
        color.a = fadeAmount;
        mainModule.startColor = color;
    }

    private void Start() {
        channelSound = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_SPELL_CHARGE, transform);
        activeSound = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_SPELL_ACTIVE, transform);
        AudioManager.instance.PlayMusic(channelSound);
    }

    public void Activate(AOESpell spell) {
        Color color = mainModule.startColor.color;
        color.a = 1;
        mainModule.startColor = color;
        this.spell = spell;
        buffs = spell.GetBuffs();
        appliedBuff = new List<Transform>[buffs.Count];
        for (int i = 0; i < buffs.Count; i++) { appliedBuff[i] = new List<Transform>(); }
        activated = true;
        AudioManager.instance.PauseMusic(channelSound);
        StartCoroutine(SelfDestruct(spell.spellSpecs.duration));
    }

    private IEnumerator SelfDestruct(float duration) {
        float timer = 0f;
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayMusic(activeSound, isfadeOut: true);
        while (timer < duration) {
            Color color = mainModule.startColor.color;
            color.a = 1 - timer * fadeAmount * 0.35f;
            mainModule.startColor = color;
            timer += Time.deltaTime;
            yield return null;
        }
        parentParticle.Stop();
        particle.Stop();
        activated = false;
        AudioManager.instance.PauseMusic(activeSound);
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Neuromancer.NPCUnit npcUnit = other.gameObject.GetComponent<Neuromancer.NPCUnit>();
        if (npcUnit != null && Neuromancer.Unit.IsEnemy(other.transform) && npcUnit.EmeraldComponent.CurrentTarget == null)
        {
            npcUnit.IssueCommand(new Command(CommandType.ATTACK_TARGET, PlayerController.player.transform));
        }
    }

    private void OnTriggerStay(Collider col) {
        if (activated) {
            UnitMentalStamina msController = col.gameObject.GetComponent<UnitMentalStamina>();
            if (msController != null && Neuromancer.Unit.IsEnemy(col.transform)) {
                msController.DrainMentalStamina(spell.spellSpecs.drainAmount * PlayerController.player.spellDamageModifier * Time.deltaTime);
            }
            for (int i = 0; i < buffs.Count; i++) {
                if (!appliedBuff[i].Contains(col.transform)) {
                    col.gameObject.GetComponent<UnitStatus>()?.AddBuff(buffs[i], transform);
                    col.gameObject.GetComponent<PlayerStatus>()?.AddBuff(buffs[i], transform);
                    appliedBuff[i].Add(col.transform);
                }
            }
        }
        else {
            for (int i = 0; i < buffs.Count; i++) {
                if (appliedBuff[i].Contains(col.transform)) {
                    col.gameObject.GetComponent<UnitStatus>()?.RemoveBuff(buffs[i]);
                    col.gameObject.GetComponent<PlayerStatus>()?.RemoveBuff(buffs[i]);
                    appliedBuff[i].Remove(col.transform);
                }
            }
        }
    }

    private void OnTriggerExit(Collider col) {
        for (int i = 0; i < buffs.Count; i++) {
            if (appliedBuff[i].Contains(col.transform)) {
                col.gameObject.GetComponent<UnitStatus>()?.RemoveBuff(buffs[i]);
                col.gameObject.GetComponent<PlayerStatus>()?.RemoveBuff(buffs[i]);
                appliedBuff[i].Remove(col.transform);
            }
        }
    }
}
