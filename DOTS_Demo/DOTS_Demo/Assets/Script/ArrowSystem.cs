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
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ArrowSystem : ISystem
{
    private quaternion arrowRotation;
    [BurstCompile]
    public void OnCreate(ref SystemState systemState)
    {
        arrowRotation = quaternion.Euler(new float3(0f, -math.PI/2f, 0f));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        ArrowMoveJob arrowMoveJob = new ArrowMoveJob {deltaTime=SystemAPI.Time.DeltaTime,arrowRotation=arrowRotation,};
        systemState.Dependency = arrowMoveJob.ScheduleParallel(systemState.Dependency);

        ComponentLookup<ArrowData> arrowDataLookup = SystemAPI.GetComponentLookup<ArrowData>(true);
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        ArrowCollisionJob arrowCollisionJob = new ArrowCollisionJob{arrowDataLookup=arrowDataLookup, entityCommandBuffer=entityCommandBuffer};
        systemState.Dependency = arrowCollisionJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), systemState.Dependency);
        systemState.Dependency.Complete();

        entityCommandBuffer.Playback(systemState.EntityManager);
        entityCommandBuffer.Dispose();


        ComponentLookup<BaseData> baseDataLookup = SystemAPI.GetComponentLookup<BaseData>(true);
        arrowDataLookup=SystemAPI.GetComponentLookup<ArrowData>(true);
        entityCommandBuffer= new EntityCommandBuffer(Allocator.TempJob);

        ArrowTriggerJob arrowTriggerJob = new ArrowTriggerJob{baseDataLookup=baseDataLookup, arrowDataLookup=arrowDataLookup, entityCommandBuffer = entityCommandBuffer};
        systemState.Dependency = arrowTriggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), systemState.Dependency);
        systemState.Dependency.Complete();

        entityCommandBuffer.Playback(systemState.EntityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public partial struct ArrowMoveJob : IJobEntity
    {
        public float deltaTime;
        public quaternion arrowRotation;

        [BurstCompile]
        public void Execute(ref LocalTransform localTransform,ref PhysicsMass physicsMass,ref PhysicsVelocity physicsVelocity,ref ArrowData arrowData)
        {
            physicsVelocity.ApplyLinearImpulse(physicsMass,deltaTime*arrowData.speed*arrowData.defaultDirection);
            physicsVelocity.Linear.z=0f;
            localTransform.Position.z=0f;
            localTransform.Rotation=quaternion.LookRotation(math.normalize(physicsVelocity.Linear),math.up());
            localTransform=localTransform.Rotate(arrowRotation);
        }
    }

    [BurstCompile]
    public partial struct ArrowCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<ArrowData> arrowDataLookup;
        public EntityCommandBuffer entityCommandBuffer;

        [BurstCompile]
        public void Execute(CollisionEvent collisionEvent)
        {
            if(!arrowDataLookup.HasComponent(collisionEvent.EntityA) || !arrowDataLookup.HasComponent(collisionEvent.EntityB))
            {
                return;
            }
            RefRO<ArrowData> arrowDataA = arrowDataLookup.GetRefRO(collisionEvent.EntityA);
            RefRO<ArrowData> arrowDataB = arrowDataLookup.GetRefRO(collisionEvent.EntityB);
            if(arrowDataA.ValueRO.fraction != arrowDataB.ValueRO.fraction)
            {
                entityCommandBuffer.DestroyEntity(collisionEvent.EntityA);
                entityCommandBuffer.DestroyEntity(collisionEvent.EntityB);
            }
        }
    }

    [BurstCompile]
    public partial struct ArrowTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<BaseData> baseDataLookup;
        [ReadOnly] public ComponentLookup<ArrowData> arrowDataLookup;
        public EntityCommandBuffer entityCommandBuffer;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            
            if(baseDataLookup.HasComponent(triggerEvent.EntityA) && arrowDataLookup.HasComponent(triggerEvent.EntityB))
            {
                RefRO<BaseData> baseData = baseDataLookup.GetRefRO(triggerEvent.EntityA);
                RefRO<ArrowData> arrowData = arrowDataLookup.GetRefRO(triggerEvent.EntityB);
                if(baseData.ValueRO.fraction != arrowData.ValueRO.fraction)
                {
                    entityCommandBuffer.SetComponent(triggerEvent.EntityA, new BaseData{fraction=baseData.ValueRO.fraction, maxHealth=baseData.ValueRO.maxHealth, currentHealth=baseData.ValueRO.currentHealth-1});
                    entityCommandBuffer.DestroyEntity(triggerEvent.EntityB);
                }
            }
            else if(baseDataLookup.HasComponent(triggerEvent.EntityB) && arrowDataLookup.HasComponent(triggerEvent.EntityA))
            {
                RefRO<BaseData> baseData = baseDataLookup.GetRefRO(triggerEvent.EntityB);
                RefRO<ArrowData> arrowData = arrowDataLookup.GetRefRO(triggerEvent.EntityA);
                if(baseData.ValueRO.fraction != arrowData.ValueRO.fraction)
                {
                    entityCommandBuffer.SetComponent(triggerEvent.EntityB, new BaseData{fraction=baseData.ValueRO.fraction, maxHealth=baseData.ValueRO.maxHealth, currentHealth=baseData.ValueRO.currentHealth-1});
                    entityCommandBuffer.DestroyEntity(triggerEvent.EntityA);
                }
            }
        }
    }
}
