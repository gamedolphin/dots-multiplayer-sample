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

    public PlayerState(PlayerState ps) {
        Id = ps.Id;
        Position = ps.Position;
        Index = ps.Index;
    }
}

[MessagePackObject]
public struct WorldState
{
    [Key(0)]
    public PlayerState[] playerState;
}

public struct SerializedServerState : IBufferElementData
{
    public byte Value;
}

[DisableAutoCreation]
public class ServerSendStateSystem : JobComponentSystem
{
    private float updateTime = 0.1f;
    private float oldTime = 0;

    BeginSimulationEntityCommandBufferSystem m_EndFrameBarrier;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_EndFrameBarrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    
    public struct CreatePlayerStateJob : IJobForEachWithEntity<Translation, PlayerData, PlayerIndex>
    {
        public NativeList<PlayerState> arr;

        public void Execute(Entity e, int index, ref Translation transform, 
            ref PlayerData playerData, ref PlayerIndex playerIndex)
        {
            var pos = transform.Value;
            arr.Add(new PlayerState
            {
                Id = playerData.Id,
                Position = new Vector3Sim { x = pos.x, y = pos.y, z = pos.z },
                Index = playerIndex.Index
            });
        }
    }
    
    public struct SerializeStateJob : IJob
    {
        public EntityCommandBuffer CommandBuffer;
        public NativeList<PlayerState> players;

        public void Execute()
        {
            var stateEntity = CommandBuffer.CreateEntity();            
            var worldState = new WorldState
            {
                playerState = players.ToArray()
            };
            var serializedState = MessagePackSerializer.Serialize(worldState);
            var buffer = CommandBuffer.AddBuffer<SerializedServerState>(stateEntity).Reinterpret<byte>();
            var nativearray = new NativeArray<byte>(serializedState, Allocator.Temp);
            buffer.AddRange(nativearray);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (Time.time - oldTime > updateTime)
        {
            oldTime = Time.time;

            var nativeArr = new NativeList<PlayerState>(100, Allocator.TempJob);            

            var playerStateJob = new CreatePlayerStateJob()
            {
                arr = nativeArr

            }.ScheduleSingle(this, inputDeps);
            
            playerStateJob.Complete();            
           
            var serializeJob = new SerializeStateJob()
            {
                CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
                players = nativeArr

            }.Schedule(playerStateJob);

            m_EndFrameBarrier.AddJobHandleForProducer(serializeJob);

            serializeJob.Complete();

            nativeArr.Dispose();

            return serializeJob;
        }
        return inputDeps;
    }
}