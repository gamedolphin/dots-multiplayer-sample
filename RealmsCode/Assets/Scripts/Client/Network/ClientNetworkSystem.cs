using Unity.Entities;
using LiteNetLib;
using UnityEngine;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClientNetworkSystem : ComponentSystem
{
    EventBasedNetListener listener = new EventBasedNetListener();
    NetManager client;

    protected override void OnCreate()
    {
        base.OnCreate();
        client = new NetManager(listener);
        client.Start();
        client.Connect("localhost", 9050, "key");

        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            Debug.Log("RECEIVED FROM SERVER");
            dataReader.Recycle();
        };
    }

    protected override void OnUpdate()
    {
        client.PollEvents();
    }
     
    protected override void OnDestroy()
    {
        base.OnDestroy();
        client.Stop();
    }
}
