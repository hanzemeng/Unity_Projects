using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private HeroBuffHandler buffHandler;
    private List<(Buff, GameObject)> buffs = new();
    
    private void Awake() {
        buffHandler = new HeroBuffHandler();
    }

    public void AddBuff(Buff buff, Transform center) {
        foreach ((Buff, GameObject) b in buffs) {
            if (b.Item1 == buff) { return; }
        }
        bool success = buffHandler.AddBuffHandler(buff, transform, center);
        if (!success) { return; }
        GameObject buffParticle = Instantiate(buff.activeParticle, transform.position, buff.activeParticle.transform.rotation);
        buffParticle.transform.SetParent(transform);
        StartCoroutine(DestroyParticle(buffParticle, buff.duration));
        buffs.Add((buff, buffParticle));
        StartCoroutine(Timer(buff));
    }

    public void RemoveBuff(Buff buff) {
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

    private IEnumerator DestroyParticle(GameObject particle, float duration) {
        if (duration <= 0f) { duration = 1f; }
        yield return new WaitForSeconds(duration);
        if (particle != null) { Destroy(particle); }
    }
}
