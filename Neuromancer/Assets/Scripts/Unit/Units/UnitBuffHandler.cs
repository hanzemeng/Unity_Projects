using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EmeraldAI;
using EmeraldAI.Example;

public class UnitBuffHandler
{
    protected List<(Buff, Coroutine)> healthCoroutine = new();
    protected List<(Buff, Coroutine)> pushCoroutine = new();
    protected List<(Buff, Coroutine)> pullCoroutine = new();

    public bool AddBuffHandler(Buff buff, Transform target, Transform center) {
        switch (buff.buffType) {
            case BuffType.SPEED:
                return AddSpeedBuff(buff, target);
            case BuffType.HEAL:
                return AddHealBuff(buff, target);
            case BuffType.ATTACK:
                return AddAttackBuff(buff, target);
            case BuffType.DEFENSE:
                return AddDefenseBuff(buff, target);
            case BuffType.PULL:
                return AddPullBuff(buff, target, center);
            case BuffType.PUSH:
                return AddPushBuff(buff, target, center);
            default:
                return false;
        }
    }

    public void RemoveBuffHandler(Buff buff, Transform target) {
        switch (buff.buffType) {
            case BuffType.SPEED:
                RemoveSpeedBuff(buff, target);
                break;
            case BuffType.HEAL:
                RemoveHealBuff(buff, target);
                break;
            case BuffType.ATTACK:
                RemoveAttackBuff(buff, target);
                break;
            case BuffType.DEFENSE:
                RemoveDefenseBuff(buff, target);
                break;
            case BuffType.PUSH:
                RemovePushBuff(buff, target);
                break;
            case BuffType.PULL:
                RemovePullBuff(buff, target);
                break;
            default:
                break;
        }
    }

    protected virtual bool AddSpeedBuff(Buff buff, Transform target) { return false; }
    protected virtual void RemoveSpeedBuff(Buff buff, Transform target) {
        if (buff.buffType != BuffType.SPEED) { return; }
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return; }
        if (system.WalkSpeed > system.BaseSpeed) {
            float mod(float s) => s /= 2f - buff.modAmount;
            system.ModSpeedStats(mod);
        }
        else if (system.WalkSpeed < system.BaseSpeed) {
            float mod(float s) => s /= buff.modAmount;
            system.ModSpeedStats(mod);
        }
    }

    protected virtual bool AddHealBuff(Buff buff, Transform target) { return false; }
    protected virtual void RemoveHealBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.HEAL) || Neuromancer.Unit.IsEnemy(target)) { return; }
        for (int i = 0; i < healthCoroutine.Count; i++) {
            if (healthCoroutine[i].Item1 == buff) {
                if (healthCoroutine[i].Item2 != null)
                    PlayerController.player.StopCoroutine(healthCoroutine[i].Item2);
                healthCoroutine.RemoveAt(i);
                return;
            }
        }
    }

    protected IEnumerator Heal(int amount, float duration, EmeraldAISystem system, float interval = 1f) {
        float timePassed = 0f;
        while (timePassed <= duration) {
            system.Heal(amount);
            timePassed += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    protected virtual bool AddAttackBuff(Buff buff, Transform target) { return false; }
    protected virtual void RemoveAttackBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.ATTACK) || Neuromancer.Unit.IsHero(target)) { return; }
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return; }
        if (system.damageModifier > 1f) { system.damageModifier /= 2f - buff.modAmount; }
        else if (system.damageModifier < 1f) { system.damageModifier /= buff.modAmount; }
    }

    protected virtual bool AddDefenseBuff(Buff buff, Transform target) { return false; }
    protected virtual void RemoveDefenseBuff(Buff buff, Transform target) {
        if (buff.buffType != BuffType.DEFENSE) { return; }
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return; }
        if (system.defenseModifier > 1f) { system.defenseModifier /= 2f - buff.modAmount; }
        else if (system.defenseModifier < 1f) { system.defenseModifier /= buff.modAmount; }
    }

    protected virtual bool AddPullBuff(Buff buff, Transform target, Transform center) { return false; }
    protected virtual void RemovePullBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.PULL) || Neuromancer.Unit.IsFriend(target)) { return; }
        for (int i = 0; i < pullCoroutine.Count; i++) {
            if (pullCoroutine[i].Item1 == buff) {
                if (pullCoroutine[i].Item2 != null)
                    PlayerController.player.StopCoroutine(pullCoroutine[i].Item2);
                pullCoroutine.RemoveAt(i);
                return;
            }
        }
    }

    protected IEnumerator Pull(Transform target, Transform center, NavMeshAgent agent, float rate, float duration) {
        float time = 0f;
        while (time < duration) {
            if (target == null || center == null) { yield break; }
            Vector3 centerHeight = center.position;
            centerHeight.y = target.position.y;
            Vector3 targetPos = Vector3.Lerp(target.position, centerHeight, rate);
            agent.Warp(targetPos);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual bool AddPushBuff(Buff buff, Transform target, Transform center) { return false; }
    protected virtual void RemovePushBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.PUSH) || Neuromancer.Unit.IsFriend(target)) { return; }
        for (int i = 0; i < pushCoroutine.Count; i++) {
            if (pushCoroutine[i].Item1 == buff) {
                if (pushCoroutine[i].Item2 != null)
                    PlayerController.player.StopCoroutine(pushCoroutine[i].Item2);
                pushCoroutine.RemoveAt(i);
                return;
            }
        }
    }

    protected IEnumerator Push(Transform target, Transform center, NavMeshAgent agent, float rate, float duration) {
        float time = 0f;
        while (time < duration) {
            if (target == null || center == null) { yield break; }
            Vector3 centerHeight = center.position;
            centerHeight.y = target.position.y;
            Vector3 dir = (target.position - centerHeight).normalized;
            if (dir == Vector3.zero) { dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized; }
            Vector3 end = centerHeight + dir * 15f;
            Vector3 targetPos = Vector3.Lerp(target.position, end, rate);
            agent.Warp(targetPos);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator OneTimeMove(Transform target, Vector3 pos, float dist, NavMeshAgent agent) {
        if (target == null) { yield break; }
        Vector3 start = target.position;
        while (Vector3.Distance(start, target.position) < dist) {
            if (target == null) { yield break; }
            Vector3 targetPos = Vector3.Lerp(target.position, pos, 0.01f);
            agent.Warp(targetPos);
            yield return new WaitForEndOfFrame();
        }
    }
}

public class EnemyBuffHandler : UnitBuffHandler
{
    protected override bool AddSpeedBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.SPEED) || !Neuromancer.Unit.IsEnemy(target)) { return false; }
        
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return false; }
        float mod(float s) => s *= buff.modAmount;
        system.ModSpeedStats(mod);
        return true;
    }

    protected override bool AddAttackBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.ATTACK) || !Neuromancer.Unit.IsEnemy(target)) { return false; }
        
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return false; }
        system.damageModifier *= buff.modAmount;
        return true;
    }

    protected override bool AddDefenseBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.DEFENSE) || !Neuromancer.Unit.IsEnemy(target)) { return false; }
        
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return false; }
        system.defenseModifier *= 2f - buff.modAmount;
        return true;
    }

    protected override bool AddPullBuff(Buff buff, Transform target, Transform center) {
        if ((buff.buffType != BuffType.PULL) || !Neuromancer.Unit.IsEnemy(target)) { return false; }

        Vector3 centerHeight = center.position;
        centerHeight.y = target.position.y;
        Vector3 dir = (centerHeight - target.position).normalized;
        NavMeshAgent agent = target.GetComponent<NavMeshAgent>();
        Vector3 targetPos = target.position + dir * buff.modAmount * 10f;
        if (buff.modType == ModifierType.ADDITIVE) {
            PlayerController.player.StartCoroutine(OneTimeMove(target, targetPos, buff.modAmount, agent));
        }
        else {
            pullCoroutine.Add((buff, PlayerController.player.StartCoroutine(Pull(target, center, agent, buff.modAmount, buff.duration))));
        }
        return true;
    }

    protected override bool AddPushBuff(Buff buff, Transform target, Transform center) {
        if ((buff.buffType != BuffType.PUSH) || !Neuromancer.Unit.IsEnemy(target)) { return false; }

        Vector3 centerHeight = center.position;
        centerHeight.y = target.position.y;
        Vector3 dir = (target.position - centerHeight).normalized;
        if (dir == Vector3.zero) { dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized; }
        NavMeshAgent agent = target.GetComponent<NavMeshAgent>();
        Vector3 targetPos = target.position + dir * buff.modAmount * 10f;
        if (buff.modType == ModifierType.ADDITIVE)
            PlayerController.player.StartCoroutine(OneTimeMove(target, targetPos, buff.modAmount, agent));
        else
            pushCoroutine.Add((buff, PlayerController.player.StartCoroutine(Push(target, center, agent, buff.modAmount, buff.duration))));
        return true;
    }
}

public class AllyBuffHandler : UnitBuffHandler
{
    protected override bool AddSpeedBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.SPEED) || !Neuromancer.Unit.IsAlly(target)) { return false; }
        
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return false; }
        float mod(float s) => s *= 2f - buff.modAmount;
        system.ModSpeedStats(mod);
        return true;
    }

    protected override bool AddHealBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.HEAL) || !Neuromancer.Unit.IsAlly(target)) { return false; }
        
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return false; }
        if (buff.modType == ModifierType.ADDITIVE) {
            healthCoroutine.Add((buff, PlayerController.player.StartCoroutine(Heal((int)buff.modAmount, buff.duration, system))));
        }
        else {
            healthCoroutine.Add((buff, PlayerController.player.StartCoroutine(Heal(1, buff.duration, system, 1f / buff.modAmount))));
        }
        return true;
    }

    protected override bool AddAttackBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.ATTACK) || !Neuromancer.Unit.IsAlly(target)) { return false; }
        
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return false; }
        system.damageModifier *= 2f - buff.modAmount;
        return true;
    }

    protected override bool AddDefenseBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.DEFENSE) || !Neuromancer.Unit.IsAlly(target)) { return false; }
        
        EmeraldAISystem system = target.GetComponent<EmeraldAISystem>();
        if (!system) { return false; }
        system.defenseModifier *= buff.modAmount;
        return true;
    }
}

public class HeroBuffHandler : UnitBuffHandler
{
    protected override bool AddSpeedBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.SPEED) || !Neuromancer.Unit.IsHero(target)) { return false; }
        PlayerController.player.SetSpeed(PlayerController.player.GetSpeed() * (2f - buff.modAmount));
        return true;
    }

    protected override void RemoveSpeedBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.SPEED) || !Neuromancer.Unit.IsHero(target)) { return; }
        
        if (PlayerController.player.GetSpeed() > PlayerController.player.GetBaseSpeed()) {
            PlayerController.player.SetSpeed(PlayerController.player.GetSpeed() / (2f - buff.modAmount));
        }
        else if (PlayerController.player.GetSpeed() < PlayerController.player.GetBaseSpeed()) {
            PlayerController.player.SetSpeed(PlayerController.player.GetSpeed() / buff.modAmount);
        }
    }

    protected override bool AddHealBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.HEAL) || !Neuromancer.Unit.IsHero(target)) { return false; }
        
        if (buff.modType == ModifierType.ADDITIVE) {
            EmeraldAIPlayerHealth healthManager = target.GetComponent<EmeraldAIPlayerHealth>();
            healthCoroutine.Add((buff, PlayerController.player.StartCoroutine(HeroHeal((int)buff.modAmount, buff.duration, healthManager))));
        }
        else {
            EmeraldAIPlayerHealth healthManager = target.GetComponent<EmeraldAIPlayerHealth>();
            healthCoroutine.Add((buff, PlayerController.player.StartCoroutine(HeroHeal(1, buff.duration, healthManager, 1f / buff.modAmount))));
        }
        return true;
    }

    private IEnumerator HeroHeal(int amount, float duration, EmeraldAIPlayerHealth healthManager, float interval = 1f) {
        float timePassed = 0f;
        while (timePassed <= duration) {
            healthManager.HealPlayer(amount);
            timePassed += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    protected override bool AddAttackBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.ATTACK) || !Neuromancer.Unit.IsHero(target)) { return false; }

        PlayerController.player.spellDamageModifier *= 2f - buff.modAmount;
        return true;
    }

    protected override void RemoveAttackBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.ATTACK) || !Neuromancer.Unit.IsHero(target)) { return; }

        if (PlayerController.player.spellDamageModifier > 1f) { PlayerController.player.spellDamageModifier /= 2f - buff.modAmount; }
        else if (PlayerController.player.spellDamageModifier < 1f) { PlayerController.player.spellDamageModifier /= buff.modAmount; }
    }

    protected override bool AddDefenseBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.DEFENSE) || !Neuromancer.Unit.IsHero(target)) { return false; }
        
        EmeraldAIPlayerHealth healthManager = target.GetComponent<EmeraldAIPlayerHealth>();
        healthManager.defenseModifier *= buff.modAmount;
        return true;
    }

    protected override void RemoveDefenseBuff(Buff buff, Transform target) {
        if ((buff.buffType != BuffType.DEFENSE) || !Neuromancer.Unit.IsHero(target)) { return; }
        
        EmeraldAIPlayerHealth healthManager = target.GetComponent<EmeraldAIPlayerHealth>();
        if (healthManager.defenseModifier > 1f) { healthManager.defenseModifier /= 2f - buff.modAmount; }
        else if (healthManager.defenseModifier < 1f) { healthManager.defenseModifier /= buff.modAmount; }
    }
}

// Override add/remove specific buff methods for special behavior
// public class DragonBuffHandler : UnitBuffHandler
// {
//     
// }
