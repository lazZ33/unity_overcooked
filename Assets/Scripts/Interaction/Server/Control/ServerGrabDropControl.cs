using System.Collections;
using Unity;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

internal class ServerGrabDropControl: ServerInteractControl {
	[SerializeField] private LayerMask _grabbedGrabbableLayerMask;
	[SerializeField] private LayerMask _interactableLayerMask;


	// shared dependencies to be injected
	private NetworkVariable<ulong> GrabbedClientId;
	private ulong GRABBED_CLIENT_DEFAULT; // static?
	private new IGrabbableSO _info { get { return (IGrabbableSO)base._info; } }


	public event EventHandler<ServerGrabDropEventArgs> OnGrab;
	public event EventHandler<ServerGrabDropEventArgs> OnDrop;


	public bool IsGrabbedByPlayer => this.GrabbedClientId.Value != this.GRABBED_CLIENT_DEFAULT;
	public bool IsGrabbedByLocal => this.GrabbedClientId.Value == NetworkManager.LocalClientId;
	public bool CanPlaceOn(IServerHolder targetHolder) => this._info.CanPlaceOn(targetHolder.Info);


	// builder DI
	internal class GrabDropControlInitArgs: InteractControlInitArgs {
		internal GrabDropControlInitArgs() { }
		internal NetworkVariable<ulong> GrabbedClientId;
		internal ulong GRABBED_CLIENT_DEFAULT;
		internal void AddGrabbedClientId(NetworkVariable<ulong> grabbedClientId) => this.GrabbedClientId = grabbedClientId;
		internal void AddGrabbedClientDefault(ulong grabbedClientDefault) => this.GRABBED_CLIENT_DEFAULT = grabbedClientDefault;
	}
	internal void DepsInit(GrabDropControlInitArgs args) {
		base.DepsInit(args);
		this.GrabbedClientId = args.GrabbedClientId;
		this.GRABBED_CLIENT_DEFAULT = args.GRABBED_CLIENT_DEFAULT;
	}

	protected override void Start() {
		base.Start();

		if (this.GrabbedClientId == null || this.GRABBED_CLIENT_DEFAULT == 0)
			throw new MissingReferenceException(String.Format("grabDrop control not properly initialized before Start(), parent instance: {0}", this._parentInstance));
	}


	internal void OnGrabServerInternal(IServerHolder targetHolder) {
		if (this.IsGrabbedByPlayer)
			return;

		// set layer with LayerMask type: https://discussions.unity.com/t/layer-layermask-which-is-set-in-inspector/179105
		this.gameObject.layer = (int)Mathf.Log(this._grabbedGrabbableLayerMask.value, 2);
		this.GrabbedClientId.Value = targetHolder.OwnerClientId;

		this.OnGrab?.Invoke(this._parentInstance, new ServerGrabDropEventArgs(this._info, targetHolder));
	}

	internal void OnDropServerInternal() {
		// set layer with LayerMask type: https://discussions.unity.com/t/layer-layermask-which-is-set-in-inspector/179105
		this.gameObject.layer = (int)Mathf.Log(this._interactableLayerMask.value, 2);
		this.GrabbedClientId.Value = this.GRABBED_CLIENT_DEFAULT;

		this.OnDrop?.Invoke(this._parentInstance, new ServerGrabDropEventArgs(this._info, null));
	}

}