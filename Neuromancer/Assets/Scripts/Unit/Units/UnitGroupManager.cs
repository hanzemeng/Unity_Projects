using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Neuromancer;

public class UnitGroupManager : MonoBehaviour
{
    public static UnitGroupManager current;
    public bool ignoreAllyCollisions = false;

    [SerializeField] public int maxAllyCount = 4;
    [SerializeField] private GameObject emptyTargetObject;

    // Usage: Changing the maxAllies cap: "UnitGroupManager.current.maxAllies = X," "UnitGroupManager.current.maxAllies += 1," etc
    public int maxAllies { // wrapper so that when maxAllies change, then it can properly handle the cap
        get
        {
            return maxAllyCount;
        } 
        set
        {
            if (value < maxAllyCount) // maxAllyCount decreased (shouldn't happen but you never know)
            {
                while (allUnits.units.Count > value)
                {
                    allUnits.units[0].ChangeAllyToEnemy(); // First ally you possessed becomes enemy again.
                }
            }
            maxAllyCount = value;
            OnNewMaxAllyCountEvent.Invoke(maxAllyCount);
        }
    }

    private bool multiselectFlag;

    public Transform playerTransform { get; private set; }

    public ReticleController reticleController { get; private set; } 
    public PartyManager partyManager { get; private set; } 

    public UnitGroup selectedUnitGroup = new UnitGroup();

    public List<NPCUnit> hotkeyUnits = new List<NPCUnit>();

    public UnitGroup allUnits = new UnitGroup();

    private PlayerInputs inputs;

    /* OnHotKeySelectedGroupChanged
     * Params: 
     *      int - (0) when all units are selected and (1 - 9) denoting which hotkey group
     *      List<Neuromancer.NPCUnit> - list of units that are selected
     */
    public UnityEvent<int, List<NPCUnit>> OnHotKeySelectedGroupChanged = new UnityEvent<int, List<NPCUnit>>();
    public UnityEvent<int, NPCUnit> OnHotKeyUnitChange = new UnityEvent<int, NPCUnit>();
    public UnityEvent<int> OnNewMaxAllyCountEvent = new UnityEvent<int>(); // Call with the new max ally count for UI

    private void Awake()
    {
        if (current == null)
        {
            current = this;
            // Init hotkey unit groups
            for (int i = 0; i < 9; i ++)
            {
                hotkeyUnits.Add(null);
            }
        } else
        {
            Destroy(gameObject);
        }
        inputs = PlayerInputManager.playerInputs;
    }

