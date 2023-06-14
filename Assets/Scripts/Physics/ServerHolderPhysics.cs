using Unity;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

public class ServerHolderPhysics: NetworkBehaviour{
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private ServerHolder _grabbableControl;

    // void Awake(){
    //     this._grabbableControl.OnInfoChange += this.OnInfoChange;
    // }

    public override void OnNetworkSpawn(){
        if (!this.IsServer) return;
        this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
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

    // private void OnInfoChange(object sender, ClientGrabbable.InteractionEventArgs args){

    // }
}