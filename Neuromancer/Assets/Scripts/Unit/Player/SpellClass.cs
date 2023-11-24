using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spell
{
    public SpellScriptableObject spellSpecs {get; private set;}
    protected GameObject spellObject;
    protected List<Buff> buffs = new List<Buff>();
    protected int buffSlot = 3;
    
    [System.NonSerialized] public UnityEvent<List<Buff>> onBuffChangeEvent = new UnityEvent<List<Buff>>();
    
    public Spell(SpellScriptableObject spellSpecs) {
        this.spellSpecs = spellSpecs;
    }

    public virtual void Cast(Transform target) {}
    public virtual float GetCost() { return spellSpecs.baseCost + GetBuffCost(); }
    public virtual void Reset() {}
    public virtual Transform GetSpellTransform() { return null; }
    public virtual void Initialize() {}
    public List<Buff> GetBuffs() { return buffs; }
    public int GetBuffSlot() { return buffSlot; }
    
    public virtual bool AddBuff(Buff buff) {
        if (buffSlot < 1) { return false; }
        buffs.Add(buff);
        buffSlot--;
        onBuffChangeEvent.Invoke(buffs);
        return true;
    }

    public bool RemoveBuff(Buff buff) {
        foreach (Buff b in buffs) {
            if (b.token == buff.token) {
                buffs.Remove(b);
                buffSlot++;
                onBuffChangeEvent.Invoke(buffs);
                return true;
            }
        }
        return false;
    }

    protected float GetBuffCost() {
        float cost = 0f;
        foreach (Buff b in buffs) { cost += b.cost; }
        return cost;
    }
}

public class AOESpell : Spell
{
    private float radius = 1f;
    private Vector3 baseScale;
    public AOESpell(SpellScriptableObject spellSpecs) : base(spellSpecs) {}
    public override void Cast(Transform target) {
        if (spellObject == null) {
            spellObject = Object.Instantiate(spellSpecs.spellPrefab, target.position, 
                                             spellSpecs.spellPrefab.transform.rotation);
            baseScale = spellObject.transform.localScale;
        }
        else {
            radius = Mathf.Clamp(radius + spellSpecs.growthRate * Time.deltaTime, 1f, spellSpecs.maxRadius);
            spellObject.transform.position = target.position;
            spellObject.transform.localScale = baseScale * radius;
        }
    }

    public override float GetCost() { return spellSpecs.baseCost * radius + GetBuffCost(); }

    public override void Reset() {
        if (spellObject == null) { return; }
        SpellAOE aoe = spellObject.GetComponent<SpellAOE>();
        aoe.Activate(this);
        radius = 1f;
        spellObject = null;
    }

    public override bool AddBuff(Buff buff) {
        if (buffSlot < 1) { return false; }
        Buff clone = Object.Instantiate(buff);
        clone.duration = 120f;
        if (clone.buffType == BuffType.PULL || clone.buffType == BuffType.PUSH) {
            clone.modAmount /= 50f;
        }
        buffs.Add(clone);
        buffSlot--;
        onBuffChangeEvent.Invoke(buffs);
        return true;
    }

    public override Transform GetSpellTransform() { return spellObject.transform; }
}

public class MissileSpell : Spell
{
    private UnitMagic magicController;
    private PlayerSpellCast spellCastController;
    private float timer = 0f;
    private bool canCast = true;
    Coroutine currentCoroutine;

    public MissileSpell(SpellScriptableObject spellSpecs) : base(spellSpecs) {}

    public override void Initialize() {
        ReticleController.current.onSpellSelectionEvent.AddListener(Cast);
        magicController = PlayerController.player.GetComponent<UnitMagic>();
        currentCoroutine = PlayerController.player.StartCoroutine(Timer());
        spellCastController = PlayerController.player.GetComponent<PlayerSpellCast>();
    }

    // Missile handles its own Cast
    private new void Cast(Transform target) {
        if (!spellCastController.CanCast()) { return; }
        SpellMissile missile = GameObject.Instantiate(spellSpecs.spellPrefab, target.position, spellSpecs.spellPrefab.transform.rotation).GetComponent<SpellMissile>();
        missile.Launch(target, this);
        magicController.DrainMagic(GetCost());
        canCast = false;
    }

