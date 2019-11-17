using Unity.Entities;
using System.Collections.Generic;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClientSystemGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        AddSystemToUpdateList(World.GetOrCreateSystem<ClientNetworkSystem>());
    }
}