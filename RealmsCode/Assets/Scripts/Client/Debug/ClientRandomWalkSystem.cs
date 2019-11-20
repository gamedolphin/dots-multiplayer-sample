using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using UnityEngine;

[DisableAutoCreation]
public class ClientRandomWalkSystem : ClientInputSystem
{
    private const float WALK_TIME = 0.2f;
    private float currentTime = 0;
    private float2 currentDirection;

    protected override float2 GetInput()
    {
        if(Time.time - currentTime > WALK_TIME)
        {
            currentTime = Time.time;
            currentDirection = new float2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
        }

        return currentDirection;
    }
}