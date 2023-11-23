using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct EnemyUnitSpawnSystem : ISystem
{
    private Unity.Mathematics.Random random;
    private float spawnInterval;

    [BurstCompile]
    public void OnCreate(ref SystemState systemState)
    {
        systemState.RequireForUpdate<EnemyUnitEntity>();
        systemState.RequireForUpdate<EnemyUnitSpawnData>();
        
        random = new Unity.Mathematics.Random(123456);
        spawnInterval = 0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        EnemyUnitEntity enemyUnitEntity = SystemAPI.GetSingleton<EnemyUnitEntity>();
        EnemyUnitSpawnData enemyUnitSpawnData = SystemAPI.GetSingleton<EnemyUnitSpawnData>();

        spawnInterval += enemyUnitSpawnData.spawnRate;
        while(spawnInterval > 1f)
        {
            spawnInterval -= 1f;

            Entity entity = systemState.EntityManager.Instantiate(enemyUnitEntity.enemyUnitEntity);
            LocalTransform localTransform = systemState.EntityManager.GetComponentData<LocalTransform>(entity);
            localTransform.Position = random.NextFloat3(enemyUnitSpawnData.spawnLocationBottomLeftPoint, enemyUnitSpawnData.spawnLocationTopRightPoint);
            systemState.EntityManager.SetComponentData(entity, localTransform);
        }
    }
}
