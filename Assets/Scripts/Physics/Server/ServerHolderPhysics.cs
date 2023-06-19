using Unity;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

public class ServerHolderPhysics: ServerInteractablePhysics{
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private ServerHolder _grabbableControl;

    public override void OnNetworkSpawn(){
        if (!this.IsServer) return;
        this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}