using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Neuromancer;
using TMPro;

[System.Serializable]
public class CommandIconPair {
    public CommandType commandType;
    public Sprite icon;
}

public class UnitOverheadIcons : MonoBehaviour {

    
    [SerializeField] private GameObject templateIcon;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Sprite defaultCommandIcon;
    [SerializeField] private List<CommandIconPair> commandIcons;
    private List<GameObject> iconList;
    private NPCUnit unit;
    private Inventory inventory;
    private List<Command> commandQueue;
    
    private void Start() {
        iconList = new List<GameObject>();
        SetUnit(GetComponentInParent<NPCUnit>());
    }

    public void SetUnit(NPCUnit unit) {
        if(this.unit == unit) {
            return;
        }

        RemoveListeners();
        this.unit = unit;

        if (unit == null) {
            Refactor();
            return;
        }
        inventory = unit.inventory;
        commandQueue = unit.commandQueue;
        
        AddListeners();
        Refactor();
    }

    private void OnDestroy() {
        RemoveListeners();
    }

    private void AddListeners() {
        if(unit != null) {
            inventory?.OnInventoryChangeEvent.AddListener(Refactor);
            unit.OnCommandFinish += Refactor;
            unit.OnCommandIssued += Refactor;
        }
    }

    private void RemoveListeners() {
        if(unit != null) {
            inventory.OnInventoryChangeEvent.RemoveListener(Refactor);
            unit.OnCommandFinish -= Refactor;
            unit.OnCommandIssued -= Refactor;
        }
    }

    private void Refactor(int _) {
        Refactor();
    }

    private void Refactor() {
        foreach (GameObject go in iconList) {
            Destroy(go);
        }
        iconList.Clear();

        if(unit == null) {
            return;
        }

        if(Unit.IsEnemy(unit.transform)) {
            return;
        }

        if(commandQueue.Count > 0) {
            CommandType commandType = commandQueue[0].commandType;
            foreach(CommandIconPair commandIconPair in commandIcons) {
                if(commandType == commandIconPair.commandType) {
                    GameObject icon = Instantiate(templateIcon, iconContainer);
                    Image image = icon.GetComponent<Image>();
                    image.sprite = commandIconPair.icon;
                    TextMeshProUGUI countText = icon.GetComponentInChildren<TextMeshProUGUI>();
                    countText.enabled = false;
                    iconList.Add(icon);
                    break;
                }
            }
        } else {
            GameObject icon = Instantiate(templateIcon, iconContainer);
            Image image = icon.GetComponent<Image>();
            image.sprite = defaultCommandIcon;
            TextMeshProUGUI countText = icon.GetComponentInChildren<TextMeshProUGUI>();
            countText.enabled = false;
            iconList.Add(icon);
        }

        List<InventoryItem> items = inventory.storage;

        for (int i = 0; i < items.Count; i++) {
            GameObject icon = Instantiate(templateIcon, iconContainer);
            Image image = icon.GetComponent<Image>();
            TextMeshProUGUI countText = icon.GetComponentInChildren<TextMeshProUGUI>();

            InventoryItem item = items[i];
            image.sprite = item.itemData.icon;

            if (item.count > 1) {
                countText.enabled = true;
                countText.text = "x" + item.count;
            } else {
                countText.enabled = false;
            }

            iconList.Add(icon);
        }
    }
}