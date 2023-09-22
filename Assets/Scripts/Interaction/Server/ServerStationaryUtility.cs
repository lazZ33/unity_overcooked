using Unity;
using UnityEngine;
using System;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;
using UseInitArgs = ServerConvertControl.UseControlInitArgs;

internal class ServerStationaryUtility: ServerInteractable, IServerConverter, IServerHolder {
	[SerializeField] private ServerHoldTakeControl _holdTakeControl;
	[SerializeField] private ServerConvertControl _convertControl;

	// DI variables
	private IServerGrabbable _holdGrabbable = null;


	IConverterSO IServerConverter.Info => (IConverterSO)base._info.Value;
	IHolderSO IServerHolder.Info => (IHolderSO)base._info.Value;
	bool IServerConverter.IsHoldToConvert => ((IConverterSO)this.Info).IsHoldToConvert;
	bool IServerHolder.IsHoldingGrabbable => this._holdTakeControl.IsHoldingGrabbable;
	IServerGrabbable IServerHolder.HoldGrabbable { get { return this._holdGrabbable; } }


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
	public event EventHandler<ServerConvertEventArgs> OnConvertStart;
	public event EventHandler<ServerConvertEventArgs> OnConverting;
	public event EventHandler<ServerConvertEventArgs> OnConvertEnd;
	public event EventHandler<ServerConvertEventArgs> OnConvert;

	protected override void Awake() {
		base.Awake();

		if (this._holdTakeControl == null || this._convertControl == null) {
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

		// convert control DI
		{
			UseInitArgs useInitArgs = new UseInitArgs();
			useInitArgs.AddParentInstance(this);
			useInitArgs.AddGetInfoFunc(() => this.Info);
			useInitArgs.AddGetTargetFunc(GetConvertTarget);
			this._convertControl.OnConvertStart += (sender, args) => { this.OnConvertStart?.Invoke(sender, args); };
			this._convertControl.OnConvertStart += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this._convertControl.OnConverting += (sender, args) => { this.OnConverting?.Invoke(sender, args); };
			this._convertControl.OnConverting += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this._convertControl.OnConvertEnd += (sender, args) => { this.OnConvertEnd?.Invoke(sender, args); };
			this._convertControl.OnConvertEnd += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this._convertControl.OnConvert += (sender, args) => { this.OnConvert?.Invoke(sender, args); };
			this._convertControl.OnConvert += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			_convertControl.DepsInit(useInitArgs);
		}
	}

	private IServerCombinable GetConvertTarget() {
		if (this._holdGrabbable == null)
			return null;
		switch (this._holdGrabbable) {
			case IServerHolder targetHolder:
				if (targetHolder.Info != ((IHolderSO)this._info).BindingHolder && (((IHolderSO)this._info).BindingHolder != null)) {
					Debug.LogError("converter holding an unexpected holder (non-binding Holder)");
					return null;
				}
				if (!targetHolder.IsHoldingGrabbable)
					return null;
				else
					return (IServerCombinable)targetHolder.HoldGrabbable;

			case IServerCombinable holdCombinable:
				return holdCombinable;
			default:
				return null;
		}
	}

	public override void OnMapUpdate() {
		this._holdTakeControl.OnMapUpdate();
		this._convertControl.OnMapUpdate();
	}

	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this._holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this._holdTakeControl.OnTakeServerInternal(out takenGrabbable);
	void IServerConverter.OnConvertStartServerInternal() => this._convertControl.OnConvertStartServerInternal();
	void IServerConverter.OnConvertEndServerInternal() => this._convertControl.OnConvertEndServerInternal();
}