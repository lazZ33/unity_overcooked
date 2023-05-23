using UnityEngine;
using Unity.Netcode;
using System;
public class ServerInteractionManager: NetworkBehaviour{

    public void GrabServerInternal(ServerGrabbable grabbable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (grabbable.IsGrabbedByPlayer) return;

        grabbable.OnGrabServerInternal(grabbingControl);
        grabbingControl.OnGrabTakeServerInternal(grabbable);
    }

    public void DropServerInternal(ServerGrabbable grabbable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (!grabbable.IsGrabbedByPlayer) return; // briefly checking

        grabbable.OnDropServerInternal();
        grabbingControl.OnDropPlaceServerInternal();
    }

    public void InteractServerInternal(ServerGrabbable retainedGrabbable, ServerGrabbable removedGrabbable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }

        if (removedGrabbable.IsGrabbedByPlayer){
                // assume the grabbing control holding removed grabbable is the initiator of this interaction
                // ServerPlayerGrabbingControl grabbingControl = HelperFunc.dereference<ServerPlayerGrabbingControl>(grabbingControlReference);
                grabbingControl.OnDropPlaceServerInternal();
        }
        retainedGrabbable.OnInteractServerInternal(removedGrabbable);
    }

    public void PlaceToHolderServerInternal(ServerGrabbable targetGrabbable, ServerHolder targetHolder, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (targetHolder.IsHoldingGrabbable) return;
    
        targetHolder.OnPlaceServerInternal(targetGrabbable);
        targetGrabbable.OnPlaceToServerInternal(targetHolder);
        grabbingControl.OnDropPlaceServerInternal();

        switch(targetHolder){
            case ServerStationeryUtility targetStationeryUtility:
                if (targetStationeryUtility.CanUseOn(targetGrabbable) && !targetStationeryUtility.HoldToUse){
                    IUsable targetUsable = targetStationeryUtility; // TODO: change to implicit implementation of interface to skip this 
                    targetUsable.OnUseServerInternal(grabbingControl);
                    grabbingControl.OnUseServerInternal((targetStationeryUtility.Info).UsableName);
                }
                break;
        }
    }
    // overload for map manager
    public void PlaceToHolderServerInternal(ServerGrabbable targetGrabbable, ServerHolder targetHolder){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (targetHolder.IsHoldingGrabbable) return;
    
        targetHolder.OnPlaceServerInternal(targetGrabbable);
        targetGrabbable.OnPlaceToServerInternal(targetHolder);
    }

    public void TakeFromHolderServerInternal(ServerHolder targetHolder, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (!targetHolder.IsHoldingGrabbable | grabbingControl.IsHoldingGrabbable) return;

        targetHolder.OnTakeServerInternal(out ServerGrabbable targetGrabbable);
        targetGrabbable.OnGrabServerInternal(grabbingControl);
        grabbingControl.OnGrabTakeServerInternal(targetGrabbable);

        switch(targetHolder){
            case ServerStationeryUtility targetStationeryUtility:
                if (targetStationeryUtility.CanUseOn(targetGrabbable) && !targetStationeryUtility.HoldToUse){
                    IUsable targetUsable = targetStationeryUtility; // TODO: change to implicit implementation of interface to skip this 
                    targetUsable.OnUnuseServerInternal(grabbingControl);
                    grabbingControl.OnUnuseServerInternal();
                }
                break;
        }
    }

    public void SpawnAndGrabServerInternal(ServerSpawnable targetSpawnable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (grabbingControl.IsHoldingGrabbable) return;

        ServerGrabbable newGrabbable = targetSpawnable.SpawnGrabbableServerInternal();
        grabbingControl.OnGrabTakeServerInternal(newGrabbable);
    }

    public void UseServerInternal(IUsable targetUsable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }

        targetUsable.OnUseServerInternal(grabbingControl);
    }

    public void UnuseServerInternal(IUsable targetUsable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }

        targetUsable.OnUnuseServerInternal(grabbingControl);
    }

}