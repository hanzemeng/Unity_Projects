using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatus : MonoBehaviour
{
    private Neuromancer.NPCUnit npcUnit;
    private UnitBuffHandler buffHandler;
    private List<(Buff, GameObject)> buffs = new();

    private bool isStunned = false;

    private float stunDuration = 1f;

    private void Awake() {
        npcUnit = GetComponent<Neuromancer.NPCUnit>();
        npcUnit.StunUnit += Stun;

        // Check npcUnit type and new respective inherited class for special behavior
        // switch (npcUnit.npcUnitType) {
        //     case Neuromancer.NPCUnitType.DRAGON:
        //         buffHandler = new DragonBuffHandler();
        //         break;
        //     ...
        //     default:
        //         buffHandler = new EnemyBuffHandler();
        //         break;
        // }
        buffHandler = new EnemyBuffHandler();
    }

    private void OnEnable() {
        npcUnit.onConvertToAlly.AddListener(OnConvertedToAlly);
    }

    private void OnDisable() {
        npcUnit.onConvertToAlly.RemoveListener(OnConvertedToAlly);
    }

    public void Stun(float duration) 
    {
        stunDuration = duration;
        if (!isStunned)
        {
            Debug.Log("Stunned " + gameObject.name);
            isStunned = true;
            //npcUnit.StartStunnedAnimation?.Invoke();
            //npcUnit.OnIsBusy?.Invoke(true);
           // npcUnit.OnIsStunned?.Invoke(true);
        }
    }

    public void OnStunBegin()
    {
        StartCoroutine(EndStun());
    }

    public void OnStunEnd()
    {
        // Uses timer based stun end instead of animation end
    }

    private IEnumerator EndStun()
    {
        yield return new WaitForSeconds(stunDuration);
        Debug.Log("UnStunned " + gameObject.name);
        //npcUnit.OnIsBusy?.Invoke(false);
        //npcUnit.OnIsStunned?.Invoke(false);
        isStunned = false;
    }

    public void AddBuff(Buff buff, Transform center) {
        foreach ((Buff, GameObject) b in buffs) {
            if (b.Item1 == buff) { return; }
        }
        bool success = buffHandler.AddBuffHandler(buff, transform, center);
        if (!success) { return; }
        GameObject buffParticle = null;
        if (!Neuromancer.Unit.IsEnemy(transform)) {
            buffParticle = Instantiate(buff.activeParticle, transform.position, buff.activeParticle.transform.rotation);
            buffParticle.transform.SetParent(transform);
            StartCoroutine(DestroyParticle(buffParticle, buff.duration));
        }
        buffs.Add((buff, buffParticle));
        StartCoroutine(Timer(buff));
    }

    public void RemoveBuff(Buff buff, float delay = 0f) {
        if (delay > 0f) {
            if (buff.buffType == BuffType.SPEED || 
                buff.buffType == BuffType.ATTACK ||
                buff.buffType == BuffType.DEFENSE) {
                StartCoroutine(RemoveBuffDelay(buff, delay));
                return;
            }
        }
        foreach ((Buff, GameObject) b in buffs) {
            if (b.Item1 == buff) {
                buffHandler.RemoveBuffHandler(buff, transform);
                if (buff.duration > 50f && b.Item2 != null) {
                    Destroy(b.Item2);
                }
                buffs.Remove(b);
                return;
            }
        }
    }

    private IEnumerator RemoveBuffDelay(Buff buff, float delay) {
        yield return new WaitForSeconds(delay);
        RemoveBuff(buff);
    }

    public void ClearBuff() {
        foreach ((Buff, GameObject) b in buffs) {
            buffHandler.RemoveBuffHandler(b.Item1, transform);
            if (b.Item2 != null) { Destroy(b.Item2); }
        }
        buffs.Clear();
        StopAllCoroutines();
    }

    private IEnumerator Timer(Buff buff) {
        yield return new WaitForSeconds(buff.duration);
        RemoveBuff(buff);
    }

    private void OnConvertedToAlly() {
        ClearBuff();
        buffHandler = new AllyBuffHandler();
    }

    private IEnumerator DestroyParticle(GameObject particle, float duration) {
        if (duration <= 0f) { duration = 1f; }
        yield return new WaitForSeconds(duration);
        if (particle != null) { Destroy(particle); }
    }
}
