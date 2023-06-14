using Unity;
using UnityEngine;
using Unity.Netcode;
using System;

public class ServerStationeryUtility : ServerHolder, IUsable
{
    private ServerInteractionManager _interactions;
    
    public event EventHandler<UseEventArgs> OnUse;
    public event EventHandler<UseEventArgs> OnUsing;
    public event EventHandler<UseEventArgs> OnUnuse;
    public class UseEventArgs: EventArgs{
        internal UseEventArgs(GrabbableSO holdingGrabbableInfo){ this.holdingGrabbableInfo = holdingGrabbableInfo; }
        public GrabbableSO holdingGrabbableInfo;
    }
    
    private double UseHoldStartTime;
    private double OnUsingLastUpdateTime;

    private new ClientStationeryUtility _client => (ClientStationeryUtility)base._client;
    public new StationeryUtilitySO Info { get { return (StationeryUtilitySO)base._info; } set { base._info = value; } }    
    private double UseHoldCurTime => Time.fixedUnscaledTimeAsDouble;
    internal bool IsHoldToUse => (this.Info).IsHoldToUse;
    private bool _isHoldingUse => UseHoldStartTime != 0;

    protected override void Awake(){
        base.Awake();

        this._interactions = GameObject.FindObjectOfType<ServerInteractionManager>();
        if (this._interactions == null) { Debug.LogError("Failed to find ServerInteractionManager, please ensure there is one accessible in the scene");}
    }

    internal override void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        base.OnPlaceServerInternal(targetGrabbable);
        print("OnPlaceServerInternal: StationeryUtility");

        this.OnUse?.Invoke(this, new UseEventArgs(this.HoldGrabbable.Info));
    }

    internal override void OnTakeServerInternal(out ServerGrabbable takenGrabbable){
        base.OnTakeServerInternal(out takenGrabbable);
        print("OnTakeServerInternal: StationeryUtility");

        this.OnUse?.Invoke(this, new UseEventArgs(this.HoldGrabbable.Info));
    }

    void IUsable.OnUseHoldServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsHoldingGrabbable | !this.HoldGrabbable.CanPlaceOn(this) | this.Info.IsHoldToUse) return;
        print("OnUseHoldServerInternal");
        
        this.UseHoldStartTime = this.UseHoldCurTime;
        grabbingControl.OnUseHoldServerInternal();
        this.OnUnuse?.Invoke(this, new UseEventArgs(this.HoldGrabbable.Info));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUse, this.UseHoldStartTime, this.UseHoldCurTime);
        return;
    }

    void IUsable.OnUseUnholdServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (this.Info.IsHoldToUse) return;
        print("OnUseHoldServerInternal");
        
        this.UseHoldStartTime = 0;
        grabbingControl.OnUseUnholdServerInternal();
        this.OnUnuse?.Invoke(this, new UseEventArgs(this.HoldGrabbable.Info));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUnuse, this.UseHoldStartTime, this.UseHoldCurTime);
        return;
    }

    void IUsable.OnUseServerInternal(ServerPlayerGrabbingControl grabbingControl){}
    void IUsable.OnUnuseServerInternal(ServerPlayerGrabbingControl grabbingControl){}

    void FixedUpdate(){
        if (this.HoldGrabbable.GetType() != typeof(ServerCombinable)) return;
        ServerCombinable holdCombinable = (ServerCombinable)this.HoldGrabbable;

        if ((this._isHoldingUse | !this.Info.IsHoldToUse) && this.IsHoldingGrabbable){
            double timePassed = this.UseHoldCurTime - this.UseHoldStartTime;

            if (timePassed <= 0){
                // TODO: trigger event written in the GrabbableSO (e.g. fire hazard if any)
                this.OnUnuse?.Invoke(this, new UseEventArgs(this.HoldGrabbable.Info));
                this._interactions.ConvertServerInternal(holdCombinable, this);
            }
            else if (this.OnUsingLastUpdateTime - this.UseHoldCurTime > this.Info.OnUsingUpdateInterval){
                this.OnUsing?.Invoke(this, new UseEventArgs(this.HoldGrabbable.Info));
                this.OnUsingLastUpdateTime = this.UseHoldCurTime;
                this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUsing, this.UseHoldStartTime, this.UseHoldCurTime);
            }
        }
    }
}