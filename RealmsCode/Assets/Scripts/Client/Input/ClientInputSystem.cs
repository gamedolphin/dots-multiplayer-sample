using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using Unity.Mathematics;


// Client Input Buffer is added by the world manager. 
// This restricts to single input player on each client
[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClientInputSystem : ComponentSystem
{
    public const int MAX_BUFFER_COUNT = 512;

    private long index = 0;

    private EntityQuery Eq => GetEntityQuery(typeof(ClientInput),typeof(LatestInputIndex));

    protected virtual float2 GetInput()
    {
        return new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    protected override void OnUpdate()
    {        
        Entities.With(Eq).ForEach((entity) =>
        {
            var inputData = GetInput();
            var horizontal = inputData.x;
            var vertical = inputData.y;
            var input = new InputData
            {
                Right = horizontal > 0,
                Left = horizontal < 0,
                Up = vertical > 0,
                Down = vertical < 0,
                Index = index
            };
            var inputBuffer = EntityManager.GetBuffer<ClientInput>(entity);
            int bufferSlot = (int)(index % MAX_BUFFER_COUNT);
            var clientInput = new ClientInput { inputData = input };
            if (inputBuffer.Length < MAX_BUFFER_COUNT)
            {
                inputBuffer.Add(clientInput);
            }
            else
            {
                inputBuffer[bufferSlot] = clientInput;
            }            
            var networkData = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(networkData, input);            

            EntityManager.SetComponentData(entity, new LatestInputIndex { Index = index });
        });        
        index++;
    }
}
