using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Breakable))]
public class ObstacleShaderController : MonoBehaviour
{
    public Shader breakableShader;
    [SerializeField] [ColorUsage(true, true)]
    public Color neutralColor;
    [SerializeField] [ColorUsage(true, true)]
    public Color bluntColor;
    [SerializeField] [ColorUsage(true, true)]
    public Color sharpColor;

    public enum KeyWordEnum
    {
        Off,
        Blunt,
        Sharp,
        Neutral
    }
    
    
    public KeyWordEnum keywordEnum = KeyWordEnum.Off;   // default value

    private Material originalMaterial;
    private Material customMaterial;
    private Renderer objectRenderer;
    private Interactable interactable;
    private VisualizationStateManager instance;
    private bool isShaderOn;

    private int keywordEnumID;
    
    // Start is called before the first frame update
    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        interactable = GetComponent<Interactable>();
        
        originalMaterial = objectRenderer.material;
        instance = VisualizationStateManager.instance;
    }

    private void Start()
    {
        // creates a new material instance with the custom shader
        customMaterial = new Material(breakableShader);

        // Get the property ID for the enumerator property
        keywordEnumID = Shader.PropertyToID("_Options");

        // Set the enumerator properties in the shader:
        customMaterial.SetTexture("_MainTex", originalMaterial.mainTexture);
        
        if(interactable.group != null)
        {
            InteractableGroup.Type interactCategory = interactable.group.type;
        
            switch(interactCategory)
            {
                default:
                case InteractableGroup.Type.NEUTRAL:
                    customMaterial.SetColor("_EmissionColor", neutralColor);
                    keywordEnum = KeyWordEnum.Neutral;
                break;

                case InteractableGroup.Type.BLUNT:
                    customMaterial.SetColor("_EmissionColor", bluntColor);
                    keywordEnum = KeyWordEnum.Blunt;
                break;

                case InteractableGroup.Type.SHARP:
                    customMaterial.SetColor("_EmissionColor", sharpColor);
                    keywordEnum = KeyWordEnum.Sharp;
                break;                
                
            }
        }
        else
        {
            customMaterial.SetColor("_EmissionColor", neutralColor);
            keywordEnum = KeyWordEnum.Neutral;
        }

        customMaterial.SetInt(keywordEnumID, (int)keywordEnum);
        ToggleCustomMaterial(instance.ShowBreakableVisuals);

        instance.toggleBreakableVisualizationsEvent.AddListener(ToggleCustomMaterial);

        // Apply the custom material to the object
        // if(isShaderOn)
        // {
        //     objectRenderer.material = customMaterial;
        // }
        
    }

    public void ToggleCustomMaterial(bool enable)
    {
        isShaderOn = enable;
        // Debug.Log("toggling the breakable shader to = " + enable);
        switch (isShaderOn)
        {
            case true:
                objectRenderer.material = customMaterial;
                break;
            case false:
                objectRenderer.material = originalMaterial;
                break;
        }
    }

    private void OnDestroy()
    {
        instance.toggleBreakableVisualizationsEvent.RemoveListener(ToggleCustomMaterial);
    }

}
