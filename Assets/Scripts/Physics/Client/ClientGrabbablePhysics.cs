using Unity;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

public class ClientGrabbablePhysics: ClientInteractablePhysics{
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private ClientGrabbable _grabbableControl;

    void Awake(){
        this._grabbableControl.OnInfoChange += this.OnInfoChange;
    }

    public override void OnNetworkSpawn(){
        if (this.IsClient && !this.IsServer) this._meshCollider.enabled = false;
    }
}