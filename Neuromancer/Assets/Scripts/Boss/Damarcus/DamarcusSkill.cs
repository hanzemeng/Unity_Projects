using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using EmeraldAI;

public class DamarcusSkill : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private EmeraldAISystem emeraldAISystem;

    private Collider[] nearbyUnits;

    public GameObject teleportSourceEffect;
    public GameObject teleportDestinationEffect;
    public float teleportInterval;
    private float teleportIntervalCooldown;

    public GameObject dashMoveEffect;
    public GameObject dashImpactEffect;
    public float dashInterval;
    public float dashMinimumDistance;
    public float dashMaximumDistance;
    private float dashIntervalCooldown;
    public float dashPrepareInterval;
    public float dashFinishInterval;
    public float dashSpeed;
    public int dashImpactDamage;
    private bool isBusy;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        emeraldAISystem = GetComponent<EmeraldAISystem>();

        StartCoroutine(CheckUnits());

        nearbyUnits = new Collider[0];

        teleportIntervalCooldown = teleportInterval;
        dashIntervalCooldown = dashInterval;
        isBusy = false;
    }

    private void FixedUpdate()
    {
        if(isBusy || emeraldAISystem.IsDead)
        {
            return;
        }

        teleportIntervalCooldown -= Time.deltaTime;
        dashIntervalCooldown -= Time.deltaTime;
        if(teleportIntervalCooldown < 0f)
        {
            Teleport();
        }
        if(dashIntervalCooldown < 0f)
        {
            Dash();
        }
    }

    private IEnumerator CheckUnits()
    {
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Ally");
        while(true)
        {
            nearbyUnits = Physics.OverlapSphere(transform.position, 5f, layerMask);
            yield return new WaitForSeconds(1f);
        }
    }

    private void Teleport()
    {
        if(0 == nearbyUnits.Length)
        {
            return;
        }
        AnimatorClipInfo[] clipsInfo = animator.GetCurrentAnimatorClipInfo(0);
        if("Attack01" == clipsInfo[0].clip.name)
        {
            return;
        }

        emeraldAISystem.EmeraldEventsManagerComponent.CancelAttackAnimation();
        emeraldAISystem.EmeraldEventsManagerComponent.ClearTarget();

        Instantiate(teleportSourceEffect, transform.position, Quaternion.identity);
        float randomRadian = Random.Range(0f, 2f*Mathf.PI);
        Vector3 newLoaction = Random.Range(10f,15f) * new Vector3(Mathf.Cos(randomRadian), 0f, Mathf.Sin(randomRadian));
        navMeshAgent.Warp(transform.position + newLoaction);
        Instantiate(teleportDestinationEffect, transform.position, Quaternion.identity);

        teleportIntervalCooldown = teleportInterval;
    }

    private void Dash()
    {
        if(0 != nearbyUnits.Length || (transform.position-PlayerController.player.transform.position).magnitude < dashMinimumDistance || (transform.position-PlayerController.player.transform.position).magnitude > dashMaximumDistance)
        {
            return;
        }
        AnimatorClipInfo[] clipsInfo = animator.GetCurrentAnimatorClipInfo(0);
        if("Attack01" == clipsInfo[0].clip.name)
        {
            return;
        }

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isBusy = true;

        Vector3 source = transform.position;
        Vector3 playerLocation = PlayerController.player.transform.position;
        Vector3 overDistance = 3f*(playerLocation-transform.position).normalized;
        Vector3 detination = playerLocation+overDistance;

        emeraldAISystem.EmeraldEventsManagerComponent.CancelAttackAnimation();
        ResetEAI();
        emeraldAISystem.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAISystem.CurrentBehavior.Passive);
        float originalDetectionFrequency = emeraldAISystem.DetectionFrequency;
        EmeraldAISystem.YesOrNo originalUseAggro = emeraldAISystem.UseAggro;
        emeraldAISystem.DetectionFrequency = 666f;
        emeraldAISystem.UseAggro = EmeraldAISystem.YesOrNo.No;

        emeraldAISystem.EmeraldEventsManagerComponent.RotateAITowardsTarget(PlayerController.player.transform, -1);
        emeraldAISystem.EmeraldEventsManagerComponent.LoopEmoteAnimation(1); // 1 is ready
        yield return new WaitForSeconds(dashPrepareInterval);
        emeraldAISystem.EmeraldEventsManagerComponent.StopLoopEmoteAnimation(1);
        ResetEAI();
        emeraldAISystem.EmeraldEventsManagerComponent.LoopEmoteAnimation(2); // 2 is run

        bool isAttacking = false;
        float nextDashMoveEffectDistance = 0f;
        float lerpAmountMultiplier = dashSpeed/(detination-source).magnitude;
        float startAttackAnimationValue = Mathf.Max(0.05f, ((detination-source).magnitude - 0.83f*dashSpeed)/(detination-source).magnitude);

        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            navMeshAgent.Warp(Vector3.Lerp(source, detination, lerpAmount));
            if((transform.position - source).magnitude > nextDashMoveEffectDistance)
            {
                Instantiate(dashMoveEffect, transform.position, Quaternion.identity);
                nextDashMoveEffectDistance += 0.5f;
            }
            if(!isAttacking && lerpAmount>startAttackAnimationValue)
            {
                emeraldAISystem.EmeraldEventsManagerComponent.StopLoopEmoteAnimation(2);
                ResetEAI();
                emeraldAISystem.EmeraldEventsManagerComponent.PlayEmoteAnimation(3); // 3 is attack
                isAttacking = true;
            }

            lerpAmount += lerpAmountMultiplier*Time.deltaTime;
            yield return null;
        }
        Instantiate(dashImpactEffect, transform.position+1.2f*Vector3.up, Quaternion.Euler(-90f,0f,0f));

        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Ally");
        layerMask |= 1 << LayerMask.NameToLayer("Hero");
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, layerMask);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject == gameObject)
            {
                continue;
            }

            if(null != collider.transform.GetComponent<EmeraldAI.EmeraldAISystem>())
            {
                collider.transform.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(dashImpactDamage, EmeraldAISystem.TargetType.AI, transform, 100);
            }
            else if(null != collider.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>())
            {
                collider.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>().SendPlayerDamage(dashImpactDamage, transform, GetComponent<EmeraldAISystem>());
            }
        }

        float tempDashFinishInterval = dashFinishInterval;
        while(tempDashFinishInterval>0f)
        {
            yield return null;
            tempDashFinishInterval -= Time.deltaTime;
            emeraldAISystem.EmeraldEventsManagerComponent.CancelAttackAnimation();
        }

        emeraldAISystem.EmeraldEventsManagerComponent.CancelRotateAITowardsTarget();
        emeraldAISystem.UseAggro = originalUseAggro;
        emeraldAISystem.DetectionFrequency = originalDetectionFrequency;
        emeraldAISystem.EmeraldEventsManagerComponent.ChangeBehavior(EmeraldAISystem.CurrentBehavior.Aggressive);
        emeraldAISystem.EmeraldEventsManagerComponent.OverrideCombatTarget(PlayerController.player.transform);

        dashIntervalCooldown = dashInterval;
        isBusy = false;
    }

    private void ResetEAI()
    {
        int tempCurrentHealth = emeraldAISystem.CurrentHealth;
        emeraldAISystem.EmeraldEventsManagerComponent.ResetAI();
        emeraldAISystem.CurrentHealth = tempCurrentHealth;
    }
}
