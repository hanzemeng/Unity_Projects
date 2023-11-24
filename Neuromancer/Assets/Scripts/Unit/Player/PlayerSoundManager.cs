using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private PlayerSpellCast spellCastManager;
    private GameObject rightStep;
    private GameObject leftStep;
    private GameObject spellCast;
    private GameObject switchSpell;
    private GameObject hit;
    private GameObject die;

    private void Awake() {
        spellCastManager = GetComponent<PlayerSpellCast>();
    }

    private void Start() {
        rightStep = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_WALK, transform);
        leftStep = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_WALK1, transform);
        spellCast = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_SPELL_HIT, transform);
        switchSpell = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_SWITCH_SPELL, transform);
        hit = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_GOT_HIT, transform);
        die = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_DEATH, transform);
    }

    private void OnEnable() {
        spellCastManager.onPlayerSwitchSpell.AddListener(SwitchSpell);
    }
    
    private void OnDisable() {
        spellCastManager.onPlayerSwitchSpell.RemoveListener(SwitchSpell);
    }

    private void RightStep() { AudioManager.instance.PlayMusic(rightStep); }
    private void LeftStep() { AudioManager.instance.PlayMusic(leftStep); }
    private void SpellCast() { AudioManager.instance.PlayMusic(spellCast); }
    private void SwitchSpell(SpellScriptableObject spell) { AudioManager.instance.PlayMusic(switchSpell); }
    private void Hit() { AudioManager.instance.PlayMusic(hit); }
    private void Die() { AudioManager.instance.PlayMusic(die); }
}
