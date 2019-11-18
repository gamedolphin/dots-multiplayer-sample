using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private float playerSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlayerSpeed { Speed = playerSpeed });
        dstManager.AddComponentData(entity, new PlayerIndex { Index = 0 });
    }
}
