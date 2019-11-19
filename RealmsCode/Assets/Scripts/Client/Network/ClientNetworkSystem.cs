﻿using Unity.Entities;
using LiteNetLib;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using MessagePack;
using System;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using LiteNetLib.Utils;
using Unity.Transforms;
using Unity.Collections;

public struct ServerStateCommand : IComponentData { }

[DisableAutoCreation]
[AlwaysUpdateSystem]
[UpdateAfter(typeof(ClientInputSystem))]
public class ClientNetworkSystem : ComponentSystem, INetEventListener
{
    private NetManager client;
    private NetPeer server;
    private byte[] temp = new byte[1024];
    private int OwnId;
     
    protected override void OnCreate()
    {
        base.OnCreate();
        client = new NetManager(this);
        client.Start();
        int i = Guid.NewGuid().GetHashCode();
        string name = PlayerPrefs.GetString("userName");
        NetDataWriter writer = new NetDataWriter();
        writer.Put("hashcode");
        writer.Put(i);
        writer.Put(name);
        client.Connect("localhost", 9050, writer);
        OwnId = i;
    }

    protected override void OnUpdate()
    {
        client.PollEvents();
        HandleInputs();
    }

    private void HandleInputs()
    {
        Entities.ForEach((Entity entity, ref InputData data) =>
        {
            if (server != null)
            {
                var bytes = MessagePackSerializer.Serialize(data);
                server.Send(bytes, DeliveryMethod.ReliableOrdered);
            }
            PostUpdateCommands.DestroyEntity(entity);
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        client.Stop();
        server = null;
    }

    public void OnPeerConnected(NetPeer peer)
    {
        server = peer;
        var entity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(entity, new CreatePlayer { Id = OwnId, OwnPlayer = true });
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        server = null;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        var available = reader.AvailableBytes;
        reader.GetBytes(temp, available);

        var entity = PostUpdateCommands.CreateEntity();
        PostUpdateCommands.AddComponent(entity, new ServerStateCommand());
        var buffer = PostUpdateCommands.AddBuffer<SerializedServerState>(entity).Reinterpret<byte>();
        var nativearray = new NativeArray<byte>(temp, Allocator.Temp);
        buffer.AddRange(nativearray);
        nativearray.Dispose();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
    }
}
