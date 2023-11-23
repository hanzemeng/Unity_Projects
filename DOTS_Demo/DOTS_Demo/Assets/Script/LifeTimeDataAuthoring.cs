using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class LifeTimeDataAuthoring : MonoBehaviour
{
    [SerializeField] private float lifeTime;

    private class LifeTimeDataBaker : Baker<LifeTimeDataAuthoring>
    {
        public override void Bake(LifeTimeDataAuthoring authoring)
        {
            LifeTimeData lifeTimeData = new LifeTimeData();
            lifeTimeData.lifeTime = authoring.lifeTime;

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), lifeTimeData);
        }
    }
}

public struct LifeTimeData : IComponentData
{
    public float lifeTime;
}
