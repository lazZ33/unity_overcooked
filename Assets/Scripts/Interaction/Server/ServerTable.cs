using System;
using Unity;
using UnityEngine;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;

internal class ServerTable : ServerInteractable, IServerHolder
{
	[SerializeField] private ServerHoldTakeControl holdTakeControl;

	// DI variables
	private IServerGrabbable _holdGrabbable = null;


	IHolderSO IServerHolder.Info => (IHolderSO)base._info;
	ulong IServerHolder.OwnerClientId => this.OwnerClientId;
	IServerGrabbable IServerHolder.HoldGrabbable => this._holdGrabbable;
	bool IServerHolder.IsHoldingGrabbable => this.holdTakeControl.IsHoldingGrabbable;


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;


	protected override void Awake()
	{
		base.Awake();

		if (this.holdTakeControl == null)
		{
			throw new NullReferenceException("null controller detected");
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
}