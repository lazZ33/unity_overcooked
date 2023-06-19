using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public abstract class ClientInteractablePhysics: NetworkBehaviour{
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private ClientInteractable _interactableControl;

    void Awake(){
        this._interactableControl.OnInfoChange += this.OnInfoChange;
    }

    public override void OnNetworkSpawn(){
        if (this.IsClient && !this.IsServer) this._meshCollider.enabled = false;
    }

    protected virtual void OnInfoChange(object sender, InfoChangeEventArgs args){
        this._meshCollider.sharedMesh = args.Info.MeshCollider;
    }
}