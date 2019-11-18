using Unity.Entities;
using System.Collections.Generic;
using System;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ServerInitializationSystemGroup : ComponentSystemGroup
{
    private BeginInitializationEntityCommandBufferSystem m_beginBarrier;
    private EndInitializationEntityCommandBufferSystem m_endBarrier;
    protected List<ComponentSystemBase> m_systemsInGroup = new List<ComponentSystemBase>();

    public override IEnumerable<ComponentSystemBase> Systems => m_systemsInGroup;

    protected override void OnCreate()
    {
        m_beginBarrier = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        m_endBarrier = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();

        foreach (var system in World.Active.Systems)
        {
            int depth = 0;
            var type = system.GetType();
            if (SystemUtils.IsInSystem(type, typeof(InitializationSystemGroup), ref depth))
            {
                if (depth > 1)
                {
                    var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);
                    var group = groups[0] as UpdateInGroupAttribute;
                    var groupSys = World.GetOrCreateSystem(group.GroupType) as ComponentSystemGroup;
                    groupSys.AddSystemToUpdateList(World.GetOrCreateSystem(type));
                    AddSystemToUpdateList(groupSys);
                }
                else
                {
                    AddSystemToUpdateList(World.GetOrCreateSystem(system.GetType()));
                }
            }
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

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ServerFixedUpdateGroup : ComponentSystemGroup
{
    protected List<ComponentSystemBase> m_systemsInGroup = new List<ComponentSystemBase>();

    private FixedTimeLoop m_fixedTimeLoop;
    public float UpdateTime => Time.time - m_fixedTimeLoop.accumulatedTime;
    public float UpdateDeltaTime => FixedTimeLoop.fixedTimeStep;

    public override IEnumerable<ComponentSystemBase> Systems => m_systemsInGroup;

    protected override void OnCreate()
    {

        var systemsList = new List<Type>
        {
            typeof(ServerProcessInputSystem)
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
        m_systemsInGroup = new List<ComponentSystemBase>(m_systemsToUpdate.Count);
        m_systemsInGroup.AddRange(m_systemsToUpdate);
    }

    protected override void OnUpdate()
    {
        var defaultWorld = World.Active;
        World.Active = World;
        m_fixedTimeLoop.BeginUpdate();
        while (m_fixedTimeLoop.ShouldUpdate())
        {
            base.OnUpdate();
        }
        World.Active = defaultWorld;
    }
}

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ServerSystemGroup : ComponentSystemGroup
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
            typeof(ServerNetworkSystem),
            typeof(PlayerLifecyleSystem),
            typeof(ServerFixedUpdateGroup),
            typeof(ServerSendStateSystem)
        };

        foreach (var sys in systemsList)
        {
            AddSystemToUpdateList(World.GetOrCreateSystem(sys));

        }

        foreach (var system in World.Active.Systems)
        {
            int depth = 0;
            var type = system.GetType();
            if (SystemUtils.IsInSystem(type, typeof(SimulationSystemGroup), ref depth))
            {
                if (depth > 1)
                {
                    var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);
                    var group = groups[0] as UpdateInGroupAttribute;
                    var groupSys = World.GetOrCreateSystem(group.GroupType) as ComponentSystemGroup;
                    groupSys.AddSystemToUpdateList(World.GetOrCreateSystem(type));
                    AddSystemToUpdateList(groupSys);
                }
                else
                {
                    AddSystemToUpdateList(World.GetOrCreateSystem(system.GetType()));
                }
            }
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