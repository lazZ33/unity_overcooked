using UnityEngine;
using Unity.Netcode;
using System;
public class ServerInteractionManager: NetworkBehaviour{

    public void GrabServerInternal(ServerGrabbable grabbable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (grabbable.IsGrabbedByPlayer) return;
        print("GrabServerInternal");

        grabbable.OnGrabServerInternal(grabbingControl);
        grabbingControl.OnGrabTakeServerInternal(grabbable);
    }

    public void DropServerInternal(ServerGrabbable grabbable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (!grabbable.IsGrabbedByPlayer) return; // briefly checking
        print("DropServerInternal");

        grabbable.OnDropServerInternal();
        grabbingControl.OnDropPlaceServerInternal();
    }

    public void CombineServerInternal(ServerCombinable retainedCombinable, ServerCombinable removedCombinable){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (!retainedCombinable.CanCombineWith(removedCombinable)) return;
        print("CombineServerInternal");

        retainedCombinable.OnCombineServerInternal(removedCombinable);
    }
    public void CombineServerInternal(ServerCombinable retainedCombinable, ServerCombinable removedCombinable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (!retainedCombinable.CanCombineWith(removedCombinable)) return;
        print("CombineServerInternal (with grabbingControl)");

        if (removedCombinable.IsGrabbedByPlayer){
                // assume the grabbing control holding removed grabbable is the initiator of this interaction
                // ServerPlayerGrabbingControl grabbingControl = HelperFunc.dereference<ServerPlayerGrabbingControl>(grabbingControlReference);
                grabbingControl.OnDropPlaceServerInternal();
        }
        this.CombineServerInternal(retainedCombinable, removedCombinable);
    }
    public void HolderCombineServerInternal(ServerCombinable targetCombinable, ServerHolder targetHolder, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (!targetHolder.IsHoldingGrabbable) return;
        print("CombineOnHolderServerInternal");

        switch (targetHolder.HoldGrabbable){
            case ServerCombinable holderCombinable:
                if (holderCombinable.CanCombineWith(targetCombinable)){
                    targetHolder.OnTakeServerInternal(out ServerGrabbable holderGrabbable);
                    targetCombinable.OnCombineServerInternal(holderCombinable);

                    if (targetCombinable.CanPlaceOn(targetHolder))
                        targetHolder.OnPlaceServerInternal(targetCombinable);
                    else
                        grabbingControl.OnGrabTakeServerInternal(targetCombinable);
                }
                return;
        }
    }
    public void UtensilCombineServerInteranl(ServerUtensil targetUtensil, ServerCombinable targetCombinable){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        print("UtensilCombineServerInteranl");

        if (targetUtensil.HoldGrabbable == null){
            this.PlaceToServerInternal(targetCombinable, targetUtensil);
        }
        if (targetUtensil.HoldGrabbable.Info.GetType() != typeof(ServerCombinable)) return;

        ServerCombinable combinableOnUtensil = (ServerCombinable)targetUtensil.HoldGrabbable;

        if (targetCombinable.Info.CanCombineWith(combinableOnUtensil.Info)){

            CombinableSO newCombinableSO = CombinableSO.GetSO(CombinableSO.GetNextSOStrKey(targetCombinable.Info, targetCombinable.Info));
            targetUtensil.OnTakeServerInternal(out ServerGrabbable targetGrabbable);

            if (newCombinableSO.CanPlaceOn(targetUtensil.Info)){
                ((ServerCombinable)targetGrabbable).OnCombineServerInternal(targetCombinable);
                targetUtensil.OnPlaceServerInternal(targetGrabbable);
            }
            else{
                targetCombinable.OnCombineServerInternal((ServerCombinable)targetCombinable);
            }
        }
    }
    public void UtensilCombineServerInteranl(ServerUtensil priorityUtensil, ServerUtensil targetUtensil){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        print("UtensilCombineServerInteranl");

        switch (priorityUtensil.HoldGrabbable, targetUtensil.HoldGrabbable){
            case (null, ServerGrabbable targetGrabbable):
                this.PlaceToServerInternal(targetGrabbable, priorityUtensil);
                break;
            case (ServerGrabbable targetGrabbable, null):
                this.PlaceToServerInternal(targetGrabbable, targetUtensil);
                break;
            case (ServerCombinable priorityCombinable, ServerCombinable targetCombinable):
                if (!priorityCombinable.Info.CanCombineWith(targetCombinable.Info)) return;

                CombinableSO newCombinableSO = CombinableSO.GetSO(CombinableSO.GetNextSOStrKey(priorityCombinable.Info, targetCombinable.Info));
                priorityUtensil.OnTakeServerInternal(out ServerGrabbable targetGrabbable1);
                targetUtensil.OnTakeServerInternal(out ServerGrabbable targetGrabbable2);
                priorityCombinable.OnCombineServerInternal(targetCombinable);

                if (priorityCombinable.CanPlaceOn(priorityUtensil)){
                    priorityUtensil.OnPlaceServerInternal(priorityCombinable);
                    priorityCombinable.OnGrabServerInternal(priorityUtensil);
                }
                else{
                    targetUtensil.OnPlaceServerInternal(priorityCombinable);
                }
                break;
        }
    }

