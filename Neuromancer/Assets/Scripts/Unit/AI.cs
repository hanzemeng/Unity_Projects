using System.Collections;
using UnityEngine;
using System.Linq;

namespace Neuromancer
{
    [RequireComponent(typeof(NPCUnit))]
    public class AI : MonoBehaviour
    {
        public NPCUnit npcUnit { get; private set; }
        private Command currentCommand;

        [Tooltip("Leave empty if it does not have a custom ability")]
        [SerializeField] private CustomAbility interactAbility;
        [Tooltip("Leave empty if it does not have a custom ability")]
        [SerializeField] private CustomAbility customAbilityOne;
        [Tooltip("Leave empty if it does not have a custom ability")]
        [SerializeField] private CustomAbility customAbilityTwo;
        [Tooltip("Leave empty if it does not have a custom ability")]
        [SerializeField] private CustomAbility customAbilityThree;

        private void Start()
        {
            npcUnit = GetComponent<NPCUnit>();
            npcUnit.OnCommandIssued += HandleCommandIssued;
            SetCommand(npcUnit.GetCurrentCommand()); // Initialize the AI Command (to DEFAULT or whatever is the Current Command)

            if (interactAbility != null)
                CountAbilitiesAnimationIndex(interactAbility);
            if (customAbilityOne != null)
                CountAbilitiesAnimationIndex(customAbilityOne);
            if (customAbilityTwo != null)
                CountAbilitiesAnimationIndex(customAbilityTwo);
            if (customAbilityThree != null)
                CountAbilitiesAnimationIndex(customAbilityThree);

            npcUnit.EmeraldComponent.MeleeAttackIndex = npcUnit.EmeraldComponent.numberMeleeAttackAnimations;
            npcUnit.EmeraldComponent.MeleeRunAttackIndex = npcUnit.EmeraldComponent.numberMeleeRunAttacksAnimations;
            npcUnit.EmeraldComponent.OffensiveAbilityIndex = npcUnit.EmeraldComponent.numberOffensiveAbilitiesAnimations;
            npcUnit.EmeraldComponent.SupportAbilityIndex = npcUnit.EmeraldComponent.numberSupportAbilitiesAnimations;
            npcUnit.EmeraldComponent.SummoningAbilityIndex = npcUnit.EmeraldComponent.numberSummoningAbilitiesAnimations;

        }

        private void CountAbilitiesAnimationIndex(CustomAbility ability)
        {
            if (!ability.excludeFromBasic)
            {
                return;
            }

            switch (ability.abilityType)
            {
                case CustomAbility.AbilityType.MELEE:
                    npcUnit.EmeraldComponent.numberMeleeAttackAnimations++;
                    break;
                case CustomAbility.AbilityType.MELEE_RUN:
                    npcUnit.EmeraldComponent.numberMeleeRunAttacksAnimations++;
                    break;
                case CustomAbility.AbilityType.OFFENSIVE:
                    npcUnit.EmeraldComponent.numberOffensiveAbilitiesAnimations++;
                    break;
                case CustomAbility.AbilityType.SUMMONNING:
                    npcUnit.EmeraldComponent.numberSummoningAbilitiesAnimations++;
                    break;
                case CustomAbility.AbilityType.SUPPORT:
                    npcUnit.EmeraldComponent.numberSupportAbilitiesAnimations++;
                    break;
            }
        }

        private void HandleCommandIssued(int newCommandIndex)
        {
            if (newCommandIndex == 0)
            {
                StopCommand(currentCommand); // Stop old command (reset settings)
                SetCommand(npcUnit.GetCurrentCommand()); // Tell AI to run/initalize new command settings (if needed)
            }
        }

        private void Update()
        {
            RunCommand(currentCommand); // Run the actual command (either Emerald AI handles this or we have to update it using our own logic)
        }

        private void StopCommand(Command command)
        {
            if (command == null)
                return;
            switch (command.commandType)
            {
                case CommandType.HOLD_POSITION:
                case CommandType.IDLE:
                    StopIdleCommand(command);
                    break;
                case CommandType.MOVE_TO:
                    StopMoveToCommand(command);
                    break;
                case CommandType.ATTACK_FOLLOW:
                    StopAttackFollowCommand(command);
                    break;
                case CommandType.ABILITY_ONE:
                    StopAbilityOneCommand(command);
                    break;
                case CommandType.ABILITY_TWO:
                    StopAbilityTwoCommand(command);
                    break;
                case CommandType.ABILITY_THREE:
                    StopAbilityThreeCommand(command);
                    break;
                case CommandType.INTERACT:
                    StopInteractCommand(command);
                    break;
                case CommandType.FOLLOW_TARGET:
                    StopFollowTargetCommand(command);
                    break;
                case CommandType.ATTACK_TARGET:
                    StopAttackTargetCommand(command);
                    break;
                case CommandType.ATTACK_MOVE:
                    StopAttackMoveCommand(command);
                    break;
                case CommandType.DEFAULT:
                default:
                    StopDefaultCommand(command);
                    break;
            }
        }

        private void SetCommand(Command command)
        {

            currentCommand = command;
            if (command == null)
                currentCommand = npcUnit.GetCurrentCommand();
            switch (command.commandType)
            {
                case CommandType.HOLD_POSITION:
                case CommandType.IDLE:
                    SetIdleCommand(command);
                    break;
                case CommandType.MOVE_TO:
                    SetMoveToCommand(command);
                    break;
                case CommandType.ATTACK_FOLLOW:
                    SetAttackFollowCommand(command);
                    break;
                case CommandType.ABILITY_ONE:
                    SetAbilityOneCommand(command);
                    break;
                case CommandType.ABILITY_TWO:
                    SetAbilityTwoCommand(command);
                    break;
                case CommandType.ABILITY_THREE:
                    SetAbilityThreeCommand(command);
                    break;
                case CommandType.INTERACT:
                    SetInteractCommand(command);
                    break;
                case CommandType.FOLLOW_TARGET:
                    SetFollowTargetCommand(command);
                    break;
                case CommandType.ATTACK_TARGET:
                    SetAttackTargetCommand(command);
                    break;
                case CommandType.ATTACK_MOVE:
                    SetAttackMoveCommand(command);
                    break;
                case CommandType.DEFAULT:
                default:
                    SetDefaultCommand(command);
                    break;
            }
        }

        private void RunCommand(Command command)
        {
            if (command == null)
            {
                SetCommand(npcUnit.GetCurrentCommand());
                return;
            }
            switch (command.commandType)
            {
                case CommandType.HOLD_POSITION:
                case CommandType.IDLE:
                    RunIdleCommand(command);
                    break;
                case CommandType.MOVE_TO:
                    RunMoveToCommand(command);
                    break;
                case CommandType.ATTACK_FOLLOW:
                    RunAttackFollowCommand(command);
                    break;
                case CommandType.ABILITY_ONE:
                    RunAbilityOneCommand(command);
                    break;
                case CommandType.ABILITY_TWO:
                    RunAbilityTwoCommand(command);
                    break;
                case CommandType.ABILITY_THREE:
                    RunAbilityThreeCommand(command);
                    break;
                case CommandType.INTERACT:
                    RunInteractCommand(command);
                    break;
                case CommandType.FOLLOW_TARGET:
                    RunFollowTargetCommand(command);
                    break;
                case CommandType.ATTACK_TARGET:
                    RunAttackTargetCommand(command);
                    break;
                case CommandType.ATTACK_MOVE:
                    RunAttackMoveCommand(command);
                    break;
                case CommandType.DEFAULT:
                default:
                    RunDefaultCommand(command);
                    break;
            }
        }

        // ---------- Start of DEFAULT ----------

