using System.Collections;
using System.Collections.Generic;
using Neuromancer;
using UnityEngine;

public class MakeshiftPlatformVisualizerManager : MonoBehaviour
{

    [Header("Makeshift Platform First Reveal Settings")]
    [SerializeField] private GameObject mainPlatformVisualizer;
    [SerializeField] private float lerpToValueFlashing = 0.9f;
    [SerializeField] private float flickeringSpeedFlashing = 0.3f;
    [SerializeField] private float fresnelStrengthFlashing = 2f;
    [SerializeField] private float revealDuration = 0.8f;
    [SerializeField] private float disappearDuration = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    [SerializeField] private AnimationCurve disappearCurve;

    [Header("Makeshift Platform Merge to Platform Settings")]
    [SerializeField] private float flickeringSpeedPlatform = 0.3f;
    [SerializeField] private float fresnelStrengthPlatform = 2.86f;
    [SerializeField] private float lerpToValuePlatform = 0.9f;
    [SerializeField] private float shakeIterationDuration = 0.5f;
    [SerializeField] private int numShakeIterations = 3;
    [SerializeField] private float mergeToPlatformDuration = 0.4f;
    [SerializeField] private AnimationCurve platformRevealFlashStartCurve;
    [SerializeField] private AnimationCurve platformAnticipationStartCurve;
    [SerializeField] private AnimationCurve platformRevealEndCurve;

    private float lerpToValue_original;
    private float flickeringSpeed_original;
    private float fresnelStrength_original;
    private bool isVisualActive = false;
    private bool isMergingIntoPlatform = false;
    
    private Renderer render;
    private Material material;
    private ObjectPermanent permanent;
    private ItemDropOffPointV2 dropOff;
    private MakeshiftPlatformController controller;
    private bool isHeroInCollider = false;
    private List<NPCUnit> unitsInCollider = new List<NPCUnit>();
    
    
    private void Awake()
    {
        controller = GetComponent<MakeshiftPlatformController>();
        if(!controller.MainPlatform.activeInHierarchy && mainPlatformVisualizer != null)
        {
            dropOff = mainPlatformVisualizer.GetComponent<ItemDropOffPointV2>();
            render = mainPlatformVisualizer.GetComponent<Renderer>();
            permanent = mainPlatformVisualizer.GetComponent<ObjectPermanent>();
            material = render.material;

            mainPlatformVisualizer.SetActive(false);

            lerpToValue_original = material.GetFloat("_testLerpValue");
            flickeringSpeed_original = material.GetFloat("_FlickeringSpeed");
            fresnelStrength_original = material.GetFloat("_FresnelStrength");
        }
    }
    private void Start() 
    {
        if(mainPlatformVisualizer != null)
        {
            RevealPlatform(false);   
        }
        
    }

    public void RevealPlatform(bool hasEnteredRevealZone)
    {
        isVisualActive = hasEnteredRevealZone;
        StartCoroutine(RevealFlashSequence(hasEnteredRevealZone ? revealDuration : disappearDuration, hasEnteredRevealZone ? flashCurve : disappearCurve, hasEnteredRevealZone));      
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isMergingIntoPlatform || mainPlatformVisualizer == null) { return; }

        else if (other.CompareTag(Unit.HERO_TAG) || Unit.IsAlly(other.transform))
        {
            if(other.CompareTag(Unit.HERO_TAG)) { isHeroInCollider = true; }

            else if(Unit.IsAlly(other.transform))
            {
                NPCUnit collidingUnit = other.GetComponent<NPCUnit>();
                if(!unitsInCollider.Contains(collidingUnit))
                {
                    unitsInCollider.Add(collidingUnit);
                    collidingUnit.EmeraldComponent.DeathEvent.AddListener(RemoveUnitOnDeath);
                }
            }
            
           
            if(!isVisualActive)
            {
                RevealPlatform(true);
                return;
            }

            /* 
                All the commented-out code at the bottom are just conditionals to check if the tree is in the player's inventory or not,
                was used so all the tree highlighted visuals weren't all over the place, but smaller scope means I can ignore this.
                Will keep commented out in case I do want to revisit this conditional. Will remove if unecessary at all.
            */

            // if(other.CompareTag(Neuromancer.Unit.HERO_TAG))
            // {
            //     if(isVisualActive) { return; }
            //     if(UnitGroupManager.current.allUnits.units.Count > 0)
            //     {
            //         foreach(NPCUnit unit in UnitGroupManager.current.allUnits.units)
            //         {
            //             if(unit.inventory.CountInStorage(dropOff.possibleItems[0]) > 0)
            //             {
            //                 RevealPlatform(true);
            //                 return;
            //             }
            //         }  
            //     }
            // }

            // else
            // {
            //     NPCUnit collidingUnit = other.GetComponent<NPCUnit>();
            //     if(!unitsInCollider.Contains(collidingUnit) && collidingUnit.inventory.CountInStorage(dropOff.possibleItems[0]) > 0)
            //     {
            //         unitsInCollider.Add(collidingUnit);
            //     }
                
            //     // else
            //     // {
            //     //     if(unitsInCollider.Count > 0  && collidingUnit.inventory.CountInStorage(dropOff.possibleItems[0]) < 0)
            //     //     {
            //     //         unitsInCollider.Remove(collidingUnit);
            //     //     }
            //     // }

            //     if(collidingUnit.inventory.CountInStorage(dropOff.possibleItems[0]) > 0 && !isVisualActive)
            //     {
            //         RevealPlatform(true);
            //         return;
            //     }
            // }
        }
        
    }
    private void RemoveUnitOnDeath()
    {
        foreach(NPCUnit unit in unitsInCollider)
        {
            if(unit.isDead)
            {
                unit.EmeraldComponent.DeathEvent.RemoveListener(RemoveUnitOnDeath);
                if(unitsInCollider.Count == 1 && !isHeroInCollider)
                {
                    RevealPlatform(false);
                }
                unitsInCollider.Remove(unit);
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(isMergingIntoPlatform || mainPlatformVisualizer == null) { return; }

        if(!isVisualActive) { return; }
        
        if(other.CompareTag(Unit.HERO_TAG) || Unit.IsAlly(other.transform))
        {

            if(other.CompareTag(Unit.HERO_TAG)) { isHeroInCollider = false; }

            else if(Unit.IsAlly(other.transform))
            {
                NPCUnit collidingUnit = other.GetComponent<NPCUnit>();
                if(unitsInCollider.Contains(collidingUnit))
                {
                    unitsInCollider.Remove(collidingUnit);
                    collidingUnit.EmeraldComponent.DeathEvent.RemoveListener(RemoveUnitOnDeath);
                }
            }
            
            if(unitsInCollider.Count == 0 && !isHeroInCollider)
            {
                RevealPlatform(false);
            }
        }


        // if(other.CompareTag(Neuromancer.Unit.HERO_TAG) || Neuromancer.Unit.IsAlly(other.transform))
        // {
        //     if(Neuromancer.Unit.IsAlly(other.transform))
        //     {
        //         NPCUnit collidingUnit = other.GetComponent<NPCUnit>();
        //         unitsInCollider.Remove(collidingUnit);
        //     }

        //     // if any unit does NOT have the desired item and is still within the collider, turn off visualizer  
        //     bool anyUnitHasItem = false;
        //     foreach(NPCUnit unit in unitsInCollider)
        //     {
        //         if(unit.inventory.CountInStorage(dropOff.possibleItems[0]) > 0)
        //         {
        //             anyUnitHasItem = true;
        //             break;
        //         }
        //     }
        //     if (!anyUnitHasItem)
        //     {
        //         RevealPlatform(false);
        //     }
        // }
    }

    private IEnumerator RevealFlashSequence(float duration, AnimationCurve curve, bool isActive)
    {
        yield return new WaitForEndOfFrame();
        if(mainPlatformVisualizer == null) { yield break; }

        float time = 0;
        if(isActive)
        {
            mainPlatformVisualizer.SetActive(isActive);
        }

        material.SetFloat("_FlickeringSpeed", flickeringSpeedFlashing);
        while(time < duration)
        {
            time += Time.deltaTime;
            float percent = Mathf.Clamp01(time / duration);

            // Initiate tilt before properly fall
            float flashPercent = curve.Evaluate(percent);
            float targetLerpToValue = Mathf.Lerp(lerpToValue_original, lerpToValueFlashing, flashPercent);
            float targetFresnelStrength = Mathf.Lerp(fresnelStrength_original, fresnelStrengthFlashing, flashPercent);
            
            material.SetFloat("_testLerpValue", targetLerpToValue);   
            material.SetFloat("_FresnelStrength", targetFresnelStrength);

            if(isActive)
            {
                float targetFlickeringSpeed = Mathf.Lerp(flickeringSpeed_original, flickeringSpeedFlashing, flashPercent);
                material.SetFloat("_FlickeringSpeed", targetFlickeringSpeed);
            }
            
            yield return null;
        }
        
        if(!isActive)
        {
            mainPlatformVisualizer.SetActive(isActive);
        }
        
        material.SetFloat("_testLerpValue", lerpToValue_original);
        material.SetFloat("_FlickeringSpeed", flickeringSpeed_original);
        material.SetFloat("_FresnelStrength", fresnelStrength_original);
        
    }

    public void MergeIntoMainPlatform()
    {
        mainPlatformVisualizer.SetActive(true);
        isVisualActive = true;
        isMergingIntoPlatform = true;
        StartCoroutine(StartVisualMerge(shakeIterationDuration,numShakeIterations, 
        platformRevealFlashStartCurve, platformRevealEndCurve, controller));
    }
    private IEnumerator StartVisualMerge(float iterationDuration, float numIterations, AnimationCurve curveStart, AnimationCurve curveEnd, MakeshiftPlatformController target)
    {
        material.SetFloat("_FlickeringSpeed", flickeringSpeedPlatform);
        Vector3 targetStartupPosition = mainPlatformVisualizer.transform.position + Vector3.up * 1.2f;
        Vector3 targetStartupScale = mainPlatformVisualizer.transform.localScale * 1.05f;

        float totalStartupTime = 0f;
        for(int i = 0; i < numIterations; i++)
        {
            float time = 0;
            
            while(time < iterationDuration)
            {
                time += Time.deltaTime;
                totalStartupTime += Time.deltaTime;

                float percent = Mathf.Clamp01(time / iterationDuration);

                // Initiates flashing before properly fall
                float flashPercent = curveStart.Evaluate(percent);
                float targetLerpToValue = Mathf.Lerp(lerpToValue_original, lerpToValuePlatform, flashPercent);
                float targetFresnelStrength = Mathf.Lerp(fresnelStrength_original, fresnelStrengthPlatform, flashPercent);
                
                material.SetFloat("_testLerpValue", targetLerpToValue);   
                material.SetFloat("_FresnelStrength", targetFresnelStrength);

                // Initiate scale + position change before merge
                percent = Mathf.Clamp01(totalStartupTime / (iterationDuration * numIterations));
                float anticipationPercent = platformAnticipationStartCurve.Evaluate(percent);
                mainPlatformVisualizer.transform.position =  Vector3.Lerp(mainPlatformVisualizer.transform.position, targetStartupPosition, anticipationPercent);
                mainPlatformVisualizer.transform.localScale = Vector3.Lerp(mainPlatformVisualizer.transform.localScale, targetStartupScale, anticipationPercent);

                yield return null;
            }
        }
        mainPlatformVisualizer.transform.position =  targetStartupPosition;
        mainPlatformVisualizer.transform.localScale = targetStartupScale;


        float mergeTime = 0f;
        while(mergeTime < mergeToPlatformDuration)
        {
            mergeTime += Time.deltaTime;
            float percent = Mathf.Clamp01(mergeTime / iterationDuration);
            float mergePercent = curveEnd.Evaluate(percent);
            mainPlatformVisualizer.transform.position = Vector3.Lerp(mainPlatformVisualizer.transform.position, target.MainPlatform.transform.position, mergePercent);
            mainPlatformVisualizer.transform.localScale = Vector3.Lerp(mainPlatformVisualizer.transform.localScale, target.MainPlatform.transform.localScale, mergePercent);

            yield return null;
        }

        mainPlatformVisualizer.transform.position = target.MainPlatform.transform.position;
        mainPlatformVisualizer.transform.localScale = target.MainPlatform.transform.localScale;
        
        permanent.SetDestroy();
        Destroy(permanent.gameObject);
        target.ActivatePlatform();
    }



    
}
