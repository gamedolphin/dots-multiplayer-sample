using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public struct LatestPlayerState : IComponentData
{
    public PlayerState pState;
}

public struct LatestStateIndex : IComponentData {
    public long Index;
};

public struct LatestInputIndex : IComponentData
{
    public long Index;
}

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class PlayerLifecyleSystem : ComponentSystem
{
    private Dictionary<int, Entity> players = new Dictionary<int, Entity>();

    public int OwnId = -1;

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref CreatePlayer cp) =>
        {
            PostUpdateCommands.DestroyEntity(e);
            var entity = GeneratePlayer(cp);         
        });

        Entities.ForEach((Entity e, ref DestroyPlayer dp) =>
        {
            PostUpdateCommands.DestroyEntity(e);
            if (players.TryGetValue(dp.Id, out Entity entity))
            {
                PostUpdateCommands.DestroyEntity(entity);
                players.Remove(dp.Id);
            }
        });

        Entities.ForEach((Entity e, ref ServerInputCommand command) =>
        {
            // this only happens on the server
            PostUpdateCommands.DestroyEntity(e);
            if (players.TryGetValue(command.playerID, out Entity player))
            {
                AddInput(player, command.inputData);
            }
        });

        Entities.ForEach((Entity e, ref PlayerState pState) =>
        {
            // this happens only on the client
            // this is state that came from the server
            var entity = GetOrCreatePlayer(pState.Id, false);
            EntityManager.SetComponentData(entity, new LatestPlayerState { pState = new PlayerState(pState) });
            EntityManager.DestroyEntity(e);
        });        
    }

    private Entity GetOrCreatePlayer(int id, bool isServer)
    {
        if (players.TryGetValue(id, out Entity entity))
        {
            return entity;
        }
        else
        {
            return GeneratePlayer(new CreatePlayer { Id = id, IsServer = isServer, OwnPlayer = id == OwnId });
        }
    }

    private Entity GeneratePlayer (CreatePlayer cp)
    {
        var p = GameObjectConversionUtility.ConvertGameObjectHierarchy(WorldManager.PlayerPrefab,
                World);
        var entity = EntityManager.Instantiate(p);
        EntityManager.AddComponentData(entity, new PlayerData { Id = cp.Id });
        EntityManager.AddComponentData(entity, new PlayerTarget { TargetPos = float3(0.0f) });
        if(cp.IsServer == false)
        {
            EntityManager.AddComponentData(entity, new LatestPlayerState());
        }        
        else
        {
            EntityManager.AddBuffer<ClientInput>(entity);
        }
        EntityManager.SetName(entity, $"Player{cp.Id}");
        players[cp.Id] = entity;
        EntityManager.DestroyEntity(p);
        if (cp.OwnPlayer)
        {
            // add special component for current player
            var buffer = EntityManager.AddBuffer<ClientInput>(entity);
            buffer.AddRange(new NativeArray<ClientInput>(ClientInputSystem.MAX_BUFFER_COUNT, Allocator.Temp));
            EntityManager.AddComponentData(entity, new LatestInputIndex { Index = 0 });
        }
        return entity;
    }

    private void AddInput(Entity player, InputData inputData)
    {
        var buffer = EntityManager.GetBuffer<ClientInput>(player);
        buffer.Add(new ClientInput { inputData = inputData });
    }
}