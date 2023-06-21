using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerGrabbable: ServerInteractable{

    [SerializeField] private LayerMask _grabbedLayerMask;
    [SerializeField] private LayerMask _grabbableLayerMask;
    private new ClientGrabbable _client => (ClientGrabbable)base._client;
    private new GrabbableSO _info { get{ return (GrabbableSO)base._info; } set{ base._info = value; } }
    public new GrabbableSO Info => (GrabbableSO)base._client.Info;
    private NetworkVariable<ulong> _grabbedClientId { get; } = new NetworkVariable<ulong>(ServerGrabbable.GRABBED_CLIENT_DEFAULT, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public ulong GrabbedClientId => this._grabbedClientId.Value;

    public event EventHandler<GrabDropEventArgs> OnGrab;
    public event EventHandler<GrabDropEventArgs> OnDrop;
    public event EventHandler<GrabDropEventArgs> OnTake;
    public event EventHandler<GrabDropEventArgs> OnPlace;

    public bool IsGrabbedByPlayer => this.GrabbedClientId != ServerGrabbable.GRABBED_CLIENT_DEFAULT;
    public bool IsGrabbedByLocal => this.GrabbedClientId == NetworkManager.LocalClientId;
    public bool CanPlaceOn(ServerHolder targetHolder) => this.Info.CanPlaceOn(targetHolder.Info);
    public bool CanPlaceOn(ServerUtensil targetUtensil) => this.Info.CanPlaceOn(targetUtensil.Info);

    internal void OnGrabServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (this.IsGrabbedByPlayer) return;
        print("OnGrabServerInternal");

        this.gameObject.layer = this._grabbedLayerMask;
        this._grabbedClientId.Value = grabbingControl.OwnerClientId;

        this.OnGrab?.Invoke(this, new GrabDropEventArgs(this._info, grabbingControl));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnGrab);
    }
    internal void OnGrabServerInternal(ServerUtensil targetUtensil){
        if (this.IsGrabbedByPlayer) return;
        print("OnGrabServerInternal");

        this.gameObject.layer = this._grabbedLayerMask;
        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;

        this.OnGrab?.Invoke(this, new GrabDropEventArgs(this._info, targetUtensil));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnGrab);
    }

    internal void OnDropServerInternal(){
        print("OnDropServerInternal");

        this.gameObject.layer = this._grabbableLayerMask;
        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;

        this.OnDrop?.Invoke(this, new GrabDropEventArgs(this._info, null));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnDrop);
    }

    internal void OnTakeServerInternal(ServerPlayerGrabbingControl grabbingControl){
        print("OnTakeServer");

        this.OnGrabServerInternal(grabbingControl);

        this.OnTake?.Invoke(this, new GrabDropEventArgs(this._info, null));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnTake);
    }

    internal void OnPlaceToServerInternal(ServerHolder targetHolder){
        print("OnPlaceToServerInternal");

        this.gameObject.layer = this._grabbedLayerMask;
        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;

        this.OnPlace?.Invoke(this, new GrabDropEventArgs(this._info, targetHolder));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnPlace);
    }


    public void Update(){
        // if (!this.IsOwner) return;
        // Debug.Log(string.Format("OwnerId: {0}, LocalId: {1}, GrabbedClientId: {2} InfoStrKey: {3}", this.OwnerClientId, NetworkManager.LocalClientId, this.GrabbedClientId, this._infoStrKey));

        // if (this.GrabbedClientId == NetworkManager.LocalClientId && this._targetTransform != null) {
            // this._rigidbody.MovePosition(this._targetTransform.position);
        // }
    }

}