﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


// Client Input Buffer is added by the world manager. 
// This restricts to single input player on each client
[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClientInputSystem : ComponentSystem
{
    public const int MAX_BUFFER_COUNT = 512;

    private long index = 0;

    protected override void OnUpdate()
    {
        var eq = GetEntityQuery(typeof(ClientInput));
        Entities.With(eq).ForEach((entity) =>
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            var interact = Input.GetKeyDown(KeyCode.F);
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
            var networkData = EntityManager.CreateEntity();
            EntityManager.AddComponentData(networkData, input);
        });        
        index++;
    }
}