using System;
using Unity;
using UnityEngine;

internal class ServerHoldTakeControl : ServerInteractControl
{
	// shared dependencies to be injected
	internal delegate IServerGrabbable GetHoldGrabbableFunc();
	internal delegate void SetHoldGrabbableFunc(IServerGrabbable holdGrabbable);
	private GetHoldGrabbableFunc _getHoldGrabbable = null;
	private SetHoldGrabbableFunc _setHoldGrabbable = null;
	private IServerGrabbable _holdGrabbable { get { return this._getHoldGrabbable(); } set { this._setHoldGrabbable(value); } }
	private new IHolderSO _info { get { return (IHolderSO)base._info; } }


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;


	internal bool IsHoldingGrabbable => this._holdGrabbable != null;
	
	
	// builder DI
	internal class HoldTakeControlInitArgs: InteractControlInitArgs
	{
		internal HoldTakeControlInitArgs() { }
		internal GetHoldGrabbableFunc GetHoldGrabbableFunc;
		internal SetHoldGrabbableFunc SetHoldGrabbableFunc;
		internal void AddGetHoldGrabbableFunc(GetHoldGrabbableFunc getHoldGrabbableFunc) => this.GetHoldGrabbableFunc = getHoldGrabbableFunc;
		internal void AddSetHoldGrabbableFunc(SetHoldGrabbableFunc setHoldGrabbableFunc) => this.SetHoldGrabbableFunc = setHoldGrabbableFunc;
	}
	internal void DepsInit(HoldTakeControlInitArgs args)
	{
		base.DepsInit(args);
		this._getHoldGrabbable = args.GetHoldGrabbableFunc;
		this._setHoldGrabbable = args.SetHoldGrabbableFunc;
	}


	protected override void Start()
	{
		base.Start();

		if (this._getHoldGrabbable != null || this._setHoldGrabbable != null)
			throw new MissingReferenceException("Grabbable not initialized before Start()");
	}

	internal void OnHoldServerInternal(IServerGrabbable targetGrabbable)
	{
		if (this.IsHoldingGrabbable) return;
		print("OnPlaceServerInternal");

		this._holdGrabbable = targetGrabbable;
	}

	internal void OnTakeServerInternal(out IServerGrabbable takenGrabbable)
	{
		if (!this.IsHoldingGrabbable) { Debug.LogError("OnTakeServerInternal called while not holding any grabbable"); takenGrabbable = null; return; }
		print("OnTakeServerInternal");

		takenGrabbable = this._holdGrabbable;
		this._holdGrabbable = null;
	}
}