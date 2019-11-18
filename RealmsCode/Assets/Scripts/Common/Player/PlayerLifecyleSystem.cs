﻿using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class PlayerLifecyleSystem : ComponentSystem
{
    private Dictionary<int, Entity> players = new Dictionary<int, Entity>();

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref CreatePlayer cp) =>
        {
            PostUpdateCommands.DestroyEntity(e);
            var p = GameObjectConversionUtility.ConvertGameObjectHierarchy(WorldManager.PlayerPrefab,
                World);
            var entity = EntityManager.Instantiate(p);
            EntityManager.AddComponentData(entity, new PlayerData { Id = cp.Id });
            EntityManager.AddBuffer<ClientInput>(entity);
            EntityManager.SetName(entity, $"Player{cp.Id}");

            if(cp.OwnPlayer)
            {
                // add special component for current player
                EntityManager.AddComponent<CurrentPlayer>(entity);
            }

            players[cp.Id] = entity;
            EntityManager.DestroyEntity(p);
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
    }

    private void AddInput(Entity player, InputData inputData)
    {
        var buffer = EntityManager.GetBuffer<ClientInput>(player);
        buffer.Add(new ClientInput { inputData = inputData });
    }
}