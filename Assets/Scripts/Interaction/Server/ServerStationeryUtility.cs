using UnityEngine;
using Unity;
using Unity.Netcode;

public class ServerStationeryUtility : ServerHolder, IUsable
{
    internal bool CanUseOn(ServerGrabbable targetGrabbable) => ((StationeryUtilitySO)this._info).CanUseOn(targetGrabbable.Info);
    internal bool HoldToUse => ((StationeryUtilitySO)this._info).HoldToUse;
    public new StationeryUtilitySO Info => (StationeryUtilitySO)this._info;

    internal override void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        base.OnPlaceServerInternal(targetGrabbable);

        if (this.IsHoldingGrabbable) return;
        print("OnPlaceServerInternal");
    }
    void IUsable.OnUseServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsHoldingGrabbable | !this.CanUseOn(this._holdGrabbable) | !this.Info.HoldToUse) return;
        
        grabbingControl.OnUseServerInternal(this.Info.UsableName);
        return;
    }

    void IUsable.OnUnuseServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (!this.Info.HoldToUse) return;
        
        grabbingControl.OnUseServerInternal(this.Info.UsableName);
        return;
    }

    void IUsable.OnUseHoldServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsHoldingGrabbable | !this.CanUseOn(this._holdGrabbable) | this.Info.HoldToUse) return;

        grabbingControl.OnUseHoldServerInternal();
        return;
    }

    void IUsable.OnUseUnholdServerInternal(ServerPlayerGrabbingControl grabbingControl){
        if (this.Info.HoldToUse) return;

        grabbingControl.OnUseUnholdServerInternal();
        return;
    }

    void FixedUpdate(){
        if (!this.Info.HoldToUse && this.IsHoldingGrabbable){
            ServerGrabbable holdingGrabbable = this._holdGrabbable;
            // TODO: update "using progress"
        }
    }
}