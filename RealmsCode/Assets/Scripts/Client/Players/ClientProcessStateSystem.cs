using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using MessagePack;
using Unity.Collections;

[DisableAutoCreation]
public class ClientProcessStateSystem : JobComponentSystem
{    
    BeginSimulationEntityCommandBufferSystem m_StartFrameBarrier;
    EndSimulationEntityCommandBufferSystem m_EndFrameBarrier;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_StartFrameBarrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_EndFrameBarrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [RequireComponentTag(typeof(SerializedServerState))]
    public struct ClientProcessStateJob : IJobForEachWithEntity<ServerStateCommand>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public EntityCommandBuffer.Concurrent EndCommandBuffer;
        
        [ReadOnly] public BufferFromEntity<SerializedServerState> ServerStates;

        public void Execute(Entity entity, int index, [ReadOnly] ref ServerStateCommand c0)
        {
            EndCommandBuffer.DestroyEntity(index, entity);
            var buffer = ServerStates[entity].Reinterpret<byte>().AsNativeArray().ToArray();
            var worldState = MessagePackSerializer.Deserialize<WorldState>(buffer);
            foreach(var pState in worldState.playerState)
            {
                var e = CommandBuffer.CreateEntity(index);
                CommandBuffer.AddComponent(index, e, pState);
            }            
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var lookup = GetBufferFromEntity<SerializedServerState>();
        var job = new ClientProcessStateJob()
        {
            ServerStates = lookup,
            CommandBuffer = m_StartFrameBarrier.CreateCommandBuffer().ToConcurrent(),
            EndCommandBuffer = m_EndFrameBarrier.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);
        m_StartFrameBarrier.AddJobHandleForProducer(job);
        m_EndFrameBarrier.AddJobHandleForProducer(job);
        return job;
    }
}