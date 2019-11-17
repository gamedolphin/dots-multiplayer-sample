using System;
using System.Collections.Generic;
using Unity.Entities;
using Zenject;

public static class WorldTypes
{
    public const string Server = "Server";
    public const string Client = "Client";
}


public class WorldManager : IInitializable
{
    public static World ServerWorld {
        get;
        private set;
    }

    public static World ClientWorld {
        get;
        private set;
    }

    public void Initialize()
    {
        ServerWorld = new World(WorldTypes.Server);
        ServerSystemGroup serverSimulationGroup = ServerWorld.GetOrCreateSystem<ServerSystemGroup>();

        World.Active.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(serverSimulationGroup);

        ClientWorld = new World(WorldTypes.Client);
        ClientSystemGroup clientSimulationGroup = ClientWorld.GetOrCreateSystem<ClientSystemGroup>();
        World.Active.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(clientSimulationGroup);
    }
}