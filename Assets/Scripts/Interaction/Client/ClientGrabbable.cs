using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
public class ClientGrabbable : ClientInteractable{

    public new GrabbableSO Info => (GrabbableSO)base._info;
    private new GrabbableSO _info { get{ return (GrabbableSO)base._info; } set{ base._info = value; } }
    private new ServerGrabbable _server => (ServerGrabbable)base._server;

    public event EventHandler<InteractionEventArgs> OnGrab;
    public event EventHandler<InteractionEventArgs> OnDrop;
    public event EventHandler<InteractionEventArgs> OnTake;
    public event EventHandler<InteractionEventArgs> OnPlace;
    protected event EventHandler<InteractionCallbackExtensionEventArgs> OnInteractionCallbackExtensionHook;
    public class InteractionEventArgs: EventArgs{
        internal InteractionEventArgs(GrabbableSO info){ this.Info = info; }
        public GrabbableSO Info;
    }
    protected class InteractionCallbackExtensionEventArgs: EventArgs{
        internal InteractionCallbackExtensionEventArgs(InteractionEventArgs args){ this.Args = args; }
        internal InteractionCallbackID id;
        internal InteractionEventArgs Args;
    }

    public bool IsGrabbedByPlayer => this._server.IsGrabbedByPlayer;
    public bool IsGrabbedByLocal => this._server.IsGrabbedByLocal;

    [ClientRpc]
    internal void InteractionCallbackClientRpc(InteractionCallbackID id){
        InteractionEventArgs args = new InteractionEventArgs(this._info);
        switch (id){
            case InteractionCallbackID.OnGrab:
                this.OnGrab?.Invoke(this, args);
                break;
            case InteractionCallbackID.OnDrop:
                this.OnDrop?.Invoke(this, args);
                break;
            case InteractionCallbackID.OnTake:
                this.OnTake?.Invoke(this, args);
                break;
            case InteractionCallbackID.OnPlace:
                this.OnPlace?.Invoke(this, args);
                break;
            default:
                this.OnInteractionCallbackExtensionHook?.Invoke(this, new InteractionCallbackExtensionEventArgs(args));
                break;
        }
    }

    public void Update(){
        // if (!this.IsOwner) return;
        // // Debug.Log(string.Format("OwnerId: {0}, LocalId: {1}, GrabbedClientId: {2} InfoStrKey: {3}", this.OwnerClientId, NetworkManager.LocalClientId, this.GrabbedClientId, this._infoStrKey));

        // if (this.GrabbedClientId == NetworkManager.LocalClientId && this._targetTransform != null) {
            // this._rigidbody.MovePosition(this._targetTransform.position);
        // }
    }

}