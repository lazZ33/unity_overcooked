using System.Collections;
using Unity;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

internal class ServerGrabDropControl : ServerInteractControl
{
    [SerializeField] private LayerMask _grabbedGrabbableLayerMask;
    [SerializeField] private LayerMask _interactableLayerMask;


    // shared dependencies to be injected
    private NetworkVariable<ulong> GrabbedClientId;
    private ulong GRABBED_CLIENT_DEFAULT; // static?
    private new IGrabbableSO _info { get { return (IGrabbableSO)base._info; } }


    public event EventHandler<GrabDropEventArgs> OnGrab;
    public event EventHandler<GrabDropEventArgs> OnDrop;


	public bool IsGrabbedByPlayer => this.GrabbedClientId.Value != this.GRABBED_CLIENT_DEFAULT;
    public bool IsGrabbedByLocal => this.GrabbedClientId.Value == NetworkManager.LocalClientId;
    public bool CanPlaceOn(IServerHolder targetHolder) => this._info.CanPlaceOn(targetHolder.Info);


    // builder DI
    internal class GrabDropControlInitArgs: InteractControlInitArgs
    {
        internal GrabDropControlInitArgs() { }
        internal NetworkVariable<ulong> GrabbedClientId;
        internal ulong GRABBED_CLIENT_DEFAULT;
        internal void AddGrabbedClientId(NetworkVariable<ulong> grabbedClientId) => this.GrabbedClientId = grabbedClientId;
        internal void AddGrabbedClientDefault(ulong grabbedClientDefault) => this.GRABBED_CLIENT_DEFAULT = grabbedClientDefault;
    }
	internal void DepsInit(GrabDropControlInitArgs args)
	{
		base.DepsInit(args);
        this.GrabbedClientId = args.GrabbedClientId;
        this.GRABBED_CLIENT_DEFAULT = args.GRABBED_CLIENT_DEFAULT;
	}


	internal void OnGrabServerInternal(IServerHolder targetHolder){
        if (this.IsGrabbedByPlayer) return;
        print("OnGrabServerInternal");

        this.gameObject.layer = this._grabbedGrabbableLayerMask;
        this.GrabbedClientId.Value = targetHolder.OwnerClientId;

        this.OnGrab?.Invoke(this, new GrabDropEventArgs(this._info, targetHolder));
        // this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnGrab);
        print("grabbed");
    }

    internal void OnDropServerInternal(){
        print("OnDropServerInternal");

        this.gameObject.layer = this._interactableLayerMask;
        this.GrabbedClientId.Value = this.GRABBED_CLIENT_DEFAULT;

        this.OnDrop?.Invoke(this, new GrabDropEventArgs(this._info, null));
        // this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnDrop);
        print("Dropped");
    }

}