using UnityEngine;

public enum SpellType {
    BEAM = 0,       // a single beam
    CONE,       // a cone / spread out beam
    AOE,        // area of effect
    MISSILE,    // missiles
}

[CreateAssetMenu(fileName = "NewSpell", menuName = "Player/Spell Scriptable Object")]
public class SpellScriptableObject : ScriptableObject
{
    public GameObject spellPrefab;
    public SpellType type = 0;
    public float baseCost = 0f;
    public float drainAmount = 0f;
    [Tooltip("For beam, cone, and AOE. Max scale compared to original")]
    public float maxRadius = 0f;
    [Tooltip("For beam, cone, and AOE. The rate at which the spell area grows when held down")]
    public float growthRate = 0f;
    [Tooltip("For AOE. The duration of the spell")]
    public float duration = 0f;
    [Tooltip("For Missile. The spell cool down in seconds")]
    public float coolDown = 0f;
    [Header("UI References")]
    public new string name;
    public Sprite sprite;
    public string info;
    public GameObject denyParticle;
}
