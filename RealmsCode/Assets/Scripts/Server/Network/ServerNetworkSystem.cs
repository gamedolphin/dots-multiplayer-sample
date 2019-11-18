using Unity.Entities;
using LiteNetLib;
using UnityEngine;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using MessagePack;
using Zenject;
using Unity.Transforms;

public struct ServerInputCommand : IComponentData
{
    public int playerID;
    public InputData inputData;
}

[DisableAutoCreation]
public class ServerNetworkSystem : ComponentSystem, INetEventListener
{
    [Inject]
    private WorldSettings settings;

    private NetManager server;
    private readonly byte[] temp = new byte[1024];

    private List<NetPeer> clientList = new List<NetPeer>();
    private EntityQuery stateQuery => GetEntityQuery(typeof(SerializedServerState));

    protected override void OnCreate()
    {
        base.OnCreate();
        server = new NetManager(this);
        server.Start(9050 /* port */);                
    }

    protected override void OnUpdate()
    {
        server.PollEvents();
        HandleServerState();
    }

    private void HandleServerState()
    {
        Entities.With(stateQuery).ForEach((Entity entity) =>
        {
            var data = EntityManager.GetBuffer<SerializedServerState>(entity).Reinterpret<byte>().AsNativeArray();
            for (int i = 0; i < clientList.Count; ++i)
            {
                clientList[i].Send(data.ToArray(), DeliveryMethod.ReliableOrdered);
            }
            PostUpdateCommands.DestroyEntity(entity);
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        server.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.GetHashCode());
        clientList.Add(peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        clientList.RemoveAll(client => client.Id == peer.Id);
        var hashcode = (int)peer.Tag;
        var entity = PostUpdateCommands.CreateEntity();
        PostUpdateCommands.AddComponent(entity, new DestroyPlayer());        
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.Log("[SERVER] error " + socketError);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        var available = reader.AvailableBytes;
        reader.GetBytes(temp, available);
        var inputData = MessagePackSerializer.Deserialize<InputData>(temp);
        // update the simulation 
        var entity = PostUpdateCommands.CreateEntity();
        PostUpdateCommands.AddComponent(entity, new ServerInputCommand
        {
            playerID = (int)peer.Tag,
            inputData = inputData
        });
        reader.Recycle();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        var dataReader = request.Data;
        string key = dataReader.GetString();
        if (key != "hashcode")
            request.Reject();
        var peer = request.Accept();
        int hashcode = dataReader.GetInt();
        string name = dataReader.GetString();
        peer.Tag = hashcode;
        // Add server simulation player
        var entity = PostUpdateCommands.CreateEntity();
        PostUpdateCommands.AddComponent(entity, new CreatePlayer { Id = hashcode });
    }    
}
