using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

// ADD THIS TO A FIXED UPDATE LOOP
[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ServerProcessInputSystem : JobComponentSystem
{
    [BurstCompile]
    struct ServerProcessInputSystemJob : IJobForEachWithEntity<PlayerData,PlayerSpeed,Translation,PlayerIndex>
    {
        [NativeDisableParallelForRestriction]
        public BufferFromEntity<ClientInput> inputBuffers;

        public float deltaTime;

        public void Execute(Entity entity, int index,
            [ReadOnly] ref PlayerData playerData,
            [ReadOnly] ref PlayerSpeed playerSpeed,
            ref Translation translation,
            ref PlayerIndex pIndex
            )
        {
            var clientInput = inputBuffers[entity];
            while (clientInput.Length > 0)
            {
                var currentInput = clientInput[0].inputData;
                float x = currentInput.Left ? -1 : currentInput.Right ? 1 : 0;
                float y = currentInput.Up ? 1 : currentInput.Down ? -1 : 0;
                var direction = new float3(x, 0, y);
                float delta = playerSpeed.Speed * deltaTime;
                translation.Value += direction * delta;
                pIndex.Index = currentInput.Index;
                clientInput.RemoveAt(0);
            }      
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var buffer = GetBufferFromEntity<ClientInput>();
        var job = new ServerProcessInputSystemJob()
        {
            inputBuffers = buffer,
            deltaTime = FixedTimeLoop.fixedTimeStep
        };
       
        var process = job.Schedule(this, inputDependencies);

        return process;
    }
}