using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

internal class ServerGrabDropControl: ServerInteractControl{

    [SerializeField] protected ClientGrabDropControl _client;
    [SerializeField] private ServerInteractableSharedData _data;
    internal static readonly ulong GRABBED_CLIENT_DEFAULT = ulong.MaxValue;
    internal static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";

    public event EventHandler<InteractionEventArgs> OnGrab;
    public event EventHandler<InteractionEventArgs> OnDrop;
    public event EventHandler<InteractionEventArgs> OnTake;
    public event EventHandler<InteractionEventArgs> OnPlace;
    public class InteractionEventArgs: EventArgs{
        internal InteractionEventArgs(InteractableCompositionSO info, object obj){ this.Info = info; this.Object = obj; }
        public InteractableCompositionSO Info;
        public object Object; // for generic use
    }

    public bool IsGrabbedByPlayer => this._data.GrabbedClientId.Value != ServerGrabDropControl.GRABBED_CLIENT_DEFAULT;
    public bool IsGrabbedByLocal => this._data.GrabbedClientId.Value == NetworkManager.LocalClientId;

    internal virtual void SetInfoServerInternal(string infoStrKey){
        print("SetInfoServerInternal");
        this._data.InfoStrKey.Value = infoStrKey;
    }

    internal void OnGrabServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (this.IsGrabbedByPlayer) return;
        print("OnGrabServerInternal");

        this.gameObject.layer = LayerMask.NameToLayer("GrabbedGrabbable");
        this._data.GrabbedClientId.Value = grabbingControl.OwnerClientId;

        this.OnGrab?.Invoke(this, new InteractionEventArgs(this._data.Info, grabbingControl));
        // this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnGrab);
        print("grabbed");
    }

    internal void OnDropServerInternal(){
        print("OnDropServerInternal");

        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        this._data.GrabbedClientId.Value = ServerGrabDropControl.GRABBED_CLIENT_DEFAULT;

        this.OnDrop?.Invoke(this, new InteractionEventArgs(this._data.Info, null));
        // this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnDrop);
        print("Dropped");
    }

    internal void OnPlaceToServerInternal(ServerHolder targetHolder){
        print("OnPlaceToServerInternal");

        this.gameObject.layer = LayerMask.NameToLayer("GrabbedGrabbable");
        this._data.GrabbedClientId.Value = ServerGrabDropControl.GRABBED_CLIENT_DEFAULT;

        this.OnPlace?.Invoke(this, new InteractionEventArgs(this._data.Info, targetHolder));
        // this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnPlace);
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