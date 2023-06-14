using Unity;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

public class ServerGrabbablePhysics: MonoBehaviour{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private ServerGrabbable _grabbableControl;

    void Awake(){
        this._grabbableControl.OnGrab += this.OnGrab;
        this._grabbableControl.OnDrop += this.OnDrop;
        // this._grabbableControl.OnTake += this.OnTake;
        this._grabbableControl.OnPlace += this.OnPlace;
        // this._grabbableControl.OnCombine += this.OnCombine;
    }
    
    private void OnGrab(object sender, ServerGrabbable.InteractionEventArgs args){
        ServerPlayerGrabbingControl grabbingControl = (ServerPlayerGrabbingControl)args.Object;
        ServerGrabbable grabbableControl = (ServerGrabbable)sender;

        grabbableControl.NetworkObjectBuf.TrySetParent(grabbingControl.transform, false);
        this.transform.localPosition = grabbingControl.GrabPosition.localPosition;
        this.transform.localRotation = grabbingControl.GrabPosition.localRotation;
        this._rigidBody.useGravity = false;
        this._rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        grabbableControl.NetworkObjectBuf.ChangeOwnership(grabbingControl.OwnerClientId);
    }

    private void OnDrop(object sender, ServerGrabbable.InteractionEventArgs args){
        ServerGrabbable grabbableControl = (ServerGrabbable)sender;

        grabbableControl.NetworkObjectBuf.RemoveOwnership();
        grabbableControl.NetworkObjectBuf.TryRemoveParent();
        this._rigidBody.useGravity = true;
        this._rigidBody.constraints = RigidbodyConstraints.None;
    }

    // private void OnTake(object sender, ServerGrabbable.InteractionEventArgs args){
    //     this._rigidbody.useGravity = false;
    //     this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    // }

    private void OnPlace(object sender, ServerGrabbable.InteractionEventArgs args){
        ServerGrabbable grabbableControl = (ServerGrabbable)sender;
        ServerHolder targetHolder = (ServerHolder)args.Object;

        grabbableControl.NetworkObjectBuf.RemoveOwnership();
        this._rigidBody.useGravity = false;
        this._rigidBody.constraints = RigidbodyConstraints.FreezeAll;

        grabbableControl.NetworkObjectBuf.TrySetParent(targetHolder.transform, false);
        this.transform.localPosition = targetHolder.Info.LocalPlacePoint;
        this.transform.localPosition = targetHolder.Info.LocalPlacePoint;
    }

    // private void OnCombine(object targetGrabbable, ServerGrabbable.InteractionEventArgs args){

    // }
}