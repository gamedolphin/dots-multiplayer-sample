using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using MessagePack;
using System;

[MessagePackObject]
public struct Vector3Sim
{
    [Key(0)]
    public float x;
    [Key(1)]
    public float y;
    [Key(2)]
    public float z;
}

[MessagePackObject]
public struct PlayerState : IComponentData
{
    [Key(0)]
    public int Id;
    [Key(1)]
    public Vector3Sim Position;
    [Key(2)]
    public long Index;
}

[MessagePackObject]
public struct WorldState
{
    [Key(0)]
    public List<PlayerState> playerState;
}

public struct SerializedServerState : IBufferElementData
{
    public byte Value;
}

[DisableAutoCreation]
public class ServerSendStateSystem : ComponentSystem
{
    private float updateTime = 0.2f;
    private float oldTime = 0;
    private WorldState worldState = new WorldState
    {
        playerState = new List<PlayerState>()
    };
    private int index = 0;

    protected override void OnUpdate()
    {
        if (Time.time - oldTime > updateTime)
        {
            oldTime = Time.time;
            worldState.playerState.Clear();
            Entities.ForEach((ref Translation transform, 
                ref PlayerData playerData, 
                ref PlayerIndex playerIndex) =>
            {
                var pos = transform.Value;
                worldState.playerState.Add(new PlayerState
                {
                    Id = playerData.Id,
                    Position = new Vector3Sim { x = pos.x, y = pos.y, z = pos.z },
                    Index = playerIndex.Index
                });

            });

            var stateEntity = EntityManager.CreateEntity();
            var buffer = EntityManager.AddBuffer<SerializedServerState>(stateEntity).Reinterpret<byte>();
            var serializedState = MessagePackSerializer.Serialize(worldState);
            var nativearray = new NativeArray<byte>(serializedState, Allocator.Temp);
            buffer.AddRange(nativearray);
        }
    }
}