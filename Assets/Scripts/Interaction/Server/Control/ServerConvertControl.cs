using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ServerConvertControl: ServerInteractControl
{
    [SerializeField] private ServerInteractionManager _interactions;


    // shared dependencies to be injected
    internal delegate IServerCombinable GetTargetFunc();
    internal delegate void SetTargetFunc(IServerCombinable target);
    private GetTargetFunc _getTargetFunc;
    private SetTargetFunc _setTargetFunc;
	private IServerCombinable _targetCombinable { get { return this._getTargetFunc(); } set { this._setTargetFunc(value); } }
    private new IConverterSO _info { get { return (IConverterSO)base._info; } }
	private new IServerConverter _parentInstance { get { return (IServerConverter)base._parentInstance; } }


	public event EventHandler<ServerUseEventArgs> OnConvertStart;
    public event EventHandler<ServerUseEventArgs> OnConverting;
    public event EventHandler<ServerUseEventArgs> OnConvertEnd;
    public event EventHandler<ServerUseEventArgs> OnConvert;


    internal bool IsHoldingUseButton => this._convertHoldStartTime != 0;
    private bool _isTargetExist => this._targetCombinable != null;
    private double _targetDuration => this._info.ConvertDuration;
    private double _convertHoldCurrentTime => Time.fixedUnscaledTimeAsDouble;
    private double _onConvertingLastUpdateTime;
    private double _convertHoldStartTime;


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
	protected override void Start()
	{
		base.Start();

		if (this._getTargetFunc == null) //  || this._setTargetFunc == null
			throw new MissingReferenceException(String.Format("convert control not properly initialized before Start(), parent instance: {0}", this._parentInstance));
	}


	internal void OnConvertStartServerInternal()
    {
		if (!this._isTargetExist) return;

		this._convertHoldStartTime = this._convertHoldCurrentTime;
		this.OnConvertStart?.Invoke(this, new ServerUseEventArgs(this._targetCombinable));
		return;
	}
    internal void OnConvertEndServerInternal()
    {
		if (!this._isTargetExist) return;

		this._convertHoldStartTime = 0;
		this.OnConvertEnd?.Invoke(this, new ServerUseEventArgs(this._targetCombinable));
		return;
	}

    void FixedUpdate(){
        if ((this.IsHoldingUseButton | !this._info.IsHoldToConvert) && this._isTargetExist){

            double timePassed = this._convertHoldCurrentTime - this._convertHoldStartTime;
            double timeSinceLastUpdate = this._onConvertingLastUpdateTime - this._convertHoldCurrentTime;

            if (timePassed >= this._targetDuration){
                this.OnConvertEnd?.Invoke(this, new ServerUseEventArgs(this._targetCombinable));
                this.OnConvert?.Invoke(this._parentInstance, new ServerUseEventArgs(this._targetCombinable));

                if (this._info.IsConvertToVoid)
                    this._interactions.ConvertToVoidServerInternal(this._targetCombinable, this._parentInstance);
                else
                    this._interactions.ConvertServerInternal(this._targetCombinable, this._parentInstance);
            }
            else if (timeSinceLastUpdate > this._info.OnConvertUpdateInterval){
                this.OnConverting?.Invoke(this, new ServerUseEventArgs(this._targetCombinable));
                this._onConvertingLastUpdateTime = this._convertHoldCurrentTime;
                //this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUsing, this._useHoldStartTime, this._useHoldCurrentTime);
            }
        }
    }
}