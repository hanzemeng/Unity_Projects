using EmeraldAI;
using UnityEngine;

public class GolemSpecialAttack : MonoBehaviour, IUnitAttackStat
{
    
    [SerializeField] private GameObject hitGroundObject;
    private Collider hitGroundHitbox;
    private CustomColliderDamage colliderController;

    [SerializeField] private GameObject hitGroundImpact;
    [SerializeField] private GameObject impactTarget;

    [SerializeField] private CustomAbility ability;
    [SerializeField] private int abilityIndex;
    
    [field: SerializeField] public int damage { get; set; }
    [field: SerializeField] public bool isAOE { get; set; }
    [field: SerializeField] public bool isDOT { get; set; }
    [field: SerializeField] public float tickRate { get; set; }

    private float lastTime;

    private EmeraldAISystem emeraldComponent;
    private EmeraldAIEventsManager eventsManager;

    private void Awake()
    {
        
        colliderController = hitGroundObject.GetComponent<CustomColliderDamage>();
        colliderController.attackStat = this;
        
    }
    
    private void Start()
    {
    
        hitGroundHitbox = hitGroundObject.GetComponent<Collider>();

        emeraldComponent = GetComponent<EmeraldAISystem>();
        eventsManager = GetComponent<EmeraldAIEventsManager>();

        lastTime = -ability.cooldown;
        emeraldComponent.MeleeAttacks[abilityIndex].AttackOdds = 0;

    }

    public void StartHitGround()
    {

        Instantiate(hitGroundImpact, impactTarget.transform.position, Quaternion.Euler(-90, 0, 0));
        lastTime = Time.time;
        hitGroundHitbox.enabled = true;

    }

    public void EndHitGround()
    {

        hitGroundHitbox.enabled = false;

    }

    private void Update()
    {
        if(Time.time > lastTime + 1f) EndHitGround();

        if (offCooldown())
        {

            emeraldComponent.abilityOneOnCooldown = false;
            if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
                emeraldComponent.MeleeAttacks[ability.animationIndex].AttackOdds = 1000;

        }
        else
        {

            emeraldComponent.abilityOneOnCooldown = true;
            emeraldComponent.MeleeAttacks[ability.animationIndex].AttackOdds = 0;

        }
        
    }

    public bool offCooldown()
    {

        return Time.time >= lastTime + ability.cooldown;

    }
    
}
