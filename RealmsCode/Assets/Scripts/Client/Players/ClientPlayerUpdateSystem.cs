using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

[DisableAutoCreation]
public class ClientPlayerUpdateSystem : JobComponentSystem
{
    [BurstCompile]
    [RequireComponentTag(typeof(ClientInput),typeof(PlayerData))]
    struct ClientPlayerUpdateSystemJob : IJobForEachWithEntity<
        LatestInputIndex,
        LatestPlayerState,
        PlayerSpeed, 
        Translation>
    {                
        [ReadOnly] public BufferFromEntity<ClientInput> inputBuffers;

        public float deltaTime;


        public void Execute(Entity entity, int index,
            [ReadOnly] ref LatestInputIndex inputData,
            [ReadOnly] ref LatestPlayerState latestState,
            [ReadOnly] ref PlayerSpeed pSpeed,
            ref Translation translation)
        {            
            var inputs = inputBuffers[entity];
            var serverIndex = latestState.pState.Index;

            if(serverIndex < 10)
            {
                // arbitrary lag
                return;
            }

            var inputIndex = inputData.Index;
            var diff = inputIndex - serverIndex;
            float3 direction = float3(0.0f);
            var latestServerArrayIndex = (int)(serverIndex% ClientInputSystem.MAX_BUFFER_COUNT) - 1;
            var pos =  latestState.pState.Position;
            translation.Value = new float3(pos.x, pos.y, pos.z);
            for (int i = 1; i <= diff; i++)
            {
                var inputSlot = (int)((serverIndex + i) % ClientInputSystem.MAX_BUFFER_COUNT);
                var currentInput = inputs[inputSlot].inputData;
                float x = currentInput.Left ? -1 : currentInput.Right ? 1 : 0;
                float y = currentInput.Up ? 1 : currentInput.Down ? -1 : 0;
                direction = new float3(x, 0, y);
                var mov = direction * pSpeed.Speed * deltaTime;
                translation.Value += mov;
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var inputBuffers = GetBufferFromEntity<ClientInput>();
        var job = new ClientPlayerUpdateSystemJob
        {
            inputBuffers = inputBuffers,
            deltaTime = FixedTimeLoop.fixedTimeStep
        };

        return job.Schedule(this, inputDependencies);
    }
}