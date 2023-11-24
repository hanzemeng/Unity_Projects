using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using EmeraldAI.Example;

public class PlayerSpellCast : MonoBehaviour
{
    [SerializeField] private Transform spellSpawnPoint;
    [System.NonSerialized] public UnityEvent onPlayerCastSpell = new UnityEvent();
    [System.NonSerialized] public UnityEvent onPlayerHoldSpell = new UnityEvent();
    [System.NonSerialized] public UnityEvent onPlayerReleaseSpell = new UnityEvent();
    [System.NonSerialized] public UnityEvent<SpellScriptableObject> onPlayerSwitchSpell = new UnityEvent<SpellScriptableObject>();
    private int index = 0;
    private PlayerInputs inputs;
    private Spell equippedSpell = null;
    private UnitMagic magicController;
    private EmeraldAIPlayerHealth healthController;
    private ReticleController reticle;
    private Coroutine castingCoroutine = null;
    private bool isCasting = false;
    private GameObject spellDenySound;

    private void Awake() {
        magicController = GetComponent<UnitMagic>();
        healthController = GetComponent<EmeraldAIPlayerHealth>();
        inputs = PlayerInputManager.playerInputs;
        inputs.PlayerAction.Enable();
    }

    private void Start() {
        reticle = ReticleController.current;
        spellDenySound = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_SPELL_DENY, transform);
        if (equippedSpell == null) { SelectSpell(0); }
    }

    private void OnEnable() {
        magicController.onBeginCastEvent.AddListener(StartCasting);
        magicController.onEndCastEvent.AddListener(StopCasting);
        magicController.onZeroMagicEvent.AddListener(ZeroMagic);
        healthController.DamageEvent.AddListener(InterruptCasting);
    }

    private void OnDisable() {
        magicController.onBeginCastEvent.RemoveListener(StartCasting);
        magicController.onEndCastEvent.RemoveListener(StopCasting);
        magicController.onZeroMagicEvent.RemoveListener(ZeroMagic);
        healthController.DamageEvent.RemoveListener(InterruptCasting);
    }

    private void Update() {
        float mouseScroll = inputs.PlayerAction.SelectSpell.ReadValue<float>();
        if(mouseScroll > 0) {
            SelectSpell(index - 1);
        } else if (mouseScroll < 0) {
            SelectSpell(index + 1);
        }
    }

    public void SelectSpell(int newIndex) {
        if (isCasting) { return; }
        int count = PlayerInventory.current.GetSpells().Count;
        if (count == 0) { return; }

        if (equippedSpell != null) { equippedSpell.Reset(); }
        index = (newIndex < 0 ? newIndex + count : newIndex) % count;
        equippedSpell = PlayerInventory.current.GetSpells()[index];
        if (equippedSpell == null) { return; }
        equippedSpell.Initialize();
        onPlayerSwitchSpell.Invoke(equippedSpell.spellSpecs);
    }

    private void StartCasting(float magic) {
        if (CanCast()) {
            castingCoroutine = StartCoroutine(Casting());
            isCasting = true;
        }
        if (isCasting && (equippedSpell.spellSpecs.type == SpellType.BEAM || equippedSpell.spellSpecs.type == SpellType.CONE)) {
            onPlayerHoldSpell.Invoke();
        }
    }

    private IEnumerator Casting() {
        if (equippedSpell.spellSpecs.type == SpellType.AOE) {
            magicController.DrainMagic(equippedSpell.GetCost());
        }
        Transform target = null;
        switch (equippedSpell.spellSpecs.type) {
            case SpellType.AOE:
                target = reticle.GetReticleTransform();
                break;
            case SpellType.BEAM:
                target = spellSpawnPoint;
                break;
            case SpellType.CONE:
                target = spellSpawnPoint;
                break;
            default:
                break;
        }
        while (true) {
            if (equippedSpell.spellSpecs.type == SpellType.AOE
                || equippedSpell.spellSpecs.type == SpellType.BEAM
                || equippedSpell.spellSpecs.type == SpellType.CONE) {
                equippedSpell.Cast(target);
                magicController.DrainMagic(equippedSpell.GetCost() * Time.deltaTime);
            }
            yield return null;
        }
    }

    private void StopCasting(float magic) {
        if (isCasting) {
            StopCoroutine(castingCoroutine);
            if (equippedSpell.spellSpecs.type == SpellType.AOE) {
                onPlayerCastSpell.Invoke();
                transform.LookAt(equippedSpell.GetSpellTransform());
                equippedSpell.Reset();
            }
            else if (equippedSpell.spellSpecs.type == SpellType.BEAM) {
                onPlayerReleaseSpell.Invoke();
                equippedSpell.Reset();
            }
            else if (equippedSpell.spellSpecs.type == SpellType.CONE) {
                onPlayerReleaseSpell.Invoke();
                equippedSpell.Reset();
            }
        }
        isCasting = false;
    }

    private void InterruptCasting() {
        if (isCasting && (equippedSpell.spellSpecs.type == SpellType.BEAM || equippedSpell.spellSpecs.type == SpellType.CONE)) {
            StopCasting(0f); // argument doesn't matter
        }
    }

    private void ZeroMagic(bool zeroMagic) {
        StopCasting(0f);
        if (equippedSpell.spellSpecs.type != SpellType.AOE) { PlaySpellDenyEffects(); }
    }

    public bool CanCast() {
        if (isCasting && (equippedSpell.spellSpecs.type != SpellType.MISSILE)) { return false; }
        if (equippedSpell == null) { return false; }
        if (equippedSpell.spellSpecs.type == SpellType.AOE || equippedSpell.spellSpecs.type == SpellType.MISSILE) {
            if (magicController.GetMagic() < equippedSpell.GetCost()) {
                PlaySpellDenyEffects();
                return false;
            }
        }
        return true;
    }

    private void PlaySpellDenyEffects() {
        AudioManager.instance.PlayMusic(spellDenySound);
        Vector3 position;
        if (equippedSpell.spellSpecs.type == SpellType.AOE || equippedSpell.spellSpecs.type == SpellType.MISSILE) {
            position = reticle.GetReticleTransform().position;
        }
        else { position = spellSpawnPoint.position; }
        Instantiate(equippedSpell.spellSpecs.denyParticle, position, equippedSpell.spellSpecs.denyParticle.transform.rotation);
    }
}
