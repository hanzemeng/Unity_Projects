using System.Collections.Generic;
using UnityEngine.Events;

using Neuromancer;

public class UnitGroup
{
    public List<NPCUnit> units { get; private set; } = new List<NPCUnit>();
    [System.NonSerialized] public UnityEvent<List<NPCUnit>> OnUnitChangeEvent = new UnityEvent<List<NPCUnit>>();

    public void IssueCommandToAll(Command command, CommandMode commandMode = CommandMode.REPLACE)
    {
        foreach (NPCUnit u in units)
        {
            u.IssueCommand(command, commandMode);
        }
    }

    public void RemoveUnit(NPCUnit unit)
    {
        units.Remove(unit);
        OnUnitChangeEvent.Invoke(units);
    }

    // Returns whether successfully added
    public bool AddUnit(NPCUnit unit)
    {
        if(!units.Contains(unit)) {
            units.Add(unit);
            unit.unitGroups.Add(this);
            OnUnitChangeEvent.Invoke(units);
            return true;
        } else {
            return false;
        }
    }

    public bool Contains(NPCUnit unit) {
        return units.Contains(unit);
    }

    public void RemoveAllUnits()
    {
        foreach (NPCUnit nU in units)
        {
            nU.unitGroups.Remove(this);
        }
        units.Clear();
        OnUnitChangeEvent.Invoke(units);
    }
}
