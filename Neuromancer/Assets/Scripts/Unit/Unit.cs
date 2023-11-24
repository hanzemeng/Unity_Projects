using UnityEngine;
using UnityEngine.Events;

namespace Neuromancer // needed to avoid namespace conflicts
{

    public enum NPCUnitType
    {
        DEFAULT = 0,
        TREANT,
        DRAGON,
        SPIDER,
        GOLEM,
        EVIL_MUSHROOM,
        FOREST_BAT,
        SHELL,
        BUD,
        BLOSSOM,
        SLIME,
        FRIGHTFLY,
        DEEP_SEA_LIZARD,
        BOMB,
    }

    public class Unit : MonoBehaviour
    {

        public static string HERO_TAG = "Hero";
        public static string NPC_TAG = "AI";
        public static string DEAD_TAG = "Dead";

        public static string HERO_LAYER_NAME = "Hero";
        public static string HERO_NO_COLLIDE_LAYER_NAME = "HeroNoCollide";
        public static string ENEMY_LAYER_NAME = "Enemy";
        public static string ALLY_LAYER_NAME = "Ally";
        public static string DEAD_LAYER_NAME = "Dead";

        [System.NonSerialized] public UnityEvent<bool> OnNewUnitTypeEvent = new UnityEvent<bool>();

        public void Awake()
        {
            transform.tag = HERO_TAG;
            gameObject.layer = LayerMask.NameToLayer(HERO_LAYER_NAME);
            OnNewUnitTypeEvent.Invoke(true);
        }

        public static bool IsUnit(Transform target)
        {
            return target.CompareTag(HERO_TAG) || target.CompareTag(NPC_TAG);
        }

        public static bool IsNPC(Transform target)
        {
            return target.CompareTag(NPC_TAG);
        }

        public static bool IsEnemy(Transform target)
        {
            EmeraldAI.EmeraldAIFactionData FactionData = Resources.Load("Faction Data") as EmeraldAI.EmeraldAIFactionData;
            NPCUnit npcUnit = target.GetComponent<NPCUnit>();

            if (npcUnit != null && npcUnit.EmeraldComponent.CurrentFaction == FactionData.FactionNameList.IndexOf("Enemy"))
                return true;
            return false;
        }

        // True if either the Hero or Ally
        public static bool IsFriend(Transform target)
        {
            return target.CompareTag(HERO_TAG) || IsAlly(target);
        }

        public static bool IsHero(Transform target)
        {
            return target.CompareTag(HERO_TAG);
        }

        public static bool IsAlly(Transform target)
        {
            EmeraldAI.EmeraldAIFactionData FactionData = Resources.Load("Faction Data") as EmeraldAI.EmeraldAIFactionData;
            NPCUnit npcUnit = target.GetComponent<NPCUnit>();

            if (npcUnit != null && npcUnit.EmeraldComponent.CurrentFaction == FactionData.FactionNameList.IndexOf("Ally"))
                return true;
            return false;
        }
    }
}

