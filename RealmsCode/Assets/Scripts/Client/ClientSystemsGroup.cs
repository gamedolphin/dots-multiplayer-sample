using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Transforms;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClientSystemGroup : ComponentSystemGroup
{
    private BeginSimulationEntityCommandBufferSystem m_beginBarrier;
    private EndSimulationEntityCommandBufferSystem m_endBarrier;

    protected List<ComponentSystemBase> m_systemsInGroup = new List<ComponentSystemBase>();

    public override IEnumerable<ComponentSystemBase> Systems => m_systemsInGroup;

    protected override void OnCreate()
    {
        m_beginBarrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_endBarrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        var systemsList = new List<Type>
        {
            typeof(ClientNetworkSystem),
            typeof(PlayerLifecyleSystem),
            typeof(TransformSystemGroup),
            typeof(LateSimulationSystemGroup),
            typeof(ClientInputSystem)
        };

        foreach (var sys in systemsList)
        {
            AddSystemToUpdateList(World.GetOrCreateSystem(sys));

        }
        SortSystemUpdateList();        
    }

    public override void SortSystemUpdateList()
    {
        base.SortSystemUpdateList();
        m_systemsInGroup = new List<ComponentSystemBase>(1 + m_systemsToUpdate.Count + 1);
        m_systemsInGroup.Add(m_beginBarrier);
        m_systemsInGroup.AddRange(m_systemsToUpdate);
        m_systemsInGroup.Add(m_endBarrier);
    }

    protected override void OnUpdate()
    {
        var defaultWorld = World.Active;
        World.Active = World;
        m_beginBarrier.Update();
        base.OnUpdate();
        m_endBarrier.Update();
        World.Active = defaultWorld;
    }
}