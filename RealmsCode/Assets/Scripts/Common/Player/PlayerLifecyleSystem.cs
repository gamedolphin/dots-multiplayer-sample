using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class PlayerLifecyleSystem : ComponentSystem
{
    private Dictionary<int, Entity> players = new Dictionary<int, Entity>();

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref CreatePlayer cp) =>
        {
            var entity = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(entity, new Translation());
            PostUpdateCommands.AddComponent(entity, new PlayerData { ID = cp.Id });
            PostUpdateCommands.DestroyEntity(e);
            players[cp.Id] = entity;
        });

        Entities.ForEach((Entity e, ref DestroyPlayer dp) =>
        {
            PostUpdateCommands.DestroyEntity(e);
            if(players.TryGetValue(dp.Id, out Entity entity))
            {
                PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}