        private void StopDefaultCommand(Command command)
        {
            // TODO: Adjust Emerald AI Component params in editor to change default behavior
            EmeraldAI.EmeraldAIFactionData FactionData = Resources.Load("Faction Data") as EmeraldAI.EmeraldAIFactionData;
            if (npcUnit.EmeraldComponent.CurrentFaction == FactionData.FactionNameList.IndexOf("Ally"))
            {
                //npcUnit.EmeraldEventsManagerComponent.ChangeBehavior((EmeraldAI.EmeraldAISystem.CurrentBehavior)npcUnit.EmeraldComponent.StartingBehaviorRef);
                npcUnit.EmeraldEventsManagerComponent.ClearTarget(true);
                //npcUnit.EmeraldComponent.CurrentFollowTarget = null;
            }
        }

        private void SetDefaultCommand(Command command)
        {
            // TODO: Adjust Emerald AI Component params in editor to change default behavior
            EmeraldAI.EmeraldAIFactionData FactionData = Resources.Load("Faction Data") as EmeraldAI.EmeraldAIFactionData;
            if (npcUnit.EmeraldComponent.CurrentFaction == FactionData.FactionNameList.IndexOf("Ally"))
            {
                if (npcUnit.EmeraldComponent.BehaviorRef != EmeraldAI.EmeraldAISystem.CurrentBehavior.Companion)
                {
                    npcUnit.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAI.EmeraldAISystem.CurrentBehavior.Companion);
                    npcUnit.EmeraldEventsManagerComponent.SetFollowerTarget(PlayerController.player.transform);
                    npcUnit.IssueCommand(new Command(CommandType.ATTACK_FOLLOW));
                }
            }
        }

        private void RunDefaultCommand(Command command)
        {
            SetDefaultCommand(command);
            // TODO: Adjust Emerald AI Component params in editor to change default behavior
        }

        // ---------- End of DEFAULT ----------

        // ---------- Start of IDLE ----------

        /*
         * IDLE is an equivalent command to HOLD_POSITION where the unit will still attack if in range.
         * HOLD_POSITION actually triggers these functions. There is no separate function for HOLD_POSITION.
         * 
         * There is no point for stopping attacks except in the rare case where the player wants to prevent drawing aggros
         * since damaging another unit draws their aggros.
         * If different behavior is needed for IDLE or HOLD_POSITION, please submit a ticket here: https://github.com/hanzemeng/Neuromancer/issues
         */

        private void StopIdleCommand(Command command)
        {
            npcUnit.EmeraldEventsManagerComponent.ResumeMovement();
        }

        private void SetIdleCommand(Command command)
        {
            npcUnit.EmeraldEventsManagerComponent.StopMovement();
        }

        private void RunIdleCommand(Command command)
        {

        }

        // ---------- End of IDLE ----------

        // ---------- Start of ATTACK_FOLLOW ----------

        private void StopAttackFollowCommand(Command command)
        {

            EmeraldAI.EmeraldAIFactionData FactionData = Resources.Load("Faction Data") as EmeraldAI.EmeraldAIFactionData;

            if (npcUnit.EmeraldComponent.CurrentFaction == FactionData.FactionNameList.IndexOf("Ally"))
            {
                //npcUnit.EmeraldEventsManagerComponent.ChangeBehavior((EmeraldAI.EmeraldAISystem.CurrentBehavior)npcUnit.EmeraldComponent.StartingBehaviorRef);
                npcUnit.EmeraldEventsManagerComponent.ClearTarget(true);
                //npcUnit.EmeraldComponent.CurrentFollowTarget = null;
            }
        }

        private void SetAttackFollowCommand(Command command)
        {
            EmeraldAI.EmeraldAIFactionData FactionData = Resources.Load("Faction Data") as EmeraldAI.EmeraldAIFactionData;

            if (npcUnit.EmeraldComponent.CurrentFaction == FactionData.FactionNameList.IndexOf("Ally"))
            {
                npcUnit.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAI.EmeraldAISystem.CurrentBehavior.Companion);
                npcUnit.EmeraldEventsManagerComponent.SetFollowerTarget(command.targetUnit == null ? PlayerController.player.transform : command.targetUnit);
            }
        }

        private void RunAttackFollowCommand(Command command)
        {
            // Emerald AI does the work here
        }

        // ---------- End of ATTACK_FOLLOW ----------



        // ---------- Start of MOVE_TO ----------

        private float originalStoppingDistance;
        private EmeraldAI.EmeraldAISystem.WanderType originalWanderType;

        private void StopMoveToCommand(Command command)
        {
            if (command.commandType == CommandType.MOVE_TO)
            {
                npcUnit.EmeraldComponent.WanderTypeRef = originalWanderType;
                npcUnit.EmeraldEventsManagerComponent.ChangeBehavior((EmeraldAI.EmeraldAISystem.CurrentBehavior)npcUnit.EmeraldComponent.StartingBehaviorRef);
                npcUnit.EmeraldComponent.m_NavMeshAgent.stoppingDistance = originalStoppingDistance;
                npcUnit.EmeraldComponent.CurrentMovementState = npcUnit.EmeraldComponent.StartingMovementState;
                //npcUnit.EmeraldComponent.ReachedDestinationEvent.RemoveListener(FinishMoveToCommand);
                //npcUnit.EmeraldEventsManagerComponent.ClearTarget(true);
            }
            
        }

        private void SetMoveToCommand(Command command)
        {
            npcUnit.EmeraldComponent.WanderTypeRef = EmeraldAI.EmeraldAISystem.WanderType.Destination;
            npcUnit.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAI.EmeraldAISystem.CurrentBehavior.Passive);
            //npcUnit.EmeraldComponent.ReachedDestinationEvent.AddListener(FinishMoveToCommand);
            originalStoppingDistance = npcUnit.EmeraldComponent.m_NavMeshAgent.stoppingDistance;
            npcUnit.EmeraldComponent.m_NavMeshAgent.stoppingDistance = 1f;
            if (npcUnit.EmeraldComponent.m_NavMeshAgent.isActiveAndEnabled)
                npcUnit.EmeraldEventsManagerComponent.SetDestinationPosition(command.targetPosition);
            originalWanderType = npcUnit.EmeraldComponent.WanderTypeRef;
            npcUnit.EmeraldComponent.CurrentMovementState = EmeraldAI.EmeraldAISystem.MovementState.Run;
        }

        private void RunMoveToCommand(Command command)
        {
            if (Vector3.Distance(transform.position, command.targetPosition) < npcUnit.EmeraldComponent.m_NavMeshAgent.stoppingDistance + 1f)
            {
                FinishMoveToCommand();
            }
        }

        private void FinishMoveToCommand()
        {
            if (currentCommand.commandType == CommandType.MOVE_TO)
            {
                //npcUnit.EmeraldComponent.ReachedDestinationEvent.RemoveListener(FinishMoveToCommand);
                npcUnit.FinishCurrentCommand(currentCommand);
                StopMoveToCommand(currentCommand);
            }
        }

        // ---------- End of MOVE_TO ----------


        // ---------- Start of ABILITY_ONE ----------

        private void StopAbilityOneCommand(Command command)
        {
            if (customAbilityOne != null)
            {
                customAbilityOne.StopAbility(this);
            }
        }

        private void SetAbilityOneCommand(Command command)
        {
            if (customAbilityOne != null)
            {
                customAbilityOne.SetAbility(this, command);
            }
            else
            {
                npcUnit.FinishCurrentCommand(currentCommand);
                StopAbilityOneCommand(currentCommand);
            }

        }

        private void RunAbilityOneCommand(Command command)
        {
            if (customAbilityOne != null)
            {
                customAbilityOne.RunAbility(this);
            }
        }

        private void TriggerAbilityOneCommand()
        {
            if (customAbilityOne != null)
            {
                customAbilityOne.TriggerAbility(this, currentCommand);
            }
        }

        // ---------- End of ABILITY_ONE ----------

        // ---------- Start of ABILITY_TWO ----------

        private void StopAbilityTwoCommand(Command command)
        {
            if (customAbilityTwo != null)
            {
                customAbilityTwo.StopAbility(this);
            }
        }

        private void SetAbilityTwoCommand(Command command)
        {
            if (customAbilityTwo != null)
            {
                customAbilityTwo.SetAbility(this, command);
            }
            else
            {
                npcUnit.FinishCurrentCommand(currentCommand);
                StopAbilityTwoCommand(currentCommand);
            }

        }

        private void RunAbilityTwoCommand(Command command)
        {
            if (customAbilityTwo != null)
            {
                customAbilityTwo.RunAbility(this);
            }
        }

        private void TriggerAbilityTwoCommand()
        {
            if (customAbilityTwo != null)
            {
                customAbilityTwo.TriggerAbility(this, currentCommand);
            }
        }

        // ---------- End of ABILITY_TWO ----------

        // ---------- Start of ABILITY_THREE ----------

        private void StopAbilityThreeCommand(Command command)
        {
            if (customAbilityThree != null)
            {
                customAbilityThree.StopAbility(this);
            }
        }

        private void SetAbilityThreeCommand(Command command)
        {
            if (customAbilityThree != null)
            {
                customAbilityThree.SetAbility(this, command);
            }
            else
            {
                npcUnit.FinishCurrentCommand(currentCommand);
                StopAbilityThreeCommand(currentCommand);
            }

        }

        private void RunAbilityThreeCommand(Command command)
        {
            if (customAbilityThree != null)
            {
                customAbilityThree.RunAbility(this);
            }
        }

        private void TriggerAbilityThreeCommand()
        {
            if (customAbilityThree != null)
            {
                customAbilityThree.TriggerAbility(this, currentCommand);
            }
        }

        // ---------- End of ABILITY_THREE ----------

        // ---------- Start of INTERACT ----------

        private Interactable interactable;

        private void StopInteractCommand(Command command)
        {
            if (interactAbility != null)
                interactAbility.StopAbility(this);
            interactable = null;

        }

        private void SetInteractCommand(Command command)
        {
            interactable = command.targetUnit.GetComponent<Interactable>(); // Unit acquires Interactable component here
            npcUnit.EmeraldEventsManagerComponent.ChangeBehavior((EmeraldAI.EmeraldAISystem.CurrentBehavior)npcUnit.EmeraldComponent.StartingBehaviorRef);
            if (interactable != null && interactAbility != null)    // checks if the interactable exists and the interactAbility exists
                interactAbility.SetAbility(this, command);
            else
            {
                npcUnit.FinishCurrentCommand(currentCommand);
                StopInteractCommand(currentCommand);
                return;
            }

        }

        private void RunInteractCommand(Command command)
        {
            if (interactAbility != null)
                interactAbility.RunAbility(this);
        }

        public void TriggerInteraction()
        {
            if (interactAbility != null && interactable != null && (interactable.possibleEnemies.Contains(NPCUnitType.DEFAULT) || interactable.possibleEnemies.Contains(npcUnit.unitPrefab.npcUnitType)))
            {
                if(interactable.repeatInteraction)
                {
                    Command interactCommand = currentCommand;
                    interactable.Interact?.Invoke(this.gameObject);
                    interactAbility.TriggerAbility(this, currentCommand);
                    npcUnit.IssueCommand(interactCommand);
                    return;
                }
                
                StartCoroutine(DelayInteract());
            }
            else if (currentCommand.commandType == CommandType.INTERACT)
            {
                npcUnit.FinishCurrentCommand(currentCommand);
                StopInteractCommand(currentCommand);
                interactable = null;
                return;
            }
        }

        private IEnumerator DelayInteract() {
            yield return new WaitForSeconds(Random.Range(0f, 0.1f));
            interactable.Interact?.Invoke(this.gameObject);
            interactAbility.TriggerAbility(this, currentCommand);
            interactable = null;
        }

        // ---------- End of INTERACT ----------


        // ---------- Start of FOLLOW_TARGET ----------

        private EmeraldAI.EmeraldAISystem.CombatType followTargetOriginalCombatType;

        private void StopFollowTargetCommand(Command command)
        {
            //npcUnit.EmeraldEventsManagerComponent.ChangeBehavior((EmeraldAI.EmeraldAISystem.CurrentBehavior)npcUnit.EmeraldComponent.StartingBehaviorRef);
            npcUnit.EmeraldComponent.CombatTypeRef = followTargetOriginalCombatType;
            npcUnit.EmeraldEventsManagerComponent.ClearTarget(true);

        }

        private void SetFollowTargetCommand(Command command)
        {
            if (npcUnit.EmeraldComponent.BehaviorRef != EmeraldAI.EmeraldAISystem.CurrentBehavior.Companion)
            {
                npcUnit.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAI.EmeraldAISystem.CurrentBehavior.Companion);
                followTargetOriginalCombatType = npcUnit.EmeraldComponent.CombatTypeRef;
                npcUnit.EmeraldComponent.CombatTypeRef = EmeraldAI.EmeraldAISystem.CombatType.Defensive;
                npcUnit.EmeraldEventsManagerComponent.SetFollowerTarget(command.targetUnit);
            }
        }

        private void RunFollowTargetCommand(Command command)
        {

        }

        // ---------- End of FOLLOW_TARGET ----------


        // ---------- Start of ATTACK_TARGET ----------

        private void StopAttackTargetCommand(Command command)
        {
            npcUnit.EmeraldEventsManagerComponent.ClearTarget();
        }

        private void SetAttackTargetCommand(Command command)
        {
            npcUnit.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAI.EmeraldAISystem.CurrentBehavior.Aggressive);
            npcUnit.EmeraldEventsManagerComponent.OverrideCombatTarget(command.targetUnit);
            npcUnit.EmeraldComponent.CurrentMovementState = EmeraldAI.EmeraldAISystem.MovementState.Run;
        }

        private void RunAttackTargetCommand(Command command)
        {
            if (command.commandType == CommandType.ATTACK_TARGET && (command.targetUnit == null || (npcUnit.EmeraldComponent.TargetEmerald && npcUnit.EmeraldComponent.TargetEmerald.IsDead)))
            {
                
                npcUnit.FinishCurrentCommand(currentCommand);
                
            }
        }

        // ---------- End of ATTACK_TARGET ----------


        // ---------- Start of ATTACK_TARGET ----------

        private void StopAttackMoveCommand(Command command)
        {
            //npcUnit.EmeraldComponent.CurrentMovementState = npcUnit.EmeraldComponent.StartingMovementState;
            npcUnit.EmeraldEventsManagerComponent.ClearTarget();
        }

        private void SetAttackMoveCommand(Command command)
        {
            npcUnit.EmeraldComponent.WanderTypeRef = EmeraldAI.EmeraldAISystem.WanderType.Destination;
            npcUnit.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAI.EmeraldAISystem.CurrentBehavior.Aggressive);
            if (npcUnit.EmeraldComponent.m_NavMeshAgent.isActiveAndEnabled)
                npcUnit.EmeraldEventsManagerComponent.SetDestinationPosition(command.targetPosition);
            npcUnit.EmeraldComponent.CurrentMovementState = EmeraldAI.EmeraldAISystem.MovementState.Run;
        }

        private void RunAttackMoveCommand(Command command)
        {

        }

        // ---------- End of ATTACK_TARGET ----------
    }
}