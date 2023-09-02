using System;
using Unity;
using Unity.Netcode;
using UnityEngine;

using GrabDropInitArgs = ServerGrabDropControl.GrabDropControlInitArgs;
using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;

internal class ServerUtensil : ServerInteractable, IServerGrabbable, IServerHolder
{
	[SerializeField] private ServerGrabDropControl grabDropControl;
	[SerializeField] private ServerHoldTakeControl holdTakeControl;


	// DI variables
	private IServerGrabbable _holdGrabbable = null;
	private NetworkVariable<ulong> _grabbedClientId = new NetworkVariable<ulong>(GRABBED_CLIENT_ID_DEFAULT);
	private static readonly ulong GRABBED_CLIENT_ID_DEFAULT = ulong.MaxValue;


	IHolderSO IServerHolder.Info => (IHolderSO)base._info.Value;
	IGrabbableSO IServerGrabbable.Info => (IGrabbableSO)base._info.Value;
	bool IServerGrabbable.IsGrabbedByPlayer => this.grabDropControl.IsGrabbedByPlayer;
	bool IServerGrabbable.IsGrabbedByLocal => this.grabDropControl.IsGrabbedByPlayer;
	bool IServerGrabbable.CanPlaceOn(IServerHolder targetHolder) => ((IGrabbableSO)base._info.Value).CanPlaceOn(targetHolder.Info);
	ulong IServerHolder.OwnerClientId => this.OwnerClientId;
	bool IServerHolder.IsHoldingGrabbable => this.holdTakeControl.IsHoldingGrabbable;
	IServerGrabbable IServerHolder.HoldGrabbable => this._holdGrabbable;


	public event EventHandler<ServerGrabDropEventArgs> OnGrab;
	public event EventHandler<ServerGrabDropEventArgs> OnDrop;
	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;


	protected override void Awake()
	{
		base.Awake();

		if (this.grabDropControl == null || this.holdTakeControl == null)
		{
			throw new NullReferenceException("null controller detected");
		}

		// grab drop control DI
		{
			GrabDropInitArgs grabDropInitArgs = new GrabDropInitArgs();
			grabDropInitArgs.AddParentInstance(this);
			grabDropInitArgs.AddGetInfoFunc(() => { return this.Info; });
			grabDropInitArgs.AddGrabbedClientId(_grabbedClientId);
			grabDropInitArgs.AddGrabbedClientDefault(GRABBED_CLIENT_ID_DEFAULT);
			this.grabDropControl.OnGrab += (sender, args) => { this.OnGrab?.Invoke(sender, args); };
			this.grabDropControl.OnGrab += (sender, args) =>
			{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnGrab, args.GrabbableInfo.StrKey); };
			this.grabDropControl.OnDrop += (sender, args) => { this.OnDrop?.Invoke(sender, args); };
			this.grabDropControl.OnDrop += (sender, args) =>
			{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnDrop, args.GrabbableInfo.StrKey); };
			this.grabDropControl.DepsInit(grabDropInitArgs);
		}

		// hold take control DI
		{
			HoldTakeInitArgs holdTakeInitArgs = new HoldTakeInitArgs();
			holdTakeInitArgs.AddParentInstance(this);
			holdTakeInitArgs.AddGetInfoFunc(() => this.Info);
			holdTakeInitArgs.AddGetHoldGrabbableFunc(() => this._holdGrabbable);
			holdTakeInitArgs.AddSetHoldGrabbableFunc((IServerGrabbable holdGrabbable) => this._holdGrabbable = holdGrabbable);
			this.holdTakeControl.OnHold += (sender, args) => { this.OnHold?.Invoke(sender, args); };
			this.holdTakeControl.OnHold += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnHold, args.TargetGrabbableInfo.StrKey); };
			this.holdTakeControl.OnTake += (sender, args) => { this.OnTake?.Invoke(sender, args); };
			this.holdTakeControl.OnTake += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnTake, args.TargetGrabbableInfo.StrKey); };
			holdTakeControl.DepsInit(holdTakeInitArgs);
		}
	}


	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this.holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this.holdTakeControl.OnTakeServerInternal(out takenGrabbable);
	void IServerGrabbable.OnGrabServerInternal(IServerHolder targetHolder) => this.grabDropControl.OnGrabServerInternal(targetHolder);
	void IServerGrabbable.OnDropServerInternal() => this.grabDropControl.OnDropServerInternal();
}