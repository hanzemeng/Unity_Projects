using System.Collections;
using UnityEngine;
using EmeraldAI;

public class CorrosiveSlimeSkill : MonoBehaviour
{
    private Animator animator;
    private EmeraldAISystem emeraldAISystem;

    public bool isBusy;

    public GameObject pukeStormProjectile;
    public AudioManager.SoundResource pukeStormProjectileImpactSound;
    public GameObject pukeStormProjectileImpactExplosion;
    public float pukeStormInterval;
    private float pukeStormIntervalCooldown;
    public float pukeStormDuration;
    public float pukeStormProjectileInterval;
    public float pukeStormDetectionRadius;
    public float pukeStormProjectileHeight;
    public float pukeStormProjectileSpeed;
    public float pukeStormLandProjectileVariance;
    public float pukeStormSpawnProjectileVariance;
    public int pukeStormProjectileDamage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        emeraldAISystem = GetComponent<EmeraldAISystem>();

        isBusy = false;

        pukeStormIntervalCooldown = pukeStormInterval;
    }

    private void FixedUpdate()
    {
        if(isBusy || emeraldAISystem.IsDead)
        {
            return;
        }

        pukeStormIntervalCooldown -= Time.deltaTime;

        if(pukeStormIntervalCooldown < 0f)
        {
            PukeStorm();
        }
    }

    private void PukeStorm()
    {
        AnimatorClipInfo[] clipsInfo = animator.GetCurrentAnimatorClipInfo(0);
        if("attack" == clipsInfo[0].clip.name)
        {
            return;
        }

        StartCoroutine(PukeStormCoroutine());
    }

    private IEnumerator PukeStormCoroutine()
    {
        isBusy = true;

        emeraldAISystem.EmeraldEventsManagerComponent.CancelAttackAnimation();
        ResetEAI();
        emeraldAISystem.EmeraldEventsManagerComponent.ClearTarget();
        emeraldAISystem.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAISystem.CurrentBehavior.Passive);
        float originalDetectionFrequency = emeraldAISystem.DetectionFrequency;
        EmeraldAISystem.YesOrNo originalUseAggro = emeraldAISystem.UseAggro;
        emeraldAISystem.DetectionFrequency = 666f;
        emeraldAISystem.UseAggro = EmeraldAISystem.YesOrNo.No;

        emeraldAISystem.EmeraldEventsManagerComponent.PlayEmoteAnimation(1); // 1 is hit, in this case it is the animation for summoning the puke storm

        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Ally");
        layerMask |= 1 << LayerMask.NameToLayer("Hero");
        Collider[] colliders = Physics.OverlapSphere(transform.position, pukeStormDetectionRadius, layerMask);
        if(0 == colliders.Length)
        {
            goto EXIT;
        }

        float elapsedTime = 0f;
        float nextPukeStormProjectileTime = pukeStormProjectileInterval;
        while(elapsedTime < pukeStormDuration)
        {
            emeraldAISystem.EmeraldEventsManagerComponent.CancelAttackAnimation();
            elapsedTime += Time.deltaTime;
            if(elapsedTime > nextPukeStormProjectileTime)
            {
                nextPukeStormProjectileTime += pukeStormProjectileInterval;

                Vector2 positionVariance = pukeStormLandProjectileVariance * Random.insideUnitCircle;
                Vector3 targetPosition = colliders[Random.Range(0, colliders.Length)].transform.position + new Vector3(positionVariance.x, 0f, positionVariance.y);

                positionVariance = pukeStormSpawnProjectileVariance * Random.insideUnitCircle;
                PukeStormProjectile newProjectile = Instantiate(pukeStormProjectile, new Vector3(targetPosition.x + positionVariance.x, pukeStormProjectileHeight, targetPosition.z+positionVariance.y), Quaternion.identity).GetComponent<PukeStormProjectile>();

                newProjectile.impactSound = pukeStormProjectileImpactSound;
                newProjectile.impactExplosion = pukeStormProjectileImpactExplosion;
                newProjectile.fallingDirection = (targetPosition - newProjectile.transform.position).normalized;
                newProjectile.transform.rotation = Quaternion.LookRotation(newProjectile.fallingDirection);
                newProjectile.fallingSpeed = pukeStormProjectileSpeed;
                newProjectile.damage = pukeStormProjectileDamage;
            }
            yield return null;
        }

        EXIT:
        emeraldAISystem.UseAggro = originalUseAggro;
        emeraldAISystem.DetectionFrequency = originalDetectionFrequency;
        emeraldAISystem.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAISystem.CurrentBehavior.Aggressive);

        pukeStormIntervalCooldown = pukeStormInterval;
        isBusy = false;
    }

    private void ResetEAI()
    {
        int tempCurrentHealth = emeraldAISystem.CurrentHealth;
        emeraldAISystem.EmeraldEventsManagerComponent.ResetAI();
        emeraldAISystem.CurrentHealth = tempCurrentHealth;
    }
}
