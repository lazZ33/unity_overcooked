using Unity;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

public class ClientGrabbablePhysics: NetworkBehaviour{
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private ClientGrabbable _grabbableControl;

    void Awake(){
        this._grabbableControl.OnInfoChange += this.OnInfoChange;
    }

    public override void OnNetworkSpawn(){
        if (this.IsClient && !this.IsServer) this._meshCollider.enabled = false;
    }
    
    // private void OnGrab(object sender, ClientGrabbable.InteractionEventArgs args){

    // }

    // private void OnDrop(object sender, ClientGrabbable.InteractionEventArgs args){

    // }

    // private void OnTake(object sender, ClientGrabbable.InteractionEventArgs args){

    // }

    // private void OnPlace(object sender, ClientGrabbable.InteractionEventArgs args){
        
    // }

    // private void OnCombine(object sender, ClientGrabbable.InteractionEventArgs args){

    // }

    private void OnInfoChange(object sender, ClientGrabbable.InteractionEventArgs args){
        this._meshCollider.sharedMesh = args.Info.MeshCollider;
    }
}