using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerUnitEntityAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject playerUnit;

    private class PlayerUnitEntityBaker : Baker<PlayerUnitEntityAuthoring>
    {
        public override void Bake(PlayerUnitEntityAuthoring authoring)
        {
            PlayerUnitEntity playerUnitEntity = new PlayerUnitEntity();
            playerUnitEntity.playerUnitEntity = GetEntity(authoring.playerUnit, TransformUsageFlags.Dynamic);

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), playerUnitEntity);
        }
    }
}

public struct PlayerUnitEntity : IComponentData
{
    public Entity playerUnitEntity;
}