    public void PlaceToServerInternal(ServerGrabbable targetGrabbable, ServerHolder targetHolder, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (targetHolder.IsHoldingGrabbable) return;
        if (!targetGrabbable.CanPlaceOn(targetHolder) && targetHolder.GetType() != typeof(ServerTable)) return;
        print("PlaceToHolderServerInternal");
    
        targetHolder.OnPlaceServerInternal(targetGrabbable);
        targetGrabbable.OnPlaceToServerInternal(targetHolder);
        grabbingControl.OnDropPlaceServerInternal();

        switch(targetHolder){
            case ServerStationeryUtility targetStationeryUtility:
                if (targetGrabbable.CanPlaceOn(targetStationeryUtility) && !targetStationeryUtility.IsHoldToUse){
                    IUsable targetUsable = targetStationeryUtility; // TODO: change to implicit implementation of interface to skip this 
                    targetUsable.OnUseServerInternal(grabbingControl);
                    grabbingControl.OnUseServerInternal(targetStationeryUtility.Info.name);
                }
                break;
        }
    }
    public void PlaceToServerInternal(ServerGrabbable targetGrabbable, ServerHolder targetHolder){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (targetHolder.IsHoldingGrabbable) return;
        if (!targetGrabbable.CanPlaceOn(targetHolder) && targetHolder.GetType() != typeof(ServerTable)) return;
        print("PlaceToHolderServerInternal");
    
        targetHolder.OnPlaceServerInternal(targetGrabbable);
        targetGrabbable.OnPlaceToServerInternal(targetHolder);
    }
    public void PlaceToServerInternal(ServerGrabbable targetGrabbable, ServerUtensil targetUtensil){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (targetUtensil.IsHoldingGrabbable) return;
        if (!targetGrabbable.CanPlaceOn(targetUtensil)) return;
        print("PlaceToUtensilServerInternal");

        targetUtensil.OnPlaceServerInternal(targetGrabbable);
        targetGrabbable.OnGrabServerInternal(targetUtensil);
    }

    public void TakeFromHolderServerInternal(ServerHolder targetHolder, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        if (!targetHolder.IsHoldingGrabbable | grabbingControl.IsHoldingGrabbable) return;
        print("TakeFromHolderServerInternal");

        targetHolder.OnTakeServerInternal(out ServerGrabbable targetGrabbable);
        targetGrabbable.OnTakeServerInternal(grabbingControl);
        grabbingControl.OnGrabTakeServerInternal(targetGrabbable);

        switch(targetHolder){
            case ServerStationeryUtility targetStationeryUtility:
                if (targetGrabbable.CanPlaceOn(targetStationeryUtility) && !targetStationeryUtility.IsHoldToUse){
                    IUsable targetUsable = targetStationeryUtility; // TODO: change to implicit implementation of interface to skip this 
                    targetUsable.OnUnuseServerInternal(grabbingControl);
                    grabbingControl.OnUnuseServerInternal();
                }
                break;
        }
    }

    public void SpawnAndGrabServerInternal(ServerSpawnable targetSpawnable, ServerPlayerGrabbingControl grabbingControl){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        print("SpawnAndGrabServerInternal");
        if (grabbingControl.IsHoldingGrabbable) return;

        ServerGrabbable newGrabbable = targetSpawnable.SpawnCombinableServerInternal();
        grabbingControl.OnGrabTakeServerInternal(newGrabbable);
        newGrabbable.OnGrabServerInternal(grabbingControl);
    }

    public void ConvertServerInternal(ServerCombinable targetCombinable, ServerStationeryUtility targetStationeryUtility){
        if (!this.IsServer) { Debug.LogError("Calling interactions in interaction manager directly from client-side"); return; }
        print("ConvertServerInternal");

        string newGrabbbaleInfoStrKey = CombinableSO.GetNextSOStrKey(targetCombinable.Info, targetStationeryUtility.Info);
        targetCombinable.SetInfoServerInternal(newGrabbbaleInfoStrKey);
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