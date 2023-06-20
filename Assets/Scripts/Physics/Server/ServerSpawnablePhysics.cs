using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ServerSpawnablePhysics : ServerInteractablePhysics{
    [SerializeField] private Rigidbody _rigidbody = null;

    public override void OnNetworkSpawn(){
        if (!this.IsServer) return;
        this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}