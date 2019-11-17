using Unity.Entities;
using LiteNetLib;
using UnityEngine;
using LiteNetLib.Utils;

[DisableAutoCreation]
public class ServerNetworkSystem : ComponentSystem
{
    EventBasedNetListener listener = new EventBasedNetListener();
    NetManager server;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        server = new NetManager(listener);
        server.Start(9050 /* port */);


        listener.ConnectionRequestEvent += request =>
        {
            if (server.PeersCount < 10 /* max connections */)
                request.AcceptIfKey("key");
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Debug.Log($"We got connection: {peer.EndPoint}"); // Show peer ip
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
        };
    }

    protected override void OnUpdate()
    {
        server.PollEvents();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        server.Stop();
    }
}
