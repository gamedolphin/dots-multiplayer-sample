# DOTS MULTIPLAYER 
This is an attempt to create a dots based multiplayer game using Unity and to learn how to make things to scale. The eventual goal is to re-create [Realm of the Mad God](https://www.realmofthemadgod.com/), but i am sure you can use this repo to figure out how to do a few of the multiplayer systems. 

# How it works

![Top Level Diagram](https://user-images.githubusercontent.com/7590634/69215448-c154a400-0b8f-11ea-81d3-4e49193ca629.png)

This top level architecture is inspired from the multiplayer sample project inside the not-yet released unity.transport package. But unlike the `ICustomBootstrap` method to initialize the different worlds, we use [Zenject](https://github.com/modesttree/Zenject) based Initialize function to create the server and client worlds. The settings are stored in a scriptable object which is injected into the `WorldManager`. The `ClientWorld` makes a copy of the `InitializationGroup`, `SimulationGroup` and `PresentationGroup` from the `DefaultWorld`. The `PresentationGroup` is added only if you're the "user" and not a bot. You can set `clientCount` to spawn multiple clients. The `ServerWorld` makes a copy of `InitializationGroup` and `SimulationGroup`. Custom worlds cannot call update on their systems so these custom world systems are added back to the `DefaultWorld` systems update cycles. 

## Data Flow Overview
![Data Flow](https://user-images.githubusercontent.com/7590634/69218145-1abfd180-0b96-11ea-8a28-7174056db2e5.png)

[LiteNetLib](https://github.com/RevenantX/LiteNetLib) is used to create the server and client network peers and are completely contained in the `ClientNetworkSystem` and `ServerNetworkSystem`. Communication with and to these systems is done through custom component data objects in their respective worlds. We use [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp) to serialize the data between the worlds. Right now only two objects are exchanged between the clients and server. 
1. The `ClientInputData` which is the per frame info about the user's input + their input tick. This is sent from Client -> Server.
2. The `WorldState` which is the snapshot _per_ client (relative to the player input index). 

# Next Steps
1. Generalize the network data from only input data and world state to general rpc objects. 
2. Send deltas instead of world snapshot.
3. Physics, obstacle detection.
4. Server side NPCs. 
5. Diconnect/Re-connect handling.
