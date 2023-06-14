using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ServerUseControl: ServerInteractControl{
    [SerializeField] private ClientUseControl _client;
    [SerializeField] private ServerInteractableSharedData _serverData;
    [SerializeField] private ServerInteractionManager _interactions;

    public event EventHandler<UseEventArgs> OnUse;
    public event EventHandler OnUsing;
    public event EventHandler<UseEventArgs> OnUnuse;
    public class UseEventArgs: EventArgs{
        internal UseEventArgs(InteractableCompositionSO holdingGrabbableInfo){ this.holdingGrabbableInfo = holdingGrabbableInfo; }
        public InteractableCompositionSO holdingGrabbableInfo;
    }
    public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChange { get { return this._serverData.InfoStrKey.OnValueChanged; } set { this._serverData.InfoStrKey.OnValueChanged += value; }}

    private double _useHoldStartTime;
    private double _useHoldCurrentTime => Time.fixedUnscaledTimeAsDouble;
    private double _onUsingLastUpdateTime;

    internal bool IsHoldingGrabbable => this._serverData.HoldGrabbable != null;
    internal bool IsHoldingUseButton => this._useHoldStartTime != 0;
    public InteractableCompositionSO Info => this._serverData.Info;


    void OnUseHoldServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsHoldingGrabbable | !this._serverData.Info.CanUseOn(this._serverData.HoldGrabbable.Info) | this.Info.IsHoldToUse) return;
        print("OnUseHoldServerInternal");
        
        this._useHoldStartTime = this._useHoldCurrentTime;
        grabbingControl.OnUseHoldServerInternal();
        this.OnUnuse?.Invoke(this, new UseEventArgs(this._serverData.HoldGrabbable.Info));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUse, this._useHoldStartTime, this._useHoldCurrentTime);
        return;
    }

    void OnUseUnholdServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (this.Info.IsHoldToUse) return;
        print("OnUseHoldServerInternal");
        
        this._useHoldStartTime = 0;
        grabbingControl.OnUseUnholdServerInternal();
        this.OnUnuse?.Invoke(this, new UseEventArgs(this._serverData.HoldGrabbable.Info));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUnuse, this._useHoldStartTime, this._useHoldCurrentTime);
        return;
    }

    void OnUseServerInternal(ServerPlayerGrabbingControl grabbingControl){}
    void OnUnuseServerInternal(ServerPlayerGrabbingControl grabbingControl){}

    void FixedUpdate(){
        if (!this._serverData.HoldGrabbable.IsUsable) return;

        if ((this.IsHoldingUseButton | !this.Info.IsHoldToUse) && this.IsHoldingGrabbable){
            double timePassed = this._useHoldCurrentTime - this._useHoldStartTime;
            double timeSinceLastUpdate = this._onUsingLastUpdateTime - this._useHoldCurrentTime;

            if (timePassed <= 0){
                // TODO: trigger event written in the GrabbableSO (e.g. fire hazard if any)
                this.OnUnuse?.Invoke(this, new UseEventArgs(this._serverData.HoldGrabbable.Info));
                this._interactions.ConvertServerInternal(this._serverData.HoldGrabbable, this);
            }
            else if (timeSinceLastUpdate > this.Info.OnUsingUpdateInterval){
                this.OnUsing?.Invoke(this, EventArgs.Empty);
                this._onUsingLastUpdateTime = this._useHoldCurrentTime;
                this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUsing, this._useHoldStartTime, this._useHoldCurrentTime);
            }
        }
    }
}