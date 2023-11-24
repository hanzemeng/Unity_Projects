using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory current;
    [Tooltip("Array to store all the spell scriptable objects")]
    [SerializeField] private SpellScriptableObject[] spells;
    [Tooltip("Array to store all the buff scriptable objects")]
    [SerializeField] private Buff[] buffs;
    private List<Spell> knownSpells = new List<Spell>();
    private List<Buff> unsetBuffs = new List<Buff>();
    private PlayerSpellCast spellCastManager;

    [System.NonSerialized] public UnityEvent<Spell> onNewSpellEvent = new UnityEvent<Spell>();
    [System.NonSerialized] public UnityEvent<List<Buff>> onUnsetBuffChangeEvent = new UnityEvent<List<Buff>>();
    [System.NonSerialized] public UnityEvent onInventoryReset = new UnityEvent();

    private void Awake() {
        if (current == null) { current = this; }
        else { Destroy(gameObject); }
        spellCastManager = GetComponent<PlayerSpellCast>();
    }

    public Spell AddSpell(SpellType type) {
        foreach (Spell s in knownSpells) {
            if (s.spellSpecs.type == type) { return s; }
        }

        foreach (SpellScriptableObject s in spells) {
            if (s.type == type) {
                Spell newSpell;
                switch (type) {
                    case SpellType.AOE:
                        newSpell = new AOESpell(s);
                        break;
                    case SpellType.MISSILE:
                        newSpell = new MissileSpell(s);
                        break;
                    case SpellType.BEAM:
                        newSpell = new BeamSpell(s);
                        break;
                    case SpellType.CONE:
                        newSpell = new ConeSpell(s);
                        break;
                    default:
                        return null;
                }
                knownSpells.Add(newSpell);
                onNewSpellEvent.Invoke(newSpell);
                if (knownSpells.Count == 1) {
                    spellCastManager.SelectSpell(0);
                    PartyManager.current.ActivateUI();
                }
                return newSpell;
            }
        }
        return null;
    }

    public bool AddBuff(BuffToken token) {
        if (unsetBuffs.Count + 1 > SpellMenuManager.current.MaxUnsetBuffs()) { return false; }
        foreach (Buff b in buffs) {
            if (b.token == token) {
                unsetBuffs.Add(b);
                onUnsetBuffChangeEvent.Invoke(unsetBuffs);
                return true;
            }
        }
        return false;
    }

    private Buff GetBuff(BuffToken token) {
        foreach (Buff b in buffs) {
            if (b.token == token) { return b; }
        }
        return null;
    }

    public void BuffDragged(Spell sourceSpell, Spell targetSpell, Buff buff) {
        
        if (sourceSpell == targetSpell) { return; }

        Buff origBuff = buff;
        foreach (Buff b in buffs) {
            if (b.token == buff.token) {
                origBuff = b;
                break;
            }
        }

        if (targetSpell != null) {
            if (!targetSpell.AddBuff(origBuff)) { return; }
        }
        else {
            unsetBuffs.Add(origBuff);
            onUnsetBuffChangeEvent.Invoke(unsetBuffs);
        }

        if (sourceSpell != null) {
            if (!sourceSpell.RemoveBuff(buff)) {
                Debug.Log("Untracked Spell Buff! This should not be happening!");
            }
        }
        else {
            unsetBuffs.Remove(origBuff);
            onUnsetBuffChangeEvent.Invoke(unsetBuffs);
        }
    }

    public List<Spell> GetSpells() { return knownSpells; }
    public List<Buff> GetUnsetBuffs() { return unsetBuffs; }

    public void Reset() {
        knownSpells.Clear();
        unsetBuffs.Clear();
        onInventoryReset.Invoke();
        AddSpell(SpellType.MISSILE);
    }

    class SpellData {
        public string name;
        public List<string> buffs;

        public SpellData(string n, List<Buff> lb) {
            name = n;
            buffs = new List<string>();
            foreach (Buff b in lb) {
                buffs.Add(System.Enum.GetName(typeof(BuffToken), b.token));
            }
        }

        public string ToJsonString() { return JsonUtility.ToJson(this); }
    }

    class Data {
        public List<string> spells;
        public List<string> buffs;
        public Data(List<Spell> ls, List<Buff> lb) {
            spells = new List<string>();
            buffs = new List<string>();
            foreach (Spell s in ls) {
                SpellData sd = new SpellData(System.Enum.GetName(typeof(SpellType), s.spellSpecs.type), s.GetBuffs());
                spells.Add(sd.ToJsonString());
            }
            foreach (Buff b in lb) {
                buffs.Add(System.Enum.GetName(typeof(BuffToken), b.token));
            }
        }
    }

    public string GetData() {
        Data d = new Data(knownSpells, unsetBuffs);
        return JsonUtility.ToJson(d);
    }

    public void SetData(string s) {
        Data d = JsonUtility.FromJson<Data>(s);
        foreach (string sps in d.spells) {
            SpellData sd = JsonUtility.FromJson<SpellData>(sps);
            Spell spell = AddSpell((SpellType)System.Enum.Parse(typeof(SpellType), sd.name));
            if (spell == null) { continue; }
            foreach (string bn in sd.buffs) {
                Buff b = GetBuff((BuffToken)System.Enum.Parse(typeof(BuffToken), bn));
                BuffDragged(null, spell, b);
            }
        }
        foreach (string bf in d.buffs) {
            AddBuff((BuffToken)System.Enum.Parse(typeof(BuffToken), bf));
        }
    }
}