    public override void Reset() { 
        ReticleController.current.onSpellSelectionEvent.RemoveListener(Cast);
        PlayerController.player.StopCoroutine(currentCoroutine);
    }

    private IEnumerator Timer() {
        while(true) {
            if (!canCast) { timer += Time.deltaTime; }
            if (timer > spellSpecs.coolDown) {
                canCast = true;
                timer = 0f;
            }
            yield return null;
        }
    }

    public override bool AddBuff(Buff buff) {
        if (buffSlot < 1) { return false; }
        Buff clone = Object.Instantiate(buff);
        if (clone.buffType == BuffType.HEAL) {
            clone.modType = ModifierType.ADDITIVE;
            clone.duration = 0f;
        }
        else if (clone.buffType == BuffType.PULL || clone.buffType == BuffType.PUSH) {
            clone.duration = 0f;
            clone.modAmount *= 4f;
            clone.modType = ModifierType.ADDITIVE;
        }
        buffs.Add(clone);
        buffSlot--;
        onBuffChangeEvent.Invoke(buffs);
        return true;
    }
}

public class BeamSpell : Spell
{
    private float scale = 1f;
    private SpellBeam beam;
    public BeamSpell(SpellScriptableObject spellSpecs) : base(spellSpecs) {}

    public override void Cast(Transform target) {
        if (spellObject == null) {
            spellObject = Object.Instantiate(spellSpecs.spellPrefab, target.position, 
                                             spellSpecs.spellPrefab.transform.rotation);
            beam = spellObject.GetComponent<SpellBeam>();
            beam.Initialize(this, target);
        }
        else {
            scale = Mathf.Clamp(scale + spellSpecs.growthRate * Time.deltaTime, 1f, spellSpecs.maxRadius);
            beam.ModLength(scale);
        }
    }

    public override float GetCost() { return spellSpecs.baseCost * scale + GetBuffCost(); }

    public override void Reset() {
        if (spellObject == null) { return; }
        Object.Destroy(spellObject);
        scale = 1f;
    }

    public override bool AddBuff(Buff buff) {
        if (buffSlot < 1) { return false; }
        Buff clone = Object.Instantiate(buff);
        clone.duration = 120f;
        if (clone.buffType == BuffType.HEAL) { clone.modAmount *= 2; }
        else if (clone.buffType == BuffType.PULL || clone.buffType == BuffType.PUSH) {
            clone.modAmount /= 50f;
        }
        buffs.Add(clone);
        buffSlot--;
        onBuffChangeEvent.Invoke(buffs);
        return true;
    }
}

public class ConeSpell : Spell
{
    private float scale = 1f;
    private SpellCone cone;
    public ConeSpell(SpellScriptableObject spellSpecs) : base(spellSpecs) {}

    public override void Cast(Transform target) {
        if (spellObject == null) {
            spellObject = Object.Instantiate(spellSpecs.spellPrefab, target.position, 
                                             spellSpecs.spellPrefab.transform.rotation);
            cone = spellObject.GetComponent<SpellCone>();
            cone.Initialize(this, target);
        }
        else {
            scale = Mathf.Clamp(scale + spellSpecs.growthRate * Time.deltaTime, 1f, spellSpecs.maxRadius);
            cone.ModWidth(scale);
        }
    }

    public override float GetCost() { return spellSpecs.baseCost * scale + GetBuffCost(); }

    public override void Reset() {
        if (spellObject == null) { return; }
        Object.Destroy(spellObject);
        scale = 1f;
    }

    public override bool AddBuff(Buff buff) {
        if (buffSlot < 1) { return false; }
        Buff clone = Object.Instantiate(buff);
        clone.duration = 120f;
        if (clone.buffType == BuffType.HEAL) { clone.modAmount *= 2; }
        else if (clone.buffType == BuffType.PULL || clone.buffType == BuffType.PUSH) {
            clone.modAmount /= 50f;
        }
        buffs.Add(clone);
        buffSlot--;
        onBuffChangeEvent.Invoke(buffs);
        return true;
    }
}