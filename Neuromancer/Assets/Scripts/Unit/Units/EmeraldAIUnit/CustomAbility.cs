using UnityEngine;

[CreateAssetMenu(menuName =  "CustomAbility")]
public class CustomAbility : ScriptableObject
{
    public enum AbilityType
    {
        MELEE,
        MELEE_RUN,
        OFFENSIVE,
        SUPPORT,
        SUMMONNING,
        CUSTOM,
    }

    public AbilityType abilityType;
    public int animationIndex;
    public bool singleShot = true;
    public bool excludeFromBasic = false;

    public float cooldown;

    private EmeraldAI.EmeraldAISystem.YesOrNo originalUseAggro;
    private EmeraldAI.EmeraldAISystem.MeleeAttackPickTypeEnum originalMeleeAttackPickType;
    private EmeraldAI.EmeraldAISystem.OffensiveAbilityPickTypeEnum originalOffensiveAttackPickType;

    public void StopAbility(Neuromancer.AI ai)
    {
        ai.npcUnit.EmeraldComponent.AbilityAttackIndex = -1;
        ai.npcUnit.EmeraldComponent.isMeleeAbility = false;
        ai.npcUnit.EmeraldComponent.isSummoningAbility = false;
        ai.npcUnit.EmeraldComponent.isSupportAbility = false;
        ai.npcUnit.EmeraldComponent.isOffensiveAbility = false;
        ai.npcUnit.EmeraldComponent.isUsingCustomAbility = false;

        ai.npcUnit.EmeraldComponent.MeleeAttackPickType = originalMeleeAttackPickType;
        ai.npcUnit.EmeraldComponent.OffensiveAbilityPickType = originalOffensiveAttackPickType;
        ai.npcUnit.EmeraldComponent.UseAggro = originalUseAggro;
        //ai.npcUnit.EmeraldEventsManagerComponent.SetDestination(ai.transform);
        ai.npcUnit.EmeraldEventsManagerComponent.ClearTarget();
    }


    public void SetAbility(Neuromancer.AI ai, Command command)
    {

        if (command.commandType == CommandType.ABILITY_ONE)
        {
            
            if (ai.npcUnit.EmeraldComponent.abilityOneOnCooldown)
            {
                Debug.Log("Ability On Cooldown");
                TriggerAbility(ai, command);
                return;
            }
            
        }
        
        ai.npcUnit.EmeraldEventsManagerComponent.ClearTarget();
        
        if (command.targetData != null)
            ai.npcUnit.EmeraldEventsManagerComponent.OverrideCombatTarget(command.targetData.transform);
        else if (command.targetUnit != null)
            ai.npcUnit.EmeraldEventsManagerComponent.OverrideCombatTarget(command.targetUnit);
        else
        {
            TriggerAbility(ai, command);
            return;
        }
        
        if (command.targetData != null && !command.targetData.CompareTag("AI") && abilityType == AbilityType.MELEE)
        {
            TriggerAbility(ai, command);
            return;
        }
        
        ai.npcUnit.EmeraldComponent.AbilityAttackIndex = animationIndex;
        originalUseAggro = ai.npcUnit.EmeraldComponent.UseAggro;
        ai.npcUnit.EmeraldComponent.UseAggro = EmeraldAI.EmeraldAISystem.YesOrNo.No;
        ai.npcUnit.EmeraldComponent.isUsingCustomAbility = true;
        
        originalOffensiveAttackPickType = ai.npcUnit.EmeraldComponent.OffensiveAbilityPickType;
        originalMeleeAttackPickType = ai.npcUnit.EmeraldComponent.MeleeAttackPickType;
        
        switch (abilityType)
        {
            // TODO: add the other ability types
            case AbilityType.OFFENSIVE: 
                ai.npcUnit.EmeraldComponent.isOffensiveAbility = true;
                if (ai.npcUnit.EmeraldComponent.EnableBothWeaponTypes == EmeraldAI.EmeraldAISystem.YesOrNo.Yes &&
                    ai.npcUnit.EmeraldComponent.WeaponTypeRef != EmeraldAI.EmeraldAISystem.WeaponType.Ranged)
                    ai.npcUnit.EmeraldComponent.StartCoroutine(ai.npcUnit.EmeraldComponent.InstantSwitchCurrentWeaponType("Ranged"));
                ai.npcUnit.EmeraldComponent.OffensiveAbilityPickType = EmeraldAI.EmeraldAISystem.OffensiveAbilityPickTypeEnum.Ability;
                break;
            case AbilityType.MELEE:
            default:
                ai.npcUnit.EmeraldComponent.isMeleeAbility = true;
                if (ai.npcUnit.EmeraldComponent.EnableBothWeaponTypes == EmeraldAI.EmeraldAISystem.YesOrNo.Yes &&
                    ai.npcUnit.EmeraldComponent.WeaponTypeRef != EmeraldAI.EmeraldAISystem.WeaponType.Melee)
                    ai.npcUnit.EmeraldComponent.StartCoroutine(ai.npcUnit.EmeraldComponent.InstantSwitchCurrentWeaponType("Melee"));
                ai.npcUnit.EmeraldComponent.MeleeAttackPickType = EmeraldAI.EmeraldAISystem.MeleeAttackPickTypeEnum.Ability;
                break;
        }
    }


    public void RunAbility(Neuromancer.AI ai)
    {

    }

    public void TriggerAbility(Neuromancer.AI ai, Command command)
    {
        if (singleShot)
        {
            ai.npcUnit.FinishCurrentCommand(command);
            StopAbility(ai);
        }
    }
}
