using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[DisableAutoCreation]
public class OtherPlayerUpdateSystem : JobComponentSystem
{

    [BurstCompile]
    [ExcludeComponent(typeof(ClientInput))]
    struct OtherPlayerUpdateSystemJob : IJobForEachWithEntity<LatestPlayerState,PlayerSpeed,Translation>
    {
        public float deltaTime;

        public void Execute(Entity entity, int index,
            [ReadOnly] ref LatestPlayerState latestState,
            [ReadOnly] ref PlayerSpeed pSpeed,
            ref Translation translation)
        {
            var pos = latestState.pState.Position;
            var targetPos = float3(pos.x, pos.y, pos.z);

            translation.Value = lerp(translation.Value, targetPos, pSpeed.Speed*deltaTime);
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {       
        var job = new OtherPlayerUpdateSystemJob
        {
            deltaTime = FixedTimeLoop.fixedTimeStep
        };

        return job.Schedule(this, inputDependencies);
    }
}