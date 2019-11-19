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
    protected override float2 GetInput()
    {
        return new float2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
    }
}