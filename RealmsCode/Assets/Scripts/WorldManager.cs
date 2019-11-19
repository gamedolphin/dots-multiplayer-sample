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
    public int playerClient = 0;

    public bool HasClient => playMode == PlayMode.BOTH || playMode == PlayMode.CLIENT;
    public bool HasServer => playMode == PlayMode.BOTH || playMode == PlayMode.SERVER;

    public GameObject playerPrefab;
}


public class WorldManager 
{
    [Inject]
    private WorldSettings settings;

    public World ServerWorld {
        get;
        private set;
    }

    public World[] ClientWorlds {
        get;
        private set;
    }
    public static GameObject PlayerPrefab { get; private set; }

    public void Initialize()
    {

        PlayerPrefab = settings.playerPrefab;

        var initializationSystem = World.Active.GetOrCreateSystem<InitializationSystemGroup>();
        var simulationSystem = World.Active.GetOrCreateSystem<SimulationSystemGroup>();
        var presentationSystem = World.Active.GetOrCreateSystem<PresentationSystemGroup>();

        if (settings.HasClient)
        {
            var count = Mathf.Clamp(settings.clientCount, 1, 10);
            ClientWorlds = new World[count];
            for (int i = 0; i < count; i++)
            {
                var cWorld = new World($"{WorldTypes.Client} {i + 1}");

                ClientInitializationSystemGroup clientInitializationSystem = cWorld.GetOrCreateSystem<ClientInitializationSystemGroup>();
                initializationSystem.AddSystemToUpdateList(clientInitializationSystem);

                ClientSystemGroup clientSimulationGroup = cWorld.GetOrCreateSystem<ClientSystemGroup>();
                simulationSystem.AddSystemToUpdateList(clientSimulationGroup);

                ClientWorlds[i] = cWorld;
                if (i != settings.playerClient)
                {
                    var inputSystem = cWorld.GetExistingSystem(typeof(ClientInputSystem));
                    inputSystem.Enabled = false;
                    var randomWalker = cWorld.GetOrCreateSystem(typeof(ClientRandomWalkSystem));
                    clientSimulationGroup.AddSystemToUpdateList(randomWalker);
                    clientSimulationGroup.SortSystemUpdateList();
                }
                else
                {
                    ClientPresentationSystemGroup clientPresentationSystem = cWorld.GetOrCreateSystem<ClientPresentationSystemGroup>();
                    presentationSystem.AddSystemToUpdateList(clientPresentationSystem);
                }
            }
        }

        if (settings.HasServer)
        {
            ServerWorld = new World(WorldTypes.Server);

            ServerInitializationSystemGroup serverInitializationGroup = ServerWorld.GetOrCreateSystem<ServerInitializationSystemGroup>();
            initializationSystem.AddSystemToUpdateList(serverInitializationGroup);

            ServerSystemGroup serverSimulationGroup = ServerWorld.GetOrCreateSystem<ServerSystemGroup>();
            simulationSystem.AddSystemToUpdateList(serverSimulationGroup);
        }


        initializationSystem.SortSystemUpdateList();
        simulationSystem.SortSystemUpdateList();
        presentationSystem.SortSystemUpdateList();
    }
}