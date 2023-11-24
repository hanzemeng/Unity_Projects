using UnityEngine;

[CreateAssetMenu(menuName = "Interactable/InteractableGroup")]
public class InteractableGroup : ScriptableObject
{
    public enum Type
    {
        NEUTRAL = 0,
        SHARP,
        BLUNT,
    }

    public InteractableGroup.Type type;
    public Neuromancer.NPCUnitType[] members;
}
