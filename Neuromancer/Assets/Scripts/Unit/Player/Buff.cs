using UnityEngine;

public enum BuffToken {
    SPEED1 = 0,
    HEAL1,
    ATTACK1,
    DEFENSE1,
    PULL1,
    PUSH1,
}

public enum BuffType {
    SPEED = 0,
    HEAL = 1,
    ATTACK = 2,
    DEFENSE = 3,
    PULL = 4,
    PUSH = 5,
}

public enum ModifierType {
    ADDITIVE = 0,
    MULTIPLICATIVE,
}

[CreateAssetMenu(fileName = "NewBuff", menuName = "Player/Buff Scriptable Object")]
public class Buff : ScriptableObject
{
    [Tooltip("Each buff scriptable object should have a unique token")]
    public BuffToken token;
    public BuffType buffType;
    [HideInInspector] public ModifierType modType = ModifierType.MULTIPLICATIVE;
    [Tooltip("The lower the stronger for attack/defense/speed; The higher the stronger for heal/push/pull")]
    public float modAmount;
    [Tooltip("This duration set here does not affect AOE. AOE applies the buff as long as itself exists")]
    public float duration;
    [Tooltip("The amount of additional cost added when equipped on spell")]
    public float cost;
    [Tooltip("Particle effect that appears on a unit while buff is applied")]
    public GameObject activeParticle;
    [Tooltip("Particle effect that appears when the buff collectible is collected")]
    public GameObject collectParticle;

    [Header("UI References")]
    public Sprite icon;
    public string description;

    [ExecuteInEditMode]
    private void OnValidate() {
        switch (buffType) {
            case BuffType.SPEED:
            case BuffType.DEFENSE:
            case BuffType.ATTACK:
            case BuffType.PULL:
            case BuffType.PUSH:
                modAmount = Mathf.Clamp(modAmount, 0, 1);
                break;
            case BuffType.HEAL:
                modAmount = Mathf.Clamp(modAmount, 0, modAmount);
                break;
        }
        duration = Mathf.Clamp(duration, 0, duration);
        modType = ModifierType.MULTIPLICATIVE;
    }
}
