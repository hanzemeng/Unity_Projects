using EmeraldAI;
using UnityEngine;

public class CustomColliderDamage : MonoBehaviour
{

    [HideInInspector] public IUnitAttackStat attackStat;
    
    private EmeraldAISystem emeraldComponent;
    private EmeraldAIEventsManager EventsManager;
    
    private Transform rootTransform;
    private Collider col;

    [SerializeField] private bool isProjectile = false;
    private bool fieldsSet = false;
    private float lastTime;

    private void Awake()
    {
        
        col = GetComponent<Collider>();
        col.enabled = false;
        
    }
    
    private void Start()
    {
        
        lastTime = -attackStat.tickRate;

        if (!isProjectile)
        {
            
            rootTransform = transform.root;
            emeraldComponent = rootTransform.GetComponent<EmeraldAISystem>();
            EventsManager = rootTransform.GetComponent<EmeraldAI.EmeraldAIEventsManager>();
            fieldsSet = true;
            col.enabled = false;

        }
        
    }

    public void SetFields(EmeraldAISystem component, EmeraldAIEventsManager events, Transform root)
    {

        rootTransform = root;
        emeraldComponent = component;
        EventsManager = events;

        fieldsSet = true;
        col.enabled = false;

    }

    private void OnTriggerEnter(Collider other)
    {

        if (!fieldsSet) return;
        
        EmeraldAI.EmeraldAISystem otherEmeraldAI = other.transform.root.GetComponent<EmeraldAI.EmeraldAISystem>();
        if (otherEmeraldAI != null)
        {
            if (EventsManager.GetAIRelation(otherEmeraldAI) == EmeraldAISystem.RelationType.Friendly)
            {
                return;
            }
            other.transform.root.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(attackStat.damage, EmeraldAISystem.TargetType.AI, rootTransform, 100);
            if (!attackStat.isAOE)
                col.enabled = false;

        } else if (other.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>() != null) {

            if (EventsManager.GetPlayerRelation() == EmeraldAISystem.RelationType.Friendly)
            {
                return;
            }
            other.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>().SendPlayerDamage(attackStat.damage, rootTransform, emeraldComponent);
            if (!attackStat.isAOE)
                col.enabled = false;

        }
        
    }

    private void OnTriggerStay(Collider other)
    {

        if (!attackStat.isDOT || !fieldsSet || Time.time < lastTime + attackStat.tickRate) return;

        lastTime = Time.time;
        
        EmeraldAI.EmeraldAISystem otherEmeraldAI = other.transform.root.GetComponent<EmeraldAI.EmeraldAISystem>();
        if (otherEmeraldAI != null)
        {
            if (EventsManager.GetAIRelation(otherEmeraldAI) == EmeraldAISystem.RelationType.Friendly)
            {
                return;
            }
            other.transform.root.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(attackStat.damage, EmeraldAISystem.TargetType.AI, rootTransform, 100);
            if (!attackStat.isAOE)
                col.enabled = false;

        } else if (other.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>() != null) {

            if (EventsManager.GetPlayerRelation() == EmeraldAISystem.RelationType.Friendly)
            {
                return;
            }
            other.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>().SendPlayerDamage(attackStat.damage, rootTransform, emeraldComponent);
            if (!attackStat.isAOE)
                col.enabled = false;

        }

    }
}
