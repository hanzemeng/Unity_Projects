using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class BaseDataAuthoring : MonoBehaviour
{
    [SerializeField] private int fraction;
    [SerializeField] private int maxHealth;

    private class BaseDataBaker : Baker<BaseDataAuthoring>
    {
        public override void Bake(BaseDataAuthoring authoring)
        {
            BaseData baseData = new BaseData();
            baseData.fraction = authoring.fraction;
            baseData.maxHealth = authoring.maxHealth;
            baseData.currentHealth = authoring.maxHealth;

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), baseData);
        }
    }
}

public struct BaseData : IComponentData
{
    public int fraction;
    public int maxHealth;
    public int currentHealth;
}
