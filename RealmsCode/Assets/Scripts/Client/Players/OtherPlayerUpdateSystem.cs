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
            var delta = targetPos - translation.Value;
            var len2 = dot(delta, delta);
            var step = pSpeed.Speed * deltaTime;
            if(len2 < step * step)
            {
                translation.Value = lerp(translation.Value, targetPos, step);
            }
            else
            {
                var direction = delta / sqrt(len2);
                translation.Value += direction * step;
            }
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