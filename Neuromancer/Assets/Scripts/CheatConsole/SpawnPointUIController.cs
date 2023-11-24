using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnPointUIController : MonoBehaviour
{
    [Header("General Index Text")]
    private Canvas canvas;
    [SerializeField] private GameObject spawnCenter;
    [SerializeField] private int spawnIndex = 0;
    [SerializeField] private TMP_Text indexText;
    
    public TMP_Text IndexText { get { return indexText;} set { indexText = value;} }

    [Header("Unit Spawner Specific GameObjects")]
    public bool isUnitSpawner = false;
    [SerializeField] private GameObject minDistanceDetectionVisual;
    [SerializeField] private GameObject spawnRegionWidth_plus_MinDistanceVisual;
    [SerializeField] private Color isActiveColor = Color.green;
    [SerializeField] private Color isNotActiveColor = Color.grey;
    [SerializeField] private TMP_Text isActiveTextStatus;
    [SerializeField] private Color isActiveTextColor_On = Color.green;
    [SerializeField] private Color isActiveTextColor_Off = Color.grey;
    private Color minDistanceDetectionBaseColor;
    private SpriteRenderer minDistanceDetectionRenderer;
    private SpriteRenderer spawnRegionWidth_plus_MinDistanceRenderer;
    private ImprovedUnitSpawner assignedUnitSpawner;
    // public ImprovedUnitSpawner AssignedUnitSpawner { get { return assignedUnitSpawner;} set { assignedUnitSpawner = value;}}
    

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        // SetIndexText(spawnIndex);

        // if the spawner is for enemy units specifically, you need the gameObjects representing the minDistance and minDistance + spawnRegionWidth
        if(isUnitSpawner)
        {
            spawnRegionWidth_plus_MinDistanceRenderer = spawnRegionWidth_plus_MinDistanceVisual.GetComponent<SpriteRenderer>();
            minDistanceDetectionRenderer = minDistanceDetectionVisual.GetComponent<SpriteRenderer>();
            minDistanceDetectionBaseColor = minDistanceDetectionRenderer.color;
            spawnRegionWidth_plus_MinDistanceRenderer.color = isNotActiveColor;
            isActiveTextStatus.text = "isActive = OFF";
            isActiveTextStatus.color = isActiveTextColor_Off;
        }
    }

    private void LateUpdate()
    {
        canvas.transform.rotation = Camera.main.transform.rotation;

        // Unit Spawner specific stuff: turns the green visualization on and off based on 
        if(isUnitSpawner && assignedUnitSpawner != null)
        {
            UpdateActiveStatusVisual(assignedUnitSpawner.isActive);
        }
    }

    // For initializing the index numbers
    public void SetIndexText(int index)
    {
        spawnIndex = index;
        indexText.text = spawnIndex.ToString();
    }

    // UNIT SPAWNER SPECIFIC = Will update the color depending on if the current spawner is active or not
    public void UpdateActiveStatusVisual(bool isActive)
    {
        if(isActive)
        {
            minDistanceDetectionRenderer.color = minDistanceDetectionBaseColor;
            spawnRegionWidth_plus_MinDistanceRenderer.color = isActiveColor;
            isActiveTextStatus.text = "isActive = ON";
            isActiveTextStatus.color = isActiveTextColor_On;
        }
        else
        {
            minDistanceDetectionRenderer.color = isNotActiveColor;
            spawnRegionWidth_plus_MinDistanceRenderer.color = isNotActiveColor;
            isActiveTextStatus.text = "isActive = OFF";
            isActiveTextStatus.color = isActiveTextColor_Off;
        }
    }

    public void InitializeUnitSpawnerVisuals(float spawnPointVariance, float minDistance, float spawnRegionWidth, ImprovedUnitSpawner spawner)
    {
        ConvertDistanceToScale(spawnCenter, spawnPointVariance);
        ConvertDistanceToScale(minDistanceDetectionVisual, minDistance);
        ConvertDistanceToScale(spawnRegionWidth_plus_MinDistanceVisual, minDistance + spawnRegionWidth);
        assignedUnitSpawner = spawner;
    }

    // Converts the distance used into scale
    private void ConvertDistanceToScale(GameObject go, float distance)
    {
        Vector3 size = go.transform.lossyScale;
        size.x = distance;
        // Uniform scale for all dimensions.
        size.x = (size.x * go.transform.localScale.x) / go.transform.lossyScale.x;
        size.y = size.x;
        size.z = size.x;
        //size.y = go.transform.localScale.y;

        go.transform.localScale = size;
    }
}
