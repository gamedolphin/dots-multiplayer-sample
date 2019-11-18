using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct PlayerData : IComponentData
{
    public int Id;
}

public struct CreatePlayer : IComponentData { public int Id; public bool OwnPlayer; }

public struct DestroyPlayer : IComponentData { public int Id;  }

public struct PlayerSpeed : IComponentData
{
    public float Speed;
}

public struct PlayerIndex: IComponentData
{
    public long Index;
}

public struct CurrentPlayer : IComponentData { }