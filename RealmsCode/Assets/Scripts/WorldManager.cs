using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Unity.Entities;

public class WorldManager :  IInitializable  {

    public World serverWorld;
    public World clientWorld;

    public void Initialize() {
        Debug.Log("CREATING WORLDS");
        serverWorld = new World("Server");
        clientWorld = new World("Client");
    }
}
