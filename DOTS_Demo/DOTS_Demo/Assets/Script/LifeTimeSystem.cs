using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Physics.Extensions;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(ArrowSystem))]
public partial struct LifeTimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        LifeTimeExpireJob lifeTimeExpireJob = new LifeTimeExpireJob {deltaTime=SystemAPI.Time.DeltaTime, paralleElentityCommandBuffer=entityCommandBuffer.AsParallelWriter()};
        systemState.Dependency = lifeTimeExpireJob.ScheduleParallel(systemState.Dependency);
        systemState.Dependency.Complete();

        entityCommandBuffer.Playback(systemState.EntityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public partial struct LifeTimeExpireJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter paralleElentityCommandBuffer;
        public float deltaTime;

        [BurstCompile]
        public void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, ref LifeTimeData lifeTimeData)
        {
            lifeTimeData.lifeTime -= deltaTime;
            if(lifeTimeData.lifeTime < 0f)
            {
                paralleElentityCommandBuffer.DestroyEntity(sortKey, entity);
            }
        }
    }
}
