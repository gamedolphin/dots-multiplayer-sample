using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

// ADD THIS TO A FIXED UPDATE LOOP
[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ServerProcessInputSystem : JobComponentSystem
{
    [BurstCompile]
    struct ServerProcessInputSystemJob : IJobForEachWithEntity<PlayerData>
    {
        [NativeDisableParallelForRestriction]
        public BufferFromEntity<ClientInput> inputBuffers;

        public void Execute(Entity entity, int index, [ReadOnly] ref PlayerData playerData)
        {
            var clientInput = inputBuffers[entity];
            while (clientInput.Length > 0)
            {
                var input = clientInput[0];
                clientInput.RemoveAt(0);
            }      
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var buffer = GetBufferFromEntity<ClientInput>();
        var job = new ServerProcessInputSystemJob()
        {
            inputBuffers = buffer            
        };
       
        var process = job.Schedule(this, inputDependencies);

        return process;
    }
}