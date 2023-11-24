using EmeraldAI;
using EmeraldAI.Utility;
using UnityEngine;

public class BlossomSpecialProjectile : MonoBehaviour
{

    private GameObject shooter;
    private IUnitAttackStat parentAttack;
    [SerializeField] private GameObject explosionPrefab;
    
    private EmeraldAISystem emeraldComponent;
    private EmeraldAIEventsManager eventsManager;

    private bool fieldsSet = false;

    [SerializeField] private LayerMask nonUnitLayerCollision;

    private void Update()
    {

        if (!fieldsSet)
        {

            emeraldComponent = GetComponent<EmeraldAIProjectile>().EmeraldComponent;
            eventsManager = emeraldComponent.EmeraldEventsManagerComponent;
            shooter = emeraldComponent.gameObject;
            parentAttack = shooter.GetComponent<IUnitAttackStat>();
            fieldsSet = true;

        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!fieldsSet) return;

        if ((nonUnitLayerCollision & (1 << other.gameObject.layer)) != 0)
        {
            
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            explosion.GetComponent<CustomColliderDamage>().attackStat = parentAttack;
            explosion.GetComponent<CustomColliderDamage>().SetFields(emeraldComponent, eventsManager, shooter.transform);
            explosion.GetComponent<Collider>().enabled = true;
            Destroy(gameObject);
            
        }

        EmeraldAISystem otherEmeraldAI = other.transform.GetComponent<EmeraldAI.EmeraldAISystem>();
        if (otherEmeraldAI != null)
        {

            if (eventsManager.GetAIRelation(otherEmeraldAI) == EmeraldAISystem.RelationType.Friendly)
            {
                return;
            }
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            explosion.GetComponent<CustomColliderDamage>().attackStat = parentAttack;
            explosion.GetComponent<CustomColliderDamage>().SetFields(emeraldComponent, eventsManager, shooter.transform);
            explosion.GetComponent<Collider>().enabled = true;
            Destroy(gameObject);

        } else if (other.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>() != null) {
            
            if (eventsManager.GetPlayerRelation() == EmeraldAISystem.RelationType.Friendly)
            {
                return;
            }

            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            explosion.GetComponent<CustomColliderDamage>().attackStat = parentAttack;
            explosion.GetComponent<CustomColliderDamage>().SetFields(emeraldComponent, eventsManager, shooter.transform);
            explosion.GetComponent<Collider>().enabled = true;
            Destroy(gameObject);

        }

    }
}