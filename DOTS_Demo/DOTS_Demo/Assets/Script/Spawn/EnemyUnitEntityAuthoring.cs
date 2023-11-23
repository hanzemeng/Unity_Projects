using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EnemyUnitEntityAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject enemyUnit;

    private class EnemyUnitEntityBaker : Baker<EnemyUnitEntityAuthoring>
    {
        public override void Bake(EnemyUnitEntityAuthoring authoring)
        {
            EnemyUnitEntity enemyUnitEntity = new EnemyUnitEntity();
            enemyUnitEntity.enemyUnitEntity = GetEntity(authoring.enemyUnit, TransformUsageFlags.Dynamic);

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), enemyUnitEntity);
        }
    }
}

public struct EnemyUnitEntity : IComponentData
{
    public Entity enemyUnitEntity;
}
