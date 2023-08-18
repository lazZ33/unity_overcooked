using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ServerUseControl: ServerInteractControl
{
    private ServerInteractionManager _interactions = ServerInteractionManager.Instance;


    // shared dependencies to be injected
    internal delegate IServerInteractable GetTargetFunc();
    internal delegate void SetTargetFunc(IServerInteractable target);
    private GetTargetFunc _getTargetFunc;
    private SetTargetFunc _setTargetFunc;
	private IServerInteractable _target { get { return this._getTargetFunc(); } set { this._setTargetFunc(value); } }
    private IServerCombinable _targetCombinable { 
        get
        { 
            if (!this._info.IsConverter) { Debug.LogError("Attempt to get _target as ICombinable without explicitly marking its info as converter."); return null; }
            return (IServerCombinable)this._getTargetFunc();
        } 
        set 
        { 
            this._target = value;
        } 
    }
    private new IUsableSO _info { get { return (IUsableSO)base._info; } }
	private new IServerUsable _parentInstance { get { return (IServerUsable)base._parentInstance; } }


	public event EventHandler<ServerUseEventArgs> OnUse;
    public event EventHandler<ServerUseEventArgs> OnUsing;
    public event EventHandler<ServerUseEventArgs> OnUnuse;
    public event EventHandler<ServerUseEventArgs> OnConvert;


    internal bool IsHoldingUseButton => this._useHoldStartTime != 0;
    private bool _isTargetExist => this._target != null;
    private double _useHoldCurrentTime => Time.fixedUnscaledTimeAsDouble;
    private double _onUsingLastUpdateTime;
    private double _useHoldStartTime;


    // builder DI
    internal class UseControlInitArgs: InteractControlInitArgs
    {
        internal UseControlInitArgs() { }
        internal GetTargetFunc GetTargetFunc;
        //internal SetTargetFunc SetTargetFunc;
        internal void AddGetTargetFunc(GetTargetFunc getTargetFunc) => this.GetTargetFunc = getTargetFunc;
        //internal void AddSetTargetFunc(SetTargetFunc setTargetFunc) => this.SetTargetFunc = setTargetFunc;
    }
    internal void DepsInit(UseControlInitArgs args)
    {
        base.DepsInit(args);
        this._getTargetFunc = args.GetTargetFunc;
        //this._setTargetFunc = args.SetTargetFunc;
    }


    internal void OnUseServerInternal()
    {
		if (!this._isTargetExist) return;
		print("OnUseServerInternal");

		this._useHoldStartTime = this._useHoldCurrentTime;
		this.OnUnuse?.Invoke(this, new ServerUseEventArgs(this._target));
		return;
	}
    internal void OnUnuseServerInternal()
    {
		if (!this._isTargetExist) return;
		print("OnUnuseServerInternal");

		this._useHoldStartTime = 0;
		this.OnUnuse?.Invoke(this, new ServerUseEventArgs(this._target));
		return;
	}

    void FixedUpdate(){
        //if (!this._target.IsUsable) return;

        if ((this.IsHoldingUseButton | !this._info.IsHoldToUse) && this._isTargetExist){
            double timePassed = this._useHoldCurrentTime - this._useHoldStartTime;
            double timeSinceLastUpdate = this._onUsingLastUpdateTime - this._useHoldCurrentTime;

            if (timePassed <= 0){
                this.OnUnuse?.Invoke(this, new ServerUseEventArgs(this._target));
                if (this._info.IsConverter)
                {
                    this.OnConvert?.Invoke(this._parentInstance, new ServerUseEventArgs(this._targetCombinable));
                    this._interactions.ConvertServerInternal(this._targetCombinable, this._parentInstance);
                }
                // TODO: trigger event written in the GrabbableSO (e.g. fire hazard if any)
                //else
                //{

                //}
            }
            else if (timeSinceLastUpdate > this._info.OnUsingUpdateInterval){
                this.OnUsing?.Invoke(this, new ServerUseEventArgs(this._target));
                this._onUsingLastUpdateTime = this._useHoldCurrentTime;
                //this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUsing, this._useHoldStartTime, this._useHoldCurrentTime);
            }
        }
    }
}