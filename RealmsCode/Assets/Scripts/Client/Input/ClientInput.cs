using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

[InternalBufferCapacity(ClientInputSystem.MAX_BUFFER_COUNT)]
public struct ClientInput : IBufferElementData
{
    public InputData inputData;
}
