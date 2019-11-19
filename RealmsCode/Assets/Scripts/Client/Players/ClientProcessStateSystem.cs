using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using MessagePack;

[DisableAutoCreation]
public class ClientProcessStateSystem : ComponentSystem
{
    private EntityQuery bufferQuery => GetEntityQuery(typeof(SerializedServerState));

    protected override void OnUpdate()
    {
        Entities.With(bufferQuery).ForEach((Entity e) =>
        {
            PostUpdateCommands.DestroyEntity(e);
            var buffer = EntityManager.GetBuffer<SerializedServerState>(e).Reinterpret<byte>().AsNativeArray().ToArray();
            var worldState = MessagePackSerializer.Deserialize<WorldState>(buffer);

            worldState.playerState.ForEach(pState =>
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, pState);
            });
        });
    }
}