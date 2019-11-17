using Unity.Entities;
using System.Collections.Generic;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ServerSystemGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {         
        AddSystemToUpdateList(World.GetOrCreateSystem<ServerNetworkSystem>());
    }
}