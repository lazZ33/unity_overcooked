using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerPlayerGrabbingControl : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private Collider _grabCollider;
    public Collider GrabCollider => this._grabCollider;
    [SerializeField] private Transform _grabPosition;
    public Transform GrabPosition => this._grabPosition;
    [SerializeField] private ServerInteractionManager _interactions;

    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public event EventHandler OnGrabTakeServerEvent;
    public event EventHandler OnDropPlaceServerEvent;
    public event EventHandler OnInteractServerEvent;

    private ServerGrabbable _holdGrabbable = null;
    private ClientPlayerGrabbingControl _client;

    internal bool IsHoldingGrabbable => this._holdGrabbable != null;


    public override void OnNetworkSpawn(){
        if (!this.IsServer) {this.enabled = false; return;}
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObject);
    }

    public Transform GetExpectedTargetInteractableTransform(){

        Vector3 grabCenter = new Vector3(this._grabCollider.transform.position.x, this._grabCollider.transform.position.y + this._grabCollider.bounds.extents.y, this._grabCollider.transform.position.z);
        Vector3 grabHalfSize = new Vector3(this._grabCollider.transform.lossyScale.x/2, 0.01f, this._grabCollider.transform.lossyScale.z/2); // this._grabCollider.transform.lossyScale/2;
        Vector3 grabDirection = new Vector3(this._grabCollider.transform.up.x, -this._grabCollider.transform.up.y, this._grabCollider.transform.up.z); //this._grabCollider.transform.up;
        Quaternion grabOrientation = this.transform.rotation;
        float grabMaxDist = this._grabCollider.bounds.size.y;

        RaycastHit[] allHits = Physics.BoxCastAll(grabCenter, grabHalfSize, grabDirection, grabOrientation, grabMaxDist, this._interactableLayerMask);
        
        if (allHits.Length == 0 | allHits == null) return null;

        RaycastHit targetHit = allHits[0];
        bool validTargetExist = false;
        foreach (RaycastHit hit in allHits){
            if (hit.transform.TryGetComponent<ServerGrabbable>(out ServerGrabbable curGrabbable)){
                if (curGrabbable.IsGrabbedByPlayer){
                    continue;
                }
                targetHit = hit;
                validTargetExist = true;
            }
            else if (hit.transform.TryGetComponent<ServerInteractable>(out ServerInteractable curInteractable)){
                targetHit = hit;
                validTargetExist = true;
                break;
            }
        }
        
        if (!validTargetExist) return null;

        foreach (RaycastHit hit in allHits){
            if (Vector3.Distance(this._grabCollider.transform.position, hit.transform.position) <= Vector3.Distance(this._grabCollider.transform.position, targetHit.transform.position)){
                targetHit = hit;
            }
        }
        return targetHit.transform;
    }

    [ServerRpc(RequireOwnership = false)]
    internal void GrabDropActionServerRpc(){
        print("GrabDropActionServerRpc");

        Transform targetInteractableTransform = this.GetExpectedTargetInteractableTransform();
        ServerInteractable targetInteractable = (targetInteractableTransform == null) ? null : targetInteractableTransform.GetComponent<ServerInteractable>();

        if (this.IsHoldingGrabbable) {

            if (targetInteractable != null){
                print("holdGrabbable and targetInteractable");
                // with both _holdGrabbable and _targetInteractable
                switch(targetInteractable, this._holdGrabbable){
                    case (ServerUtensil targetUtensil, ServerUtensil holdUtensil):
                        this.CombineUtensils(holdUtensil, targetUtensil);
                        break;
                    case (ServerUtensil targetUtensil, ServerCombinable holdCombinable):
                        this.CombineWithUtensil(targetUtensil, holdCombinable);
                        break;
                    case (ServerCombinable targetCombinable, ServerUtensil holdUtensil):
                        this.CombineWithUtensil(holdUtensil, targetCombinable);
                        break;
                    case (ServerCombinable targetCombinable, ServerCombinable holdCombinable):
                        print("both combinable");
                        if (holdCombinable.CanCombineWith(targetCombinable)){
                            this.Combine(targetCombinable, holdCombinable);
                            print("can combine");
                        }
                        break;
                    case (ServerHolder targetHolder, ServerCombinable holdCombinable):
                        if (targetHolder.IsHoldingGrabbable)
                            this.HolderCombine(holdCombinable, targetHolder);
                        this.TryDropToHolder(targetHolder);
                        break;
                    case (ServerHolder targetHolder, ServerGrabbable holdGrabbable):
                        this.TryDropToHolder(targetHolder);
                        break;
                }

                // with both _holdGrabbable and _targetInteractable, missing all previous type matches
                return;
            }

            print("holdGrabbable");
            // only _holdGrabbable
            this.DropDirect();
            return;
        }

        // return if holding nothing and targeting nothing
        if(targetInteractable == null) { print("nothing"); return; }

        print("targetInteractable");
        // only _targetInteractable
        switch(targetInteractable){
            case ServerGrabbable targetGrabbable:
                this.GrabDirect(targetGrabbable);
                break;
            case ServerSpawnable targetSpawnable:
                this.SpawnAndGrab(targetSpawnable);
                break;
            case ServerHolder targetHolder:
                this.TryGrabFromHolder(targetHolder);
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    internal void UseActionServerRpc(){
        print("UseActionServerRpc");

        switch(this._holdGrabbable){
            case ServerTool holdTool:
                // TODO: use tool
                return;
            case ServerCombinable:
            case null:
                break;
        }

        Transform targetInteractableTransform = this.GetExpectedTargetInteractableTransform();
        if (targetInteractableTransform == null) return;
        ServerInteractable targetInteractable = targetInteractableTransform.GetComponent<ServerInteractable>();

        switch(targetInteractable){
            case ServerUsableHolder targetUsableHolder:
                // TODO: use util
                return;
            case ServerTable:
            case null:
                break;
        }

    }

    internal void OnGrabTakeServerInternal(ServerGrabbable targetGrabbable){
        // if (!this.IsOwner) return;
        print("Player Id " + this.OwnerClientId + " set grab info");
        this._holdGrabbable = targetGrabbable;
        OnGrabTakeServerEvent?.Invoke(this, EventArgs.Empty);
    }
    internal void OnDropPlaceServerInternal(){
        print("Player Id " + this.OwnerClientId + " clear grab info");
        this._holdGrabbable = null;
        OnDropPlaceServerEvent?.Invoke(this, EventArgs.Empty);
    }
    internal void OnUseServerInternal(string usableName){
        this._animator.Play(usableName);
    }
    internal void OnUnuseServerInternal(){
        this._animator.Play("");
    }
    internal void OnUseHoldServerInternal(){

    }
    internal void OnUseUnholdServerInternal(){
        
    }

    private void GrabDirect(ServerGrabbable targetGrabbable){
        this._interactions.GrabServerInternal(targetGrabbable, this);
    }
    private void TryGrabFromHolder(ServerHolder targetHolder){
        this._interactions.TakeFromHolderServerInternal(targetHolder, this);
    }
    private void SpawnAndGrab(ServerSpawnable targetSpawnable){
        this._interactions.SpawnAndGrabServerInternal(targetSpawnable, this);
    }
    private void DropDirect(){
        this._interactions.DropServerInternal(this._holdGrabbable, this);
    }
    private void TryDropToHolder(ServerHolder targetHolder){
        this._interactions.PlaceToServerInternal(this._holdGrabbable, targetHolder, this);
    }
    private void Combine(ServerCombinable retainedCombinable, ServerCombinable removedCombinable){
        this._interactions.CombineServerInternal(retainedCombinable, removedCombinable, this);
    }
    private void CombineWithUtensil(ServerUtensil targetUtensil, ServerCombinable targetCombinable){
        this._interactions.UtensilCombineServerInteranl(targetUtensil, targetCombinable);
    }
    private void CombineUtensils(ServerUtensil targetUtensil, ServerUtensil priorityUtensil){
        this._interactions.UtensilCombineServerInteranl(targetUtensil, priorityUtensil);
    }
    private void HolderCombine(ServerCombinable targetCombinable, ServerHolder targetHolder){
        this._interactions.HolderCombineServerInternal(targetCombinable, targetHolder, this);
    }
    private void UseUtility(IUsable targetUsable){
        this._interactions.UseServerInternal(targetUsable, this);
    }

    void OnDrawGizmos(){

        Vector3 grabCenter = new Vector3(this._grabCollider.transform.position.x, this._grabCollider.transform.position.y + this._grabCollider.bounds.extents.y, this._grabCollider.transform.position.z);
        Vector3 grabHalfSize = new Vector3(this._grabCollider.transform.lossyScale.x/2, 0.01f, this._grabCollider.transform.lossyScale.z/2); // this._grabCollider.transform.lossyScale/2;
        Vector3 grabDirection = new Vector3(this._grabCollider.transform.up.x, -this._grabCollider.transform.up.y, this._grabCollider.transform.up.z); //this._grabCollider.transform.up;
        Quaternion grabOrientation = this.transform.rotation;
        float grabMaxDist = this._grabCollider.bounds.size.y;

        if (Physics.BoxCast(grabCenter, grabHalfSize, grabDirection, out RaycastHit hit, grabOrientation, grabMaxDist, this._interactableLayerMask))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(grabCenter, grabDirection);
            Gizmos.DrawWireCube(grabCenter + grabDirection*hit.distance, grabHalfSize*2);
        } else {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(grabCenter, grabDirection*grabMaxDist);
            Gizmos.DrawWireCube(grabCenter, grabHalfSize*2);
        }

    }
}
