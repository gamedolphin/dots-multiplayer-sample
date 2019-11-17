using System;
using System.Collections.Generic;
using Unity.Entities;
using Zenject;
using UnityEngine;

public static class WorldTypes
{
    public const string Server = "Server";
    public const string Client = "Client";
}

public enum PlayMode
{
    CLIENT,
    SERVER,
    BOTH
}

[Serializable]
public class WorldSettings
{
    public PlayMode playMode = PlayMode.BOTH;
    public int clientCount;

    public bool HasClient => playMode == PlayMode.BOTH || playMode == PlayMode.CLIENT;
    public bool HasServer => playMode == PlayMode.BOTH || playMode == PlayMode.SERVER;
}


public class WorldManager : IInitializable
{
    [Inject]
    private WorldSettings settings;

    public static World ServerWorld {
        get;
        private set;
    }

    public static World[] ClientWorlds {
        get;
        private set;
    }

    public void Initialize()
    {
        if(settings.HasServer)
        {
            ServerWorld = new World(WorldTypes.Server);
            ServerSystemGroup serverSimulationGroup = ServerWorld.GetOrCreateSystem<ServerSystemGroup>();
            World.Active.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(serverSimulationGroup);
        }

        if (settings.HasClient)
        {
            var count = Mathf.Clamp(settings.clientCount, 1, 10);
            ClientWorlds = new World[count];
            for (int i = 0; i < count; i++)
            {
                var cWorld = new World($"{WorldTypes.Client} {i + 1}");
                ClientSystemGroup clientSimulationGroup = cWorld.GetOrCreateSystem<ClientSystemGroup>();
                World.Active.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(clientSimulationGroup);
                ClientWorlds[i] = cWorld;
            }            
        }            
    }
}