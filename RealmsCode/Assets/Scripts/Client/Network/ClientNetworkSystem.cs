using Unity.Entities;
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

[DisableAutoCreation]
[AlwaysUpdateSystem]
[UpdateAfter(typeof(ClientInputSystem))]
public class ClientNetworkSystem : ComponentSystem, INetEventListener
{
    private NetManager client;
    private NetPeer server;

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

    int ind = 0;
    protected override void OnUpdate()
    {
        client.PollEvents();
        HandleInputs();

        if(ind > 1000)
        {
            client.DisconnectAll();
        }
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
        var entity = PostUpdateCommands.CreateEntity();
        PostUpdateCommands.AddComponent(entity, new CreatePlayer { Id = OwnId });
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        server = null;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        // throw new System.NotImplementedException();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        // throw new System.NotImplementedException();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        // throw new System.NotImplementedException();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        // throw new System.NotImplementedException();
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        // throw new System.NotImplementedException();
    }
}
