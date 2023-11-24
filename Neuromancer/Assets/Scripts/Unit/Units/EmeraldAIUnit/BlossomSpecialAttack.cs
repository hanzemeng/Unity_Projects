using EmeraldAI;
using UnityEngine;

public class BlossomSpecialAttack : MonoBehaviour, IUnitAttackStat
{
    
    [field: SerializeField] public int damage { get; set; }
    [field: SerializeField] public bool isAOE { get; set; }
    [field: SerializeField] public bool isDOT { get; set; }
    [field: SerializeField] public float tickRate { get; set; }

    [SerializeField] private CustomAbility ability;
    [SerializeField] private int abilityIndex;
    
    private float lastTime;
    
    private EmeraldAISystem emeraldComponent;
    private EmeraldAIEventsManager eventsManager;
    
    private void Start()
    {

        emeraldComponent = GetComponent<EmeraldAISystem>();
        eventsManager = GetComponent<EmeraldAIEventsManager>();
        lastTime = -ability.cooldown;
        emeraldComponent.OffensiveAbilities[abilityIndex].AbilityOdds = 0;

    }

    private void Update()
    {
        
        if (offCooldown())
        {

            emeraldComponent.abilityOneOnCooldown = false;
            if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
                emeraldComponent.OffensiveAbilities[abilityIndex].AbilityOdds = 1000;

        }
        else
        {

            emeraldComponent.abilityOneOnCooldown = true;
            emeraldComponent.OffensiveAbilities[abilityIndex].AbilityOdds = 0;

        }
        
    }

    public bool offCooldown()
    {

        return Time.time >= lastTime + ability.cooldown;

    }

    public void StartCooldown()
    {

        lastTime = Time.time;

    }
    
}
