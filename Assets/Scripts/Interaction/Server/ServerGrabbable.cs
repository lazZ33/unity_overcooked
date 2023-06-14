using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerGrabbable: ServerInteractable{

    private new ClientGrabbable _client => (ClientGrabbable)base._client;
    private new GrabbableSO _info { get{ return (GrabbableSO)base._info; } set{ base._info = value; } }
    public new GrabbableSO Info => (GrabbableSO)base._client.Info;
    private NetworkVariable<ulong> _grabbedClientId { get; } = new NetworkVariable<ulong>(ServerGrabbable.GRABBED_CLIENT_DEFAULT, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public ulong GrabbedClientId => this._grabbedClientId.Value;

    public event EventHandler<InteractionEventArgs> OnGrab;
    public event EventHandler<InteractionEventArgs> OnDrop;
    public event EventHandler<InteractionEventArgs> OnTake;
    public event EventHandler<InteractionEventArgs> OnPlace;
    public class InteractionEventArgs: EventArgs{
        internal InteractionEventArgs(GrabbableSO info, object obj){ this.Info = info; this.Object = obj; }
        public GrabbableSO Info;
        public object Object; // for generic use
    }

    public bool IsGrabbedByPlayer => this.GrabbedClientId != ServerGrabbable.GRABBED_CLIENT_DEFAULT;
    public bool IsGrabbedByLocal => this.GrabbedClientId == NetworkManager.LocalClientId;
    public bool CanPlaceOn(ServerHolder targetHolder) => this.Info.CanPlaceOn(targetHolder.Info);
    public bool CanPlaceOn(ServerUtensil targetUtensil) => this.Info.CanPlaceOn(targetUtensil.Info);

    internal void OnGrabServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (this.IsGrabbedByPlayer) return;
        print("OnGrabServerInternal");

        this.gameObject.layer = LayerMask.NameToLayer("GrabbedGrabbable");
        this._grabbedClientId.Value = grabbingControl.OwnerClientId;

        this.OnGrab?.Invoke(this, new InteractionEventArgs(this._info, grabbingControl));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnGrab);
        print("grabbed");
    }
    internal void OnGrabServerInternal(ServerUtensil targetUtensil){
        if (this.IsGrabbedByPlayer) return;
        print("OnGrabServerInternal");

        this.gameObject.layer = LayerMask.NameToLayer("GrabbedGrabbable");
        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;

        this.OnGrab?.Invoke(this, new InteractionEventArgs(this._info, targetUtensil));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnGrab);
        print("grabbed");
    }

    internal void OnDropServerInternal(){
        print("OnDropServerInternal");

        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;

        this.OnDrop?.Invoke(this, new InteractionEventArgs(this._info, null));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnDrop);
        print("Dropped");
    }

    internal void OnTakeServerInternal(ServerPlayerGrabbingControl grabbingControl){
        print("OnTakeServer");

        this.OnGrabServerInternal(grabbingControl);

        this.OnTake?.Invoke(this, new InteractionEventArgs(this._info, null));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnTake);
        print("Taken");
    }

    internal void OnPlaceToServerInternal(ServerHolder targetHolder){
        print("OnPlaceToServerInternal");

        this.gameObject.layer = LayerMask.NameToLayer("GrabbedGrabbable");
        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;

        this.OnPlace?.Invoke(this, new InteractionEventArgs(this._info, targetHolder));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnPlace);
        print("Placed");
    }


    public void Update(){
        // if (!this.IsOwner) return;
        // Debug.Log(string.Format("OwnerId: {0}, LocalId: {1}, GrabbedClientId: {2} InfoStrKey: {3}", this.OwnerClientId, NetworkManager.LocalClientId, this.GrabbedClientId, this._infoStrKey));

        // if (this.GrabbedClientId == NetworkManager.LocalClientId && this._targetTransform != null) {
            // this._rigidbody.MovePosition(this._targetTransform.position);
        // }
    }

}