using Unity;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

public class ServerGrabbablePhysics: ServerInteractablePhysics{
    [SerializeField] private Rigidbody _rigidBody;
    private ServerGrabbable _grabbableControl => (ServerGrabbable)base._interactableControl;

    protected override void Awake(){
        base.Awake();
        this._grabbableControl.OnGrab += this.OnGrab;
        this._grabbableControl.OnDrop += this.OnDrop;
        // this._grabbableControl.OnTake += this.OnTake;
        this._grabbableControl.OnPlace += this.OnPlace;
        // this._grabbableControl.OnCombine += this.OnCombine;
    }

    private void OnGrab(object sender, GrabDropEventArgs args){
        ServerPlayerGrabbingControl grabbingControl = (ServerPlayerGrabbingControl)args.Object;
        ServerGrabbable grabbableControl = (ServerGrabbable)sender;

        grabbableControl.NetworkObjectBuf.TrySetParent(grabbingControl.transform, false);
        this.transform.localPosition = grabbingControl.GrabPosition.localPosition;
        this.transform.localRotation = grabbingControl.GrabPosition.localRotation;

        this._rigidBody.useGravity = false;
        this._rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        grabbableControl.NetworkObjectBuf.ChangeOwnership(grabbingControl.OwnerClientId);
    }

    private void OnDrop(object sender, GrabDropEventArgs args){
        ServerGrabbable grabbableControl = (ServerGrabbable)sender;

        this._rigidBody.useGravity = true;
        this._rigidBody.constraints = RigidbodyConstraints.None;
        grabbableControl.NetworkObjectBuf.TryRemoveParent();
        grabbableControl.NetworkObjectBuf.RemoveOwnership();
    }

    // private void OnTake(object sender, ServerGrabbable.InteractionEventArgs args){
    //     this._rigidbody.useGravity = false;
    //     this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    // }

    private void OnPlace(object sender, GrabDropEventArgs args){
        ServerGrabbable grabbableControl = (ServerGrabbable)sender;
        ServerHolder targetHolder = (ServerHolder)args.Object;

        grabbableControl.NetworkObjectBuf.TrySetParent(targetHolder.transform, false);
        this.transform.localPosition = targetHolder.Info.LocalPlacePosition;
        this.transform.localPosition = targetHolder.Info.LocalPlacePosition;

        this._rigidBody.useGravity = false;
        this._rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        grabbableControl.NetworkObjectBuf.RemoveOwnership();
    }

}