using System;
using Unity;
using UnityEngine;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;

internal class ServerTable: ServerInteractable, IServerHolder {
	[SerializeField] private ServerHoldTakeControl _holdTakeControl;

	// DI variables
	private IServerGrabbable _holdGrabbable = null;


	IHolderSO IServerHolder.Info => (IHolderSO)base._info.Value;
	ulong IServerHolder.OwnerClientId => this.OwnerClientId;
	IServerGrabbable IServerHolder.HoldGrabbable => this._holdGrabbable;
	bool IServerHolder.IsHoldingGrabbable => this._holdTakeControl.IsHoldingGrabbable;


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;


	protected override void Awake() {
		base.Awake();

		if (this._holdTakeControl == null) {
			throw new NullReferenceException("null controller detected");
		}

		// hold take control DI
		{
			HoldTakeInitArgs holdTakeInitArgs = new HoldTakeInitArgs();
			holdTakeInitArgs.AddParentInstance(this);
			holdTakeInitArgs.AddGetInfoFunc(() => this.Info);
			holdTakeInitArgs.AddGetHoldGrabbableFunc(() => this._holdGrabbable);
			holdTakeInitArgs.AddSetHoldGrabbableFunc((IServerGrabbable holdGrabbable) => this._holdGrabbable = holdGrabbable);
			this._holdTakeControl.OnHold += (sender, args) => { this.OnHold?.Invoke(sender, args); };
			this._holdTakeControl.OnHold += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnHold, args.TargetGrabbableInfo.StrKey); };
			this._holdTakeControl.OnTake += (sender, args) => { this.OnTake?.Invoke(sender, args); };
			this._holdTakeControl.OnTake += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnTake, args.TargetGrabbableInfo.StrKey); };
			_holdTakeControl.DepsInit(holdTakeInitArgs);
		}
	}

	public override void OnMapUpdate() {
		this._holdTakeControl.OnMapUpdate();
	}

	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this._holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this._holdTakeControl.OnTakeServerInternal(out takenGrabbable);
}