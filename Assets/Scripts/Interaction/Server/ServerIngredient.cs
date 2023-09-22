using System;
using Unity;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

using GrabDropInitArgs = ServerGrabDropControl.GrabDropControlInitArgs;
using CombineInitArgs = ServerCombineControl.CombineControlInitArgs;

internal class ServerIngredient: ServerInteractable, IServerCombinable {
	[SerializeField] private ServerGrabDropControl _grabDropControl;
	[SerializeField] private ServerCombineControl _combineControl;


	// DI variable
	private NetworkVariable<ulong> _grabbedClientId = new NetworkVariable<ulong>(GRABBED_CLIENT_ID_DEFAULT);
	private static readonly ulong GRABBED_CLIENT_ID_DEFAULT = ulong.MaxValue;
	public event EventHandler<ServerGrabDropEventArgs> OnGrab;
	public event EventHandler<ServerGrabDropEventArgs> OnDrop;
	public event EventHandler<CombineEventArgs> OnCombine;


	IGrabbableSO IServerGrabbable.Info => (IGrabbableSO)base._info.Value;
	ICombinableSO IServerCombinable.Info => (ICombinableSO)base._info.Value;
	public bool IsGrabbedByPlayer => this._grabDropControl.IsGrabbedByPlayer;
	public bool IsGrabbedByLocal => this._grabDropControl.IsGrabbedByLocal;
	public bool CanPlaceOn(IServerHolder targetHolder) => this._grabDropControl.CanPlaceOn(targetHolder);
	public bool CanCombineWith(IServerCombinable targetCombinable) => this._combineControl.CanCombineWith(targetCombinable);


	protected override void Awake() {
		base.Awake();

		if (_grabDropControl == null || _combineControl == null) {
			throw new NullReferenceException("null controller detected");
		}

		// grab drop control DI
		{
			GrabDropInitArgs grabDropInitArgs = new GrabDropInitArgs();
			grabDropInitArgs.AddParentInstance(this);
			grabDropInitArgs.AddGetInfoFunc(() => { return this.Info; });
			grabDropInitArgs.AddGrabbedClientId(_grabbedClientId);
			grabDropInitArgs.AddGrabbedClientDefault(GRABBED_CLIENT_ID_DEFAULT);
			this._grabDropControl.OnGrab += (sender, args) => { this.OnGrab?.Invoke(sender, args); };
			this._grabDropControl.OnGrab += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnGrab, args.GrabbableInfo.StrKey); };
			this._grabDropControl.OnDrop += (sender, args) => { this.OnDrop?.Invoke(sender, args); };
			this._grabDropControl.OnDrop += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnDrop, args.GrabbableInfo?.StrKey); };
			this._grabDropControl.DepsInit(grabDropInitArgs);
		}

		// combine control DI
		{
			CombineInitArgs combineInitArgs = new CombineInitArgs();
			combineInitArgs.AddParentInstance(this);
			combineInitArgs.AddGetInfoFunc(() => { return this.Info; });
			this._combineControl.OnCombine += (sender, args) => { this.OnCombine?.Invoke(sender, args); };
			this._combineControl.OnCombine += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnCombine, args.CombinedInfo.StrKey); };
			this._combineControl.DepsInit(combineInitArgs);
		}
	}

	public override void OnMapUpdate() {
		this._grabDropControl.OnMapUpdate();
		this._combineControl.OnMapUpdate();
	}

	void IServerGrabbable.OnGrabServerInternal(IServerHolder targetHolder) => this._grabDropControl.OnGrabServerInternal(targetHolder);
	void IServerGrabbable.OnDropServerInternal() => this._grabDropControl.OnDropServerInternal();
	void IServerCombinable.OnCombineServerInternal() => this._combineControl.OnCombineServerInternal();
}