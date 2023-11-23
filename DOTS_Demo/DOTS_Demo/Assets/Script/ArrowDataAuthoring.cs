using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class ArrowDataAuthoring : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int fraction;
    [SerializeField] private Vector3 defaultDirection;


    private class ArrowDataBaker : Baker<ArrowDataAuthoring>
    {
        public override void Bake(ArrowDataAuthoring authoring)
        {
            ArrowData arrowData = new ArrowData();
            arrowData.speed = authoring.speed;
            arrowData.fraction = authoring.fraction;
            arrowData.defaultDirection = authoring.defaultDirection;

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), arrowData);
        }
    }
}

public struct ArrowData : IComponentData
{
    public float speed;
    public int fraction;
    public float3 defaultDirection;
}
