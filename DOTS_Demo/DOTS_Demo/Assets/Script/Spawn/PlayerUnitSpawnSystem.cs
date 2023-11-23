using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct PlayerUnitSpawnSystem : ISystem
{
    private float spawnInterval;

    [BurstCompile]
    public void OnCreate(ref SystemState systemState)
    {
        systemState.RequireForUpdate<PlayerUnitEntity>();
        systemState.RequireForUpdate<PlayerUnitSpawnData>();

        spawnInterval = 0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        PlayerUnitEntity playerUnitEntity = SystemAPI.GetSingleton<PlayerUnitEntity>();
        PlayerUnitSpawnData playerUnitSpawnData = SystemAPI.GetSingleton<PlayerUnitSpawnData>();
        if(!playerUnitSpawnData.leftMouseButtonDown)
        {
            return;
        }

        //EntityQueryBuilder entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        //entityQueryBuilder.WithDisabled<ArrowData>();
        //entityQueryBuilder.WithOptions(EntityQueryOptions.IncludeDisabledEntities);
        //EntityQuery entityQuery = entityQueryBuilder.Build(ref systemState);
        //NativeArray<Entity> disabledEntities = entityQuery.ToEntityArray(Allocator.Temp);
        //int index = disabledEntities.Length-1;

        spawnInterval += playerUnitSpawnData.spawnRate;
        while(spawnInterval > 1f)
        {
            spawnInterval -= 1f;

            Entity entity = systemState.EntityManager.Instantiate(playerUnitEntity.playerUnitEntity);
            //if(index < 0)
            //{
            //    entity = systemState.EntityManager.Instantiate(playerUnitEntity.playerUnitEntity);
            //}
            //else
            //{
            //    entity = disabledEntities[index];
            //    systemState.EntityManager.SetEnabled(entity, true);
            //    systemState.EntityManager.SetComponentEnabled<ArrowData>(entity, true);
            //    index--;
            //}
            
            LocalTransform localTransform = systemState.EntityManager.GetComponentData<LocalTransform>(entity);
            localTransform.Position = playerUnitSpawnData.mousePosition;
            systemState.EntityManager.SetComponentData(entity, localTransform);
        }

        //disabledEntities.Dispose();
    }
}
