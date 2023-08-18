using Unity;
using UnityEngine;
using System;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;
using UseInitArgs = ServerUseControl.UseControlInitArgs;
using System.Diagnostics;

internal class ServerStationaryUtility : ServerInteractable, IServerUsable, IServerHolder
{
	[SerializeField] private ServerHoldTakeControl holdTakeControl;
	[SerializeField] private ServerUseControl useControl;

	// DI variables
	private IServerGrabbable _holdGrabbable = null;


	IUsableSO IServerUsable.Info => (IUsableSO)base._info;
	IHolderSO IServerHolder.Info => (IHolderSO)base._info;
	bool IServerUsable.IsHoldToUse => ((IServerUsable)this.Info).IsHoldToUse;
	bool IServerHolder.IsHoldingGrabbable => this.holdTakeControl.IsHoldingGrabbable;
	IServerGrabbable IServerHolder.HoldGrabbable { get { return this._holdGrabbable; } }


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
	public event EventHandler<ServerUseEventArgs> OnUse;
	public event EventHandler<ServerUseEventArgs> OnUsing;
	public event EventHandler<ServerUseEventArgs> OnUnuse;
	public event EventHandler<ServerUseEventArgs> OnConvert;

	protected override void Awake()
	{
		base.Awake();

		if (this.holdTakeControl == null || this.useControl == null)
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

		// use control DI
		{
			UseInitArgs useInitArgs = new UseInitArgs();
			useInitArgs.AddParentInstance(this);
			useInitArgs.AddGetInfoFunc(() => this.Info);
			useInitArgs.AddGetTargetFunc(GetUseTarget);
			this.useControl.OnUse += (sender, args) => { this.OnUse?.Invoke(sender, args); };
			this.useControl.OnUse += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.useControl.OnUsing += (sender, args) => { this.OnUsing?.Invoke(sender, args); };
			this.useControl.OnUsing += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.useControl.OnUnuse += (sender, args) => { this.OnUnuse?.Invoke(sender, args); };
			this.useControl.OnUnuse += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.useControl.OnConvert += (sender, args) => { this.OnConvert?.Invoke(sender, args); };
			this.useControl.OnConvert += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			useControl.DepsInit(useInitArgs);
		}
	}

	private IServerInteractable GetUseTarget()
	{
		switch (this._holdGrabbable.GetType())
		{
			case IServerHolder targetHolder:
				if (!(targetHolder.Info == ((IHolderSO)this._info).BindingHolder)) return targetHolder;
				if (!targetHolder.IsHoldingGrabbable) return null;
				else return targetHolder.HoldGrabbable;

			case IServerGrabbable holdGrabbable:
				return holdGrabbable;
			default:
				return null;
		}
	}


	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this.holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this.holdTakeControl.OnTakeServerInternal(out takenGrabbable);
	void IServerUsable.OnUseServerInternal() => this.useControl.OnUseServerInternal();
	void IServerUsable.OnUnuseServerInternal() => this.useControl.OnUnuseServerInternal();
}