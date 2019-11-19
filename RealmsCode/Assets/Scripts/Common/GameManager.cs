using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Zenject;

[System.Serializable]
public class NetworkSettings
{
    public string Name;
    public string Ip;
    public int Port;
}

public struct NetworkSettingsComponent : IComponentData
{
    public NativeString64 Name;
    public NativeString64 Ip;
    public int Port;
}

public class GameManager : IInitializable
{
    [Inject]
    private NetworkSettings networkSettings;

    [Inject]
    private WorldManager worldManager;

    [Inject]
    private WorldSettings worldSettings;

    public void Initialize()
    {
        worldManager.Initialize();

        if (worldSettings.HasServer)
        {
            var world = worldManager.ServerWorld;
            var entity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(entity, new NetworkSettingsComponent
            {
                // server doesnot care about ip or name
                Port = networkSettings.Port
            });
        }

        if (worldSettings.HasClient)
        {
            foreach (var world in worldManager.ClientWorlds)
            {
                var entity = world.EntityManager.CreateEntity();
                world.EntityManager.AddComponentData(entity, new NetworkSettingsComponent
                {
                    Name = new NativeString64(networkSettings.Name),
                    Ip = new NativeString64(networkSettings.Ip),
                    Port = networkSettings.Port
                });
            }
        }               
    }
}