    private void Start()
    {
        if (ignoreAllyCollisions)
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Unit.HERO_LAYER_NAME), LayerMask.NameToLayer(Unit.ALLY_LAYER_NAME));

        playerTransform = PlayerController.player.transform;

        reticleController = ReticleController.current;
        partyManager = PartyManager.current;

        reticleController.onSelectionEvent.AddListener(HandleNewUnitSelection);
        reticleController.onTargetEvent.AddListener(RightClick);
        reticleController.onSelectionByTypeEvent.AddListener(HandleSelectionByType);
        partyManager.onHotkeyDragEvent.AddListener(HandleHotkeySwap);

        inputs.AllyAction.Default.started += IssueDefaultCommand;
        inputs.AllyAction.Idle.started += IssueIdleCommand;
        inputs.AllyAction.AttackFollow.started += IssueAttackFollowCommand;
        inputs.AllyAction.AttackMove.started += IssueAttackMoveCommand;
        inputs.AllyAction.Interact.started += IssueInteractCommand;
        inputs.AllyAction.BuffSelf.started += BuffSelf;
        inputs.AllyAction.AbilityOne.started += AbilityOne;
        inputs.AllyAction.AbilityTwo.started += AbilityTwo;
        inputs.AllyAction.AbilityThree.started += AbilityThree;
        inputs.AllyAction.Disband.started += Disband;
        inputs.AllyAction.DropItem.started += DropItem;

        inputs.AllyAction.SelectAllAllies.started += HandleSelectAllAllies;
        inputs.AllyAction.DeselectAllAllies.started += HandleDeselectAllAllies;
        inputs.AllyAction.Multiselect.started += StartMultiselect;
        inputs.AllyAction.Multiselect.canceled += StopMultiselect;

        // Select Hotkeys
        inputs.AllyAction.Hotkey1.started += HandleHotKeySelect1;
        inputs.AllyAction.Hotkey2.started += HandleHotKeySelect2;
        inputs.AllyAction.Hotkey3.started += HandleHotKeySelect3;
        inputs.AllyAction.Hotkey4.started += HandleHotKeySelect4;
        inputs.AllyAction.Hotkey5.started += HandleHotKeySelect5;
        inputs.AllyAction.Hotkey6.started += HandleHotKeySelect6;
        inputs.AllyAction.Hotkey7.started += HandleHotKeySelect7;
        inputs.AllyAction.Hotkey8.started += HandleHotKeySelect8;
        inputs.AllyAction.Hotkey9.started += HandleHotKeySelect9;

        OnNewMaxAllyCountEvent.Invoke(maxAllyCount);
        multiselectFlag = false;
    }

    private void OnEnable()
    {
        inputs.AllyAction.Enable();
    }

    private void OnDisable()
    {
        inputs.AllyAction.Disable();
    }

    private void OnDestroy()
    {
        reticleController.onSelectionEvent.RemoveListener(HandleNewUnitSelection);
        reticleController.onTargetEvent.RemoveListener(RightClick);
        reticleController.onSelectionByTypeEvent.RemoveListener(HandleSelectionByType);
        partyManager.onHotkeyDragEvent.RemoveListener(HandleHotkeySwap);

        inputs.AllyAction.Default.started -= IssueDefaultCommand;
        inputs.AllyAction.Idle.started -= IssueIdleCommand;
        inputs.AllyAction.AttackFollow.started -= IssueAttackFollowCommand;
        inputs.AllyAction.AttackMove.started -= IssueAttackMoveCommand;
        inputs.AllyAction.Interact.started -= IssueInteractCommand;
        inputs.AllyAction.BuffSelf.started -= BuffSelf;
        inputs.AllyAction.AbilityOne.started -= AbilityOne;
        inputs.AllyAction.AbilityTwo.started -= AbilityTwo;
        inputs.AllyAction.AbilityThree.started -= AbilityThree;
        inputs.AllyAction.Disband.started -= Disband;
        inputs.AllyAction.DropItem.started -= DropItem;

        inputs.AllyAction.SelectAllAllies.started -= HandleSelectAllAllies;
        inputs.AllyAction.DeselectAllAllies.started -= HandleDeselectAllAllies;
        inputs.AllyAction.Multiselect.started -= StartMultiselect;
        inputs.AllyAction.Multiselect.canceled -= StopMultiselect;

        inputs.AllyAction.Hotkey1.started -= HandleHotKeySelect1;
        inputs.AllyAction.Hotkey2.started -= HandleHotKeySelect2;
        inputs.AllyAction.Hotkey3.started -= HandleHotKeySelect3;
        inputs.AllyAction.Hotkey4.started -= HandleHotKeySelect4;
        inputs.AllyAction.Hotkey5.started -= HandleHotKeySelect5;
        inputs.AllyAction.Hotkey6.started -= HandleHotKeySelect6;
        inputs.AllyAction.Hotkey7.started -= HandleHotKeySelect7;
        inputs.AllyAction.Hotkey8.started -= HandleHotKeySelect8;
        inputs.AllyAction.Hotkey9.started -= HandleHotKeySelect9;
    }

    private void HandleHotKeySelect1(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(0);
    }

    private void HandleHotKeySelect2(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(1);
    }

    private void HandleHotKeySelect3(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(2);
    }
    private void HandleHotKeySelect4(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(3);
    }
    private void HandleHotKeySelect5(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(4);
    }
    private void HandleHotKeySelect6(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(5);
    }
    private void HandleHotKeySelect7(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(6);
    }
    private void HandleHotKeySelect8(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(7);
    }
    private void HandleHotKeySelect9(InputAction.CallbackContext callbackContext)
    {
        HandleHotKeySelectAt(8);
    }

    public void AddUnit(NPCUnit unit) {
        if (!allUnits.units.Contains(unit)) {
            allUnits.AddUnit(unit);
            for(int i = 0; i < hotkeyUnits.Count; i++) {
                if(hotkeyUnits[i] == null) {
                    hotkeyUnits[i] = unit;
                    OnHotKeyUnitChange.Invoke(i+1, unit);
                    return;
                }
            }
        }
    }

    public void RemoveUnit(NPCUnit unit) {
        allUnits.RemoveUnit(unit);
        int i = hotkeyUnits.FindIndex(x => x==unit);
        if (i >= 0) {
            hotkeyUnits[i] = null;
            OnHotKeyUnitChange.Invoke(i+1, null);
        }
    }

    private void HandleHotkeySwap(int num1, int num2) {
        if (num1 != num2) {
            NPCUnit temp = hotkeyUnits[num1 - 1];
            hotkeyUnits[num1 - 1] = hotkeyUnits[num2 - 1];
            hotkeyUnits[num2 - 1] = temp;

            OnHotKeyUnitChange.Invoke(num1, hotkeyUnits[num1 - 1]);
            OnHotKeyUnitChange.Invoke(num2, hotkeyUnits[num2 - 1]);
        }
    }

    public void HandleHotKeySelectAt(int hotKeyIndex)
    {
        if (hotKeyIndex >= maxAllyCount) {
            return;
        }

        NPCUnit unit = hotkeyUnits[hotKeyIndex];

        if(multiselectFlag) {
            if (selectedUnitGroup.Contains(unit)) {
                DeselectUnit(unit);
                return;
            }
        } else {
            ClearSelectedUnits();
        }

        if (unit != null) {
            selectedUnitGroup.AddUnit(unit);
            unit?.OnUnitSelectionChanged.Invoke(true);
        }
        OnHotKeySelectedGroupChanged?.Invoke(hotKeyIndex + 1, selectedUnitGroup.units);
    }

    private void HandleSelectAllAllies(InputAction.CallbackContext callbackContext)
    {
        foreach (NPCUnit u in allUnits.units)
        {
            selectedUnitGroup.AddUnit(u);
            u.OnUnitSelectionChanged?.Invoke(true);
        }
        OnHotKeySelectedGroupChanged?.Invoke(0, selectedUnitGroup.units);
    }

    private void HandleDeselectAllAllies(InputAction.CallbackContext callbackContext)
    {
        ClearSelectedUnits();
    }


    private void HandleNewUnitSelection(List<GameObject> selectedUnits, bool dragging)
    {
        if(multiselectFlag) {
            if (!dragging) {
                NPCUnit unit = selectedUnits[0].GetComponent<NPCUnit>();
                if (unit != null && selectedUnitGroup.Contains(unit)) {
                    DeselectUnit(unit);
                    return;
                }
            }
        } else {
            ClearSelectedUnits();
        }

        // Add new selectedUnitGroup
        foreach (GameObject u in selectedUnits) {
            NPCUnit npcU = u.GetComponent<NPCUnit>();
            if (npcU != null)
            {
                if (selectedUnitGroup.AddUnit(npcU)) {
                    npcU.OnUnitSelectionChanged?.Invoke(true); // Tell UI that the unit is selected
                }
            }
        }
    }

    private void HandleSelectionByType(GameObject selectedUnit)
    {
        if(!multiselectFlag) {
            ClearSelectedUnits();
        }

        NPCUnit npcU = selectedUnit.GetComponent<NPCUnit>();
        if (npcU != null) {
            foreach (NPCUnit u in allUnits.units) {
                if (npcU.unitPrefab.npcUnitType == u.unitPrefab.npcUnitType) {
                    if (selectedUnitGroup.AddUnit(u)) {
                        u.OnUnitSelectionChanged?.Invoke(true); // Tell UI that the unit is selected
                    }
                }
            }
        }
    }

    private void DeselectUnit(NPCUnit unit)
    {
        unit.OnUnitSelectionChanged?.Invoke(false); // Tell UI that the unit is deselected
        selectedUnitGroup.RemoveUnit(unit);
    }

    // Default is attack closest because the natural behavior of non-possessed enemies are attacking the closest target
    private void IssueDefaultCommand(InputAction.CallbackContext callbackContext)
    {
        if (multiselectFlag)
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.DEFAULT), CommandMode.APPEND);
        else
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.DEFAULT));
    }

    private void IssueIdleCommand(InputAction.CallbackContext callbackContext)
    {
        if (multiselectFlag)
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.IDLE), CommandMode.APPEND);
        else
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.IDLE));
    }

    private void IssueAttackFollowCommand(InputAction.CallbackContext callbackContext)
    {
        if (multiselectFlag)
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ATTACK_FOLLOW), CommandMode.APPEND);
        else
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ATTACK_FOLLOW));
    }

    private void IssueAttackMoveCommand(InputAction.CallbackContext callbackContext)
    {
        if (multiselectFlag)
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ATTACK_MOVE, reticleController.GetReticleTransform().position), CommandMode.APPEND);
        else
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ATTACK_MOVE, reticleController.GetReticleTransform().position));
    }

    private void IssueInteractCommand(InputAction.CallbackContext callbackContext)
    {
        GameObject potentialTarget = reticleController.GetTargetObject();
        if (potentialTarget)
        {
            
            if (potentialTarget.CompareTag(Interactable.INTERACT_TAG))
            {
                if (multiselectFlag)
                    selectedUnitGroup.IssueCommandToAll(new Command(CommandType.INTERACT, potentialTarget.transform), CommandMode.APPEND);
                else
                    selectedUnitGroup.IssueCommandToAll(new Command(CommandType.INTERACT, potentialTarget.transform));
            }
            
        }
    }

    private void BuffSelf(InputAction.CallbackContext callbackContext)
    {
        GameObject potentialTarget = reticleController.GetTargetObject();
        if (potentialTarget)
        {
            if (Unit.IsNPC(potentialTarget.transform))
            {
                NPCUnit npcUnit = potentialTarget.GetComponent<NPCUnit>();
                npcUnit?.StunUnit?.Invoke(5f); // TESTING: the "buff" is stunning the target
            }
        }
    }

    private void AbilityOne(InputAction.CallbackContext callbackContext)
    {
        GameObject potentialTarget = reticleController.GetTargetObject();

        if (!potentialTarget.CompareTag("AI"))
        {

            GameObject tempTarget = Instantiate(emptyTargetObject);
            tempTarget.transform.position = reticleController.GetReticleTransform().position;
            potentialTarget = tempTarget;

        }
        
        if (potentialTarget)
        {
            // CommandMode is priority because it will use the ability, and then resume whatever it was doing
            // Note on how the command is executed: The UnitAttack controller (a unit's specialized Attack controller) performs the
            // ability immediately since it is the first command on the queue. Then it will signal that it has completed it.
            // At that point, the first (the ability command) will be popped of the front of the command queue and
            // the next command (whatever command it was previously doing) will be executed.
            // Also, if the unit is Enemy (not under player's control), the basic UnitAttack controller will have a chance to
            // insert a "use ability" command once in a while.
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ABILITY_ONE, potentialTarget, reticleController.GetReticleTransform().position), CommandMode.PRIORITY);
        }
    }

    private void AbilityTwo(InputAction.CallbackContext callbackContext)
    {
        GameObject potentialTarget = reticleController.GetTargetObject();
        if (potentialTarget)
        {
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ABILITY_TWO, potentialTarget, reticleController.GetReticleTransform().position), CommandMode.PRIORITY);
        }
    }
    private void AbilityThree(InputAction.CallbackContext callbackContext)
    {
        GameObject potentialTarget = reticleController.GetTargetObject();
        if (potentialTarget)
        {
            selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ABILITY_THREE, potentialTarget, reticleController.GetReticleTransform().position), CommandMode.PRIORITY);
        }
    }

    private void Disband(InputAction.CallbackContext callbackContext)
    {
        while (selectedUnitGroup.units.Count > 0)
        {
            NPCUnit nU = selectedUnitGroup.units[0];
            nU.OnUnitSelectionChanged?.Invoke(false); // Tell UI that the unit is deselected
            nU.ChangeAllyToEnemy(); // Should delete it from the list
        }

        OnHotKeySelectedGroupChanged?.Invoke(-1, selectedUnitGroup.units);
    }

    public void Disband(NPCUnit nU) {
        nU.OnUnitSelectionChanged?.Invoke(false); // Tell UI that the unit is deselected
        nU.ChangeAllyToEnemy(); // Should delete it from the list
    }

    private void RightClick(RaycastHit hit)
    {
        if(null == hit.transform)
        {
            return;
        }
        GameObject potentialTarget = hit.transform.gameObject;
        if (potentialTarget == null)
        {
            return;
        }

        if (potentialTarget.CompareTag(Interactable.INTERACT_TAG))
        {
            if (multiselectFlag)
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.INTERACT, potentialTarget.transform), CommandMode.APPEND);
            else
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.INTERACT, potentialTarget.transform));
            PlayTargetSound();
            return;
        }

        Transform t = potentialTarget.transform;
        if (!Unit.IsUnit(t)) // Issue MoveTo Command
        {
            if (multiselectFlag)
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.MOVE_TO, hit.point), CommandMode.APPEND);
            else
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.MOVE_TO, hit.point));
            PlayTargetSound();
        }
        else if (Unit.IsEnemy(t)) // Issue AttackTarget Command
        {
            if (multiselectFlag)
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ATTACK_TARGET, potentialTarget.transform), CommandMode.APPEND);
            else
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.ATTACK_TARGET, potentialTarget.transform));
            PlayTargetSound();
        }
        else if (Unit.IsFriend(t)) // Issue FollowTarget Command
        {
            if (multiselectFlag)
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.FOLLOW_TARGET, potentialTarget.transform), CommandMode.APPEND);
            else
                selectedUnitGroup.IssueCommandToAll(new Command(CommandType.FOLLOW_TARGET, potentialTarget.transform));

            PlayTargetSound();
        }


    }

    private void PlayTargetSound() {
        if(selectedUnitGroup.units.Count > 0) {
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.UI_TARGET_SFX);
        }
    }

    private void StartMultiselect(InputAction.CallbackContext ctx) {
        multiselectFlag = true;
    }

    private void StopMultiselect(InputAction.CallbackContext ctx) {
        multiselectFlag = false;
    }

    private void ClearSelectedUnits() {
        foreach (NPCUnit nU in selectedUnitGroup.units)
        {
            nU.OnUnitSelectionChanged?.Invoke(false); // Tell UI that the unit is deselected
        }
        // Clean up old selectedUnitGroup
        selectedUnitGroup.RemoveAllUnits();
    }

    /*
     * Units drop the first item in their inventory immediately without playing an animation (This does not use the command system)
     * The reason for this is because most units do not have an animation for dropping items, so a minecraft approach is used where the unit
     * immediately drops the item without disrupting their current actions/commands.
     * 
     * Also if units want to interact/drop items off, it can use an ItemDropOffPoint via the Interact command.
     */
    private void DropItem(InputAction.CallbackContext ctx)
    {
        foreach (NPCUnit nU in selectedUnitGroup.units)
        {
            if (nU.inventory.storage.Count > 0)
            {
                nU.DropItem(nU.inventory.storage[0], 999); // 999 effectively drops all items
            }
        }
    }
}
