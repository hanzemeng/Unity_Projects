using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct PlayerUnitSpawnData : IComponentData
{
    public bool leftMouseButtonDown;
    public float3 mousePosition;

    public float spawnRate; // units per still frame
}
