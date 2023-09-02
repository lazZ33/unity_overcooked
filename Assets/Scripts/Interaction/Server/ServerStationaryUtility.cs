using Unity;
using UnityEngine;
using System;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;
using UseInitArgs = ServerConvertControl.UseControlInitArgs;

internal class ServerStationaryUtility : ServerInteractable, IServerConverter, IServerHolder
{
	[SerializeField] private ServerHoldTakeControl holdTakeControl;
	[SerializeField] private ServerConvertControl convertControl;

	// DI variables
	private IServerGrabbable _holdGrabbable = null;


	IConverterSO IServerConverter.Info => (IConverterSO)base._info.Value;
	IHolderSO IServerHolder.Info => (IHolderSO)base._info.Value;
	bool IServerConverter.IsHoldToConvert => ((IConverterSO)this.Info).IsHoldToConvert;
	bool IServerHolder.IsHoldingGrabbable => this.holdTakeControl.IsHoldingGrabbable;
	IServerGrabbable IServerHolder.HoldGrabbable { get { return this._holdGrabbable; } }


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
	public event EventHandler<ServerUseEventArgs> OnConvertStart;
	public event EventHandler<ServerUseEventArgs> OnConverting;
	public event EventHandler<ServerUseEventArgs> OnConvertEnd;
	public event EventHandler<ServerUseEventArgs> OnConvert;

	protected override void Awake()
	{
		base.Awake();

		if (this.holdTakeControl == null || this.convertControl == null)
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

		// convert control DI
		{
			UseInitArgs useInitArgs = new UseInitArgs();
			useInitArgs.AddParentInstance(this);
			useInitArgs.AddGetInfoFunc(() => this.Info);
			useInitArgs.AddGetTargetFunc(GetConvertTarget);
			this.convertControl.OnConvertStart += (sender, args) => { this.OnConvertStart?.Invoke(sender, args); };
			this.convertControl.OnConvertStart += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.convertControl.OnConverting += (sender, args) => { this.OnConverting?.Invoke(sender, args); };
			this.convertControl.OnConverting += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.convertControl.OnConvertEnd += (sender, args) => { this.OnConvertEnd?.Invoke(sender, args); };
			this.convertControl.OnConvertEnd += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.convertControl.OnConvert += (sender, args) => { this.OnConvert?.Invoke(sender, args); };
			this.convertControl.OnConvert += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			convertControl.DepsInit(useInitArgs);
		}
	}

	private IServerCombinable GetConvertTarget()
	{
		if (this._holdGrabbable == null) return null;
		switch (this._holdGrabbable)
		{
			case IServerHolder targetHolder:
				if (targetHolder.Info != ((IHolderSO)this._info).BindingHolder && (((IHolderSO)this._info).BindingHolder != null))
				{
					Debug.LogError("converter holding an unexpected holder (non-binding Holder)");
					return null;
				}
				if (!targetHolder.IsHoldingGrabbable) return null;
				else return (IServerCombinable)targetHolder.HoldGrabbable;

			case IServerCombinable holdCombinable:
				return holdCombinable;
			default:
				return null;
		}
	}


	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this.holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this.holdTakeControl.OnTakeServerInternal(out takenGrabbable);
	void IServerConverter.OnConvertStartServerInternal() => this.convertControl.OnConvertStartServerInternal();
	void IServerConverter.OnConvertEndServerInternal() => this.convertControl.OnConvertEndServerInternal();
}