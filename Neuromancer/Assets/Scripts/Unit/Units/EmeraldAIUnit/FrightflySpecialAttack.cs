using EmeraldAI;
using UnityEngine;

public class FrightflySpecialAttack : MonoBehaviour, IUnitAttackStat
{

    [SerializeField] private Transform projectileTransform;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private CustomAbility ability;

    [field: SerializeField] public int damage { get; set; }
    [field: SerializeField] public bool isAOE { get; set; }
    [field: SerializeField] public bool isDOT { get; set; }
    [field: SerializeField] public float tickRate { get; set; }

    [SerializeField] private float projectileLifespan;
    [SerializeField] private float projectileSpeed;

    private float lastTime;

    private EmeraldAISystem emeraldComponent;
    private EmeraldAIEventsManager eventsManager;

    private void Start()
    {

        emeraldComponent = GetComponent<EmeraldAISystem>();
        eventsManager = GetComponent<EmeraldAIEventsManager>();
        emeraldComponent.SwitchWeaponTypesCooldown = 0;
        lastTime = -ability.cooldown;
        emeraldComponent.SwitchWeaponTypesDistance = 30;

    }

    private void Update()
    {

        if (offCooldown())
        {

            emeraldComponent.abilityOneOnCooldown = false;
            if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
                emeraldComponent.SwitchWeaponTypesDistance = 0;

        }
        else
        {

            emeraldComponent.abilityOneOnCooldown = true;
            emeraldComponent.SwitchWeaponTypesDistance = 30;

        }
        
    }

    public void SpawnProjectile()
    {

        lastTime = Time.time;
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.position = projectileTransform.position;
        newProjectile.transform.forward = transform.forward;
        newProjectile.GetComponent<CustomColliderDamage>().attackStat = this;
        newProjectile.GetComponent<CustomColliderDamage>().SetFields(emeraldComponent, eventsManager, gameObject.transform.root);
        newProjectile.GetComponent<ProjectileMovement>().SetFields(projectileLifespan, projectileSpeed);
        newProjectile.GetComponent<Collider>().enabled = true;

    }

    public bool offCooldown()
    {

        return Time.time >= lastTime + ability.cooldown;

    }

}
