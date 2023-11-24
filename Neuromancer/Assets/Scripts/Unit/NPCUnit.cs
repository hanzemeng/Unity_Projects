using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Neuromancer
{
    public class NPCUnit : Unit
    {
        public EmeraldAI.EmeraldAISystem EmeraldComponent { get; private set; }
        public EmeraldAI.EmeraldAIEventsManager EmeraldEventsManagerComponent { get; private set; }

        public EnemyUnit unitPrefab;

        public bool isDead = false;
        
        public GameObject deathExplosionPrefab;

        public int neuronDrops = 1;
        public GameObject neuronExplosionPrefab;

        [field: SerializeField]
        public List<Command> commandQueue { get; private set; } = new List<Command>();


        public UnityAction<int> OnCommandIssued;
        public UnityAction OnCommandFinish;

        public bool dropInventoryOnDeath = true;

        public Inventory inventory;

        private UnitMentalStamina unitMentalStamina;

        public List<UnitGroup> unitGroups = new List<UnitGroup>();

        [System.NonSerialized] public UnityEvent<bool> OnUnitSelectionChanged = new UnityEvent<bool>();
        private Sprite icon;

        // List of events/actions other things can trigger (Events above are for unit internal uses or are intended to be LISTENED TO only)
        public UnityAction<float> StunUnit; // input: duration

        [System.NonSerialized] public UnityEvent onConvertToAlly = new UnityEvent();
        [System.NonSerialized] public UnityEvent onConvertToEnemy = new UnityEvent();

        [HideInInspector] public ImprovedUnitSpawner sourceSpawner;

        public new void Awake() // "new" keyword hides/overrides the inherited 
        {
            EmeraldComponent = GetComponent<EmeraldAI.EmeraldAISystem>();
            EmeraldEventsManagerComponent = GetComponent<EmeraldAI.EmeraldAIEventsManager>();

            unitMentalStamina = GetComponent<UnitMentalStamina>();
            unitMentalStamina.onZeroStaminaEvent.AddListener(ChangeEnemyToAlly);
            icon = unitPrefab.icon;
            EmeraldComponent.DeathEvent.AddListener(Die);

            if (unitPrefab.inventorySpace > EnemyUnit.MAX_INVENTORY_SPACE)
                unitPrefab.inventorySpace = EnemyUnit.MAX_INVENTORY_SPACE;
            inventory = new Inventory(unitPrefab.inventorySpace);
        }


        public void IssueCommand(Command command, CommandMode commandMode = CommandMode.REPLACE)
        {

            if (command.commandType == CommandType.DEFAULT)
            {
                this.ClearCommandQueue();
                OnCommandIssued?.Invoke(0);
                return;
            }

            switch (commandMode)
            {
                case CommandMode.APPEND:
                    commandQueue.Add(command);
                    OnCommandIssued?.Invoke(commandQueue.Count - 1);
                    break;
                case CommandMode.REPLACE:
                    this.ClearCommandQueue();
                    commandQueue.Add(command);
                    OnCommandIssued?.Invoke(0);
                    break;
                case CommandMode.PRIORITY:
                    commandQueue.Insert(0, command);
                    OnCommandIssued?.Invoke(0);
                    break;
                default:
                    Debug.Log("Invalid CommandMode");
                    break;
            }
            
        }

        public void ClearCommandQueue()
        {
            commandQueue.Clear();
        }
        
        public Command GetCurrentCommand()
        {
            if (commandQueue.Count == 0)
                return new Command();
            return commandQueue[0];
        }

        public void PopCurrentCommand()
        {
            if (commandQueue.Count > 0)
            {
                commandQueue.RemoveAt(0);
                OnCommandIssued?.Invoke(0);
            }
        }

        public void FinishCurrentCommand(Command command)
        {
            if (command.commandType == GetCurrentCommand().commandType) // INFO: Might want to "deep check" instead of "shallow check"
            {
                //Debug.Log("Finished" + command.commandType + GetCurrentCommand().commandType);
                OnCommandFinish?.Invoke();
                PopCurrentCommand();
            }
        }

        private void ChangeEnemyToAlly(bool hasZeroMentalStamina)
        {
            
            if (!unitPrefab.canTakeOver)
            {
                StunUnit?.Invoke(3f);
                return;
            }

            if (UnitGroupManager.current.allUnits.units.Count >= UnitGroupManager.current.maxAllies)
            {
                return;
            }

            if (IsAlly(transform))
            {
                return;
            }

            if (!hasZeroMentalStamina)
            {
                return;
            }

            GameObject ally = Instantiate(unitPrefab.allyPrefab, transform.position - new Vector3(0, EmeraldComponent.m_NavMeshAgent.baseOffset, 0), transform.rotation);
            ally.transform.rotation = transform.rotation; // it just being weird

            NPCUnit allyNPCunit = ally.GetComponent<NPCUnit>();
            if (allyNPCunit != null)
            {
                UnitGroupManager.current.AddUnit(allyNPCunit);
                allyNPCunit.IssueCommand(new Command(CommandType.ATTACK_FOLLOW));
                allyNPCunit.onConvertToAlly.Invoke();
                foreach (InventoryItem item in inventory.storage)
                {
                    allyNPCunit.inventory.AddItem(item.itemData, item.uniqueId, item.count);
                }
            }
            if (sourceSpawner != null)
            {
                sourceSpawner.RemoveUnit(gameObject);
            }

            Destroy(gameObject);
        }

        public void ChangeAllyToEnemy()
        {
            if (IsEnemy(transform))
            {
                return;
            }


            UnitGroupManager.current.RemoveUnit(this);
            foreach (UnitGroup uG in unitGroups)
            {
                uG.RemoveUnit(this);
            }

            GameObject enemy = Instantiate(unitPrefab.enemyPrefab, transform.position - new Vector3(0, EmeraldComponent.m_NavMeshAgent.baseOffset, 0), transform.rotation);
            enemy.transform.rotation = transform.rotation;

            NPCUnit enemyNPCUnit = enemy.GetComponent<NPCUnit>();
            foreach (InventoryItem item in inventory.storage)
            {
                enemyNPCUnit?.inventory.AddItem(item.itemData, item.uniqueId, item.count);
            }

            onConvertToEnemy.Invoke();
            Destroy(gameObject);
        }


        public void Die()
        {
            if (sourceSpawner != null)
            {
                sourceSpawner.RemoveUnit(gameObject);
            }
            EmeraldComponent.DeathEvent.RemoveListener(Die);
            // Unit Group Cleanup
            UnitGroupManager.current.RemoveUnit(this);
            foreach (UnitGroup uG in unitGroups)
            {
                uG.RemoveUnit(this);

            }

            if (isDead)
            {
                return;
            }
            isDead = true;
            StartCoroutine(DeleteUnit());
        }

        private IEnumerator DeleteUnit()
        {

            yield return new WaitForSeconds(1.5f);
            // Only drop items if option is turned on
            if (dropInventoryOnDeath)
                HandleOnDeathDropItem();
            if (neuronExplosionPrefab != null)
            {
                for (int i = 0; i < neuronDrops; i++)
                {
                    Instantiate(neuronExplosionPrefab, gameObject.transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(-90,0,0));
                }
            }

            Instantiate(deathExplosionPrefab, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);

        }

        public Sprite GetIcon() {
            return icon;
        }

        private void HandleOnDeathDropItem()
        {
            while (inventory.storage.Count > 0)
            {
                ItemData item = inventory.storage[0].itemData;
                string id = inventory.storage[0].uniqueId;
                if (item == null)
                {
                    Debug.LogError(gameObject.name + "'s inventory item data is missing. Aborting drops...");
                    return;
                }

                int numberRemoved = inventory.RemoveItem(item, inventory.CountInStorage(item));
                if (unitPrefab.deathFX == null)
                {
                    GameObject newObject = Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                    CarriableItem carriableItem = newObject.GetComponent<CarriableItem>();
                    carriableItem.count = numberRemoved;
                    carriableItem.SetID(id);
                }
                else
                {
                    GameObject deathDrop = Instantiate(unitPrefab.deathFX, EmeraldComponent.HitPointTransform.position, Quaternion.Euler(-90f, 0f, gameObject.transform.localEulerAngles.y - 90f));
                    ItemDropController itemDropController = deathDrop.GetComponent<ItemDropController>();
                    if (itemDropController == null) return;
                    itemDropController.item = item.itemPrefab;
                    itemDropController.count = numberRemoved;
                    itemDropController.uniqueId = id;
                    ParticleSystem partSys = deathDrop.GetComponent<ParticleSystem>();
                    var partSysSettings = partSys.main;
                    partSysSettings.startSize = item.itemPrefab.transform.localScale.x;
                }
            }
        }

        public int DropItem(InventoryItem item, int count = 1)
        {
            if (item == null)
            {
                Debug.LogError(gameObject.name + " failed to drop item due to NULL itemData param");
                return -1;
            }

            int numberRemoved = inventory.RemoveItem(item.itemData, count);
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.CARRIABLE_DROP);
            if (unitPrefab.dropItemFX == null)
            {
                GameObject newObject = Instantiate(item.itemData.itemPrefab, transform.position, Quaternion.identity);
                CarriableItem carriableItem = newObject.GetComponent<CarriableItem>();
                carriableItem.count = numberRemoved;
                carriableItem.SetID(item.uniqueId);
            }
            else
            {
                GameObject itemDrop = Instantiate(unitPrefab.dropItemFX, EmeraldComponent.HitPointTransform.position, Quaternion.Euler(-90f, 0f, gameObject.transform.localEulerAngles.y - 90f));
                ItemDropController itemDropController = itemDrop.GetComponent<ItemDropController>();
                if (itemDropController == null) return 0;
                itemDropController.item = item.itemData.itemPrefab;
                itemDropController.count = numberRemoved;
                itemDropController.targetLayer = item.itemData.targetLayer;
                itemDropController.uniqueId = item.uniqueId;
                ParticleSystem partSys = itemDrop.GetComponent<ParticleSystem>();
                var partSysSettings = partSys.main;
                partSysSettings.startSize = item.itemData.itemPrefab.transform.localScale.x;
            }
            return numberRemoved;
        }
    }
}