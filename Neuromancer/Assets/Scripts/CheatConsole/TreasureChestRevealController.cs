using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChestController))] 
public class TreasureChestRevealController : MonoBehaviour
{
    public Shader unhideShader;
    
    public Color unhideColor;

    private Material originalMaterial;
    private Material originalLidMaterial;

    private Material customMaterial;
    private Material customLidMaterial;

    private Renderer objectRenderer;
    private Renderer lidRenderer;

    private GameObject keyItem;
    private VisualizationStateManager instance;
    private ChestController chest;
    private Animator chestAnim;
    private GameEventListener eventListener;
    private bool isShaderOn;

    private Material[] originalMaterials;
    private Material[] customMaterials;
    private Renderer[] chestRenderers;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        chestAnim = GetComponent<Animator>();
        eventListener = GetComponent<GameEventListener>();
        foreach(Transform child in transform)
        {
            if(child.GetComponent<Interactable>() && keyItem == null)
            {
                keyItem = child.gameObject;
                continue;
            }
            // Check for the lid of the chest parented to the main gameObject
            if(child.gameObject.name.Contains("Lid"))
            {
                lidRenderer = child.GetComponent<Renderer>();
                continue;
            }
        }

        originalMaterial = objectRenderer.material;
        originalLidMaterial = lidRenderer?.material;

        chestRenderers = new Renderer[] {objectRenderer, lidRenderer};
        originalMaterials = new Material[] {originalMaterial, originalLidMaterial};

        instance = VisualizationStateManager.instance;
        chest = GetComponent<ChestController>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        // creates a new material instance with the custom shader
        customMaterial = new Material(unhideShader);
        customLidMaterial = new Material(unhideShader);

        // Set the enumerator properties in the shader:
        customMaterial.SetTexture("_MainTex", originalMaterial.mainTexture);
        customMaterial.SetColor("_Color", unhideColor);

        customLidMaterial.SetTexture("_MainTex", originalLidMaterial?.mainTexture);
        customLidMaterial.SetColor("_Color", unhideColor);

        customMaterials = new Material[] {customMaterial, customLidMaterial};

        instance.toggleUnhideChestVisualizationEvent.AddListener(ToggleUnhideMaterial);
        
        ToggleUnhideMaterial(instance.UnhideChestVisuals);
        eventListener.UnityEvent.AddListener(ReactivateAnimator);
    }

    private void ToggleUnhideMaterial(bool isEnabled)
    {
        isShaderOn = isEnabled;
        // Debug.Log("toggling the breakable shader to = " + enable);
        switch (isShaderOn)
        {
            case true:
                for(int i = 0; i < chestRenderers.Length; i++)
                {
                    chestRenderers[i].material = customMaterials[i];
                }
                
                if(keyItem != null && !chest.IsOpen)
                {
                    chestAnim.enabled = false;
                    keyItem.SetActive(true);
                }
                break;
            case false:
                for(int i = 0; i < chestRenderers.Length; i++)
                {
                    chestRenderers[i].material = originalMaterials[i];
                }
                
                if(keyItem != null && !chest.IsOpen)
                {
                    chestAnim.enabled = true;
                    keyItem.SetActive(false);
                }
                break;
        }
    }

    // Needs a GameObject parameter for whatever reason
    public void ReactivateAnimator(GameObject gameObject)
    {
        chestAnim.enabled = true;
    }

    private void OnDestroy()
    {
        instance.toggleUnhideChestVisualizationEvent.RemoveListener(ToggleUnhideMaterial);
        eventListener.UnityEvent.AddListener(ReactivateAnimator);
    }
}
