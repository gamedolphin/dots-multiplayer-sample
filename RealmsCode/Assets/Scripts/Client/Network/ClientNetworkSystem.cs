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
using Unity.Collections;

public struct ServerStateCommand : IComponentData { }

[DisableAutoCreation]
[AlwaysUpdateSystem]
[UpdateAfter(typeof(ClientInputSystem))]
public class ClientNetworkSystem : ComponentSystem, INetEventListener, INetLogger
{
    private NetManager client;
    private NetPeer server;
    private byte[] temp = new byte[5120];    
     
    protected override void OnCreate()
    {
        base.OnCreate();
        NetDebug.Logger = this;
        client = new NetManager(this);
        client.Start();        
    }

    protected override void OnUpdate()
    {        
        HandleConnection();
        HandleInputs();
        client.PollEvents();
    }

    private void HandleConnection()
    {
        Entities.ForEach((Entity entity, ref NetworkSettingsComponent settings) =>
        {
            PostUpdateCommands.DestroyEntity(entity);
            Debug.Log($"Connectin to server {settings.Ip}:{settings.Port}");
            int i = Guid.NewGuid().GetHashCode();
            string name = settings.Name.ToString();
            NetDataWriter writer = new NetDataWriter();
            writer.Put("hashcode");
            writer.Put(i);
            writer.Put(name);
            client.Connect(settings.Ip.ToString(), settings.Port, writer);
            // this is a one time thing
            // otherwise lets avoid accessing systems directly
            var lifecycleSystem = World.GetExistingSystem<PlayerLifecyleSystem>();
            lifecycleSystem.OwnId = i;
        });
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
        Debug.Log("Connected to server!");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        server = null;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.Log("[CLIENT] error " + socketError);
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

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }
}
