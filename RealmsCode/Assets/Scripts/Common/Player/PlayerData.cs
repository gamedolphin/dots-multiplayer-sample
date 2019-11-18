using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct PlayerData : IComponentData
{
    public int ID;
}

public struct CreatePlayer : IComponentData { public int Id; }

public struct DestroyPlayer : IComponentData { public int Id;  }

public struct PlayerPrefab : ISharedComponentData
{
    public Entity Prefab;
}