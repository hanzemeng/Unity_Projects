using EmeraldAI;
using UnityEngine;

public class DragonSpecialAttack : MonoBehaviour, IUnitAttackStat
{
    [SerializeField] private GameObject fireBreathObject;
    private Collider fireHitbox;
    private CustomColliderDamage colliderController;

    [SerializeField] private CustomAbility ability;
    [SerializeField] private int abilityIndex;
    
    [field: SerializeField] public int damage { get; set; }
    [field: SerializeField] public bool isAOE { get; set; }
    [field: SerializeField] public bool isDOT { get; set; }
    [field: SerializeField] public float tickRate { get; set; }
    
    
    [SerializeField] private ParticleSystem fireParticles;

    private float lastTime;
    
    private EmeraldAISystem emeraldComponent;
    private EmeraldAIEventsManager eventsManager;

    private void Start()
    {
        
        fireHitbox = fireBreathObject.GetComponent<Collider>();
        colliderController = fireBreathObject.GetComponent<CustomColliderDamage>();
        colliderController.attackStat = this;

        emeraldComponent = GetComponent<EmeraldAISystem>();
        eventsManager = GetComponent<EmeraldAIEventsManager>();

        lastTime = -ability.cooldown;
        emeraldComponent.MeleeAttacks[abilityIndex].AttackOdds = 0;

    }

    public void EnableFire()
    {

        lastTime = Time.time;
        fireParticles.Play();
        fireHitbox.enabled = true;

    }

    public void DisableFire()
    {

        fireParticles.Stop();
        fireHitbox.enabled = false;

    }

    private void Update()
    {
        
        if (offCooldown())
        {

            emeraldComponent.abilityOneOnCooldown = false;
            if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
                emeraldComponent.MeleeAttacks[abilityIndex].AttackOdds = 1000;

        }
        else
        {

            emeraldComponent.abilityOneOnCooldown = true;
            emeraldComponent.MeleeAttacks[abilityIndex].AttackOdds = 0;

        }
        
    }

    public bool offCooldown()
    {

        return Time.time >= lastTime + ability.cooldown;

    }

}
