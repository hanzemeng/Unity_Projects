using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractablePlayerDetector : MonoBehaviour
{
    [Header("Highlight Shader Parameters")]
    [SerializeField] private bool initializeShaderOnAwake = true;
    [SerializeField] private Shader highlightShader;
    [SerializeField] private Color highlightRimColor = Color.red;
    [SerializeField, Range(0, 50f)] private float minHighlightStrength = 8;
    [SerializeField, Range(0, 50f)] private float maxHighlightStrength = 12;
    [SerializeField, Range(0, 20f)] private float highlightFlickerSpeed = 5;
    [Tooltip("Turns off and on the outline component during runtime.\nNote: This was my very hacky solution to some objects not responding properly to reticle hovering for whatever reason, regardless of execution order.")]
    [SerializeField] private bool reenableOutline = false;
    [Tooltip("Will only do a specified set of gameObjects, leave empty if you want to apply to all children.")]
    [SerializeField] private Transform[] chosenObjectsToHighlight;

    [Header("Animation Curves for Shader")]
    [SerializeField, Range(0, 10f)] private float highlightAppearDuration = 3f;
    [SerializeField] private AnimationCurve highlightAppearCurve;
    [SerializeField, Range(0, 10f)] private float highlightDisappearDuration = 2f;
    [SerializeField] private AnimationCurve highlightDisappearCurve;


    private List<GameObject> objectOnTrigger = new List<GameObject>();
    private Dictionary<Renderer, Material[]> allBaseMaterialsDict = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> allHighlightedMaterialsDict = new Dictionary<Renderer, Material[]>();

    private List<Renderer> allRenderers = new List<Renderer>();
    private List<Material> allHighlightedMaterials = new List<Material>();
    private Color targetColor;

    private Outline interactableOutline;

    private bool isReticleHoveringObject = false;
    private bool isHighlighted = false;
    private bool isPermanentlyDisabled = false;

    private void Awake()
    {
        if(initializeShaderOnAwake)
        {
            if(chosenObjectsToHighlight.Length > 0)
            {
                InitializeSpecifiedShaders(chosenObjectsToHighlight);
            }
            else
            {
                InitializeShaderRecursively(transform);
            }
            
        }
        
    }
    private void Start()
    {
        if(!initializeShaderOnAwake)
        {
            if(chosenObjectsToHighlight.Length > 0)
            {
                InitializeSpecifiedShaders(chosenObjectsToHighlight);
            }
            else
            {
                InitializeShaderRecursively(transform);
            }
        }

        ReticleController.current?.onUnitFocusChangeEvent.AddListener(Focus);
        interactableOutline = gameObject.GetComponent<Outline>();
        if(reenableOutline && interactableOutline != null)
        {
            foreach(Renderer render in allRenderers)
            {
                if(allBaseMaterialsDict.ContainsKey(render)) { render.materials = allBaseMaterialsDict[render]; }
            }
        }
    }

    private void InitializeSpecifiedShaders(Transform[] objs)
    {
        foreach(Transform obj in objs)
        {
            Renderer render = obj.GetComponent<Renderer>();
            if(render != null)
            {
                allRenderers.Add(render);
                List<Material> allCurrentBaseMaterials = new List<Material>();
                List<Material> allCurrentHighlightedMaterials = new List<Material>();
                foreach(Material material in render.materials)
                {
                    // Skip all outline materials
                    if(material.HasProperty("_ZTest") || material.HasProperty("_OutlineColor"))
                    {
                        continue;
                    }

                    allCurrentBaseMaterials.Add(material);

                    // Applies all settings to the interactable
                    Material highlightMat = new Material(highlightShader);
                    if(highlightMat.HasProperty("_MainTex"))
                    {
                        highlightMat.SetTexture("_MainTex", material.mainTexture);
                    }

                    if(highlightMat.HasProperty("_Color"))
                    {
                        highlightMat.SetColor("_Color", material.color);
                    }
                    highlightMat.SetVector("_RimPower", new Vector2(minHighlightStrength, maxHighlightStrength));
                    highlightMat.SetColor("_RimColor", Color.black);
                    highlightMat.SetFloat("_PulseSpeed", highlightFlickerSpeed);
                    allCurrentHighlightedMaterials.Add(highlightMat);
                    allHighlightedMaterials.Add(highlightMat);
                }

                if(!allBaseMaterialsDict.ContainsKey(render) && !allHighlightedMaterialsDict.ContainsKey(render))
                {
                    allBaseMaterialsDict.Add(render, allCurrentBaseMaterials.ToArray());
                    allHighlightedMaterialsDict.Add(render, allCurrentHighlightedMaterials.ToArray());
                }
            }
        }

    }
    // Loop through all the materials attached to each gameObject (sometimes there's multiple materials per renderer like the KeyLock)
    private void InitializeShaderRecursively(Transform parent)
    {
        Renderer render = parent.GetComponent<Renderer>();
        if(render != null)
        {
            allRenderers.Add(render);
            List<Material> allCurrentBaseMaterials = new List<Material>();
            List<Material> allCurrentHighlightedMaterials = new List<Material>();
            foreach(Material material in render.materials)
            {
                // Skip all outline materials
                if(material.HasProperty("_ZTest") || material.HasProperty("_OutlineColor"))
                {
                    continue;
                }

                allCurrentBaseMaterials.Add(material);

                // Applies all settings to the interactable
                Material highlightMat = new Material(highlightShader);
                if(highlightMat.HasProperty("_MainTex"))
                {
                    highlightMat.SetTexture("_MainTex", material.mainTexture);
                }

                if(highlightMat.HasProperty("_Color"))
                {
                    highlightMat.SetColor("_Color", material.color);
                }
                highlightMat.SetVector("_RimPower", new Vector2(minHighlightStrength, maxHighlightStrength));
                highlightMat.SetColor("_RimColor", Color.black);
                highlightMat.SetFloat("_PulseSpeed", highlightFlickerSpeed);
                allCurrentHighlightedMaterials.Add(highlightMat);
                allHighlightedMaterials.Add(highlightMat);
            }

            if(!allBaseMaterialsDict.ContainsKey(render) && !allHighlightedMaterialsDict.ContainsKey(render))
            {
                allBaseMaterialsDict.Add(render, allCurrentBaseMaterials.ToArray());
                allHighlightedMaterialsDict.Add(render, allCurrentHighlightedMaterials.ToArray());
            }
        }
        else
        {
            return;
        }

        foreach(Transform child in parent)
        {
            if(child.GetComponent<MeshRenderer>() != null)
            {
                InitializeShaderRecursively(child);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(other.transform))
        {
            objectOnTrigger.Add(other.gameObject);
            ActivateHighlight(true);
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(other.transform))
        {
            objectOnTrigger.Remove(other.gameObject);
            if(objectOnTrigger.Count == 0 && !isReticleHoveringObject)
            {
                ActivateHighlight(false);
            } 
        } 
    }

    public void AddToObjectCounter(GameObject unit)
    {
        objectOnTrigger.Add(unit);
        ActivateHighlight(true);
    }

    public void RemoveFromObjectCounter(GameObject unit)
    {
        objectOnTrigger.Remove(unit);
        if(objectOnTrigger.Count == 0 && !isReticleHoveringObject)
        {
            ActivateHighlight(false);
        } 
    }

    private void OnEnable() 
    {
        ReticleController.current?.onUnitFocusChangeEvent.AddListener(Focus);
    }

    private void OnDisable() 
    {
        ReticleController.current?.onUnitFocusChangeEvent.RemoveListener(Focus);
    }

    private void Focus(List<GameObject> focusedObjects) 
    {
        if(focusedObjects.Contains(gameObject) && !isReticleHoveringObject)
        {
            isReticleHoveringObject = true;
            ActivateHighlight(true);
        }
        else if (!focusedObjects.Contains(gameObject) && isReticleHoveringObject)
        {
            isReticleHoveringObject = false;
            if(objectOnTrigger.Count == 0)
            {
                ActivateHighlight(false);
            }
        }
    }

    public void ActivateHighlight(bool isOn)
    {
        if(isHighlighted == isOn || isPermanentlyDisabled) { return; }
        else 
        { 
            isHighlighted = isOn;
            StopAllCoroutines();
            if(isOn)
            {
                StartCoroutine(LerpToHighlightColor(highlightRimColor, highlightAppearCurve, highlightAppearDuration));
            }
            else 
            {
                StartCoroutine(LerpToHighlightColor(Color.black, highlightDisappearCurve, highlightDisappearDuration));
            }
            
        }
    }

    private IEnumerator LerpToHighlightColor(Color col, AnimationCurve curve, float duration)
    {   
        
        if(isHighlighted)
        {
            foreach(Renderer render in allRenderers)
            {
                List<Material> allMats = new List<Material>();
                allMats = allHighlightedMaterialsDict[render].ToList();

                render.materials = allMats.ToArray();
            }
        }
        yield return new WaitForEndOfFrame();

        // My very hacky solution to inconsistent behavior with the outline not appearing on certain objects when reticle is hovered over them, only really apparent with the PushableStatues for whatever reason
        if(reenableOutline && isReticleHoveringObject)
        {
            interactableOutline.enabled = false;
            interactableOutline.enabled = true;
        }

        float timeElapsed = 0;
        targetColor = col;
        while(timeElapsed < duration)
        {
            foreach(Material mat in allHighlightedMaterials)
            {
                if(!isHighlighted) { mat.SetFloat("_PulseSpeed", 0f); }
                else { mat.SetFloat("_PulseSpeed", highlightFlickerSpeed); }

                timeElapsed += Time.deltaTime;
                float curvePercent = curve.Evaluate(Mathf.Clamp01(timeElapsed / duration));
                Color currentColor = Color.LerpUnclamped(mat.GetColor("_RimColor"), targetColor, curvePercent);
                mat.SetColor("_RimColor", currentColor);
                yield return null;
            }
            
        }
        foreach(Material mat in allHighlightedMaterials)
        {
            mat.SetColor("_RimColor", targetColor);
        }

        // reset the material back to normal only if set to false
        if(!isHighlighted)
        { 
            yield return new WaitForEndOfFrame();
            foreach(Renderer render in allRenderers)
            { 
                render.materials = allBaseMaterialsDict[render];              
            }
        }
    }

    // Will turn off the highlight permanently, will be used for single-use interactables like the single-use levers.
    public void TurnOffHighlightPermanently()
    {
        ActivateHighlight(false);
        isPermanentlyDisabled = true;
    }

}
