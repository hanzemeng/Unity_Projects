using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyUnitSpawnData : IComponentData
{
    public float3 spawnLocationBottomLeftPoint;
    public float3 spawnLocationTopRightPoint;
    public float spawnRate; // units per still frame
}
