using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Zenject;

public class PlayerAuthoring : ConvertToEntity
{
    [Inject]
    private WorldSettings worldSettings;

    private void Awake()
    {
        
    }

    private void ConvertObject()
    {
        if (ConversionMode == Mode.ConvertAndDestroy)
        {
            ConvertHierarchy(gameObject);
        }
        else
        {
            ConvertAndInjectOriginal(gameObject);
        }
    }

    private void Start()
    {
        if (transform.parent != null && transform.parent.GetComponentInParent<ConvertToEntity>() != null)
            return;

        var defaultWorld = World.Active;

        if (worldSettings.HasClient)
        {
            for (int i = 0; i < WorldManager.ClientWorlds.Length; i++)
            {
                var world = WorldManager.ClientWorlds[i];
                World.Active = world;
                ConvertObject();
            }            
        }

        if (worldSettings.HasServer)
        {
            World.Active = WorldManager.ServerWorld;
            ConvertObject();
        }        
        World.Active = defaultWorld;
    }
}