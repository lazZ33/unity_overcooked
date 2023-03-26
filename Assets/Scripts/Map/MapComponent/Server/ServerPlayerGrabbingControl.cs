using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerPlayerGrabbingControl : NetworkBehaviour
{
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private Collider _grabCollider;
    public Collider GrabCollider => this._grabCollider;
    [SerializeField] private Transform _grabPosition;
    public Transform GrabPosition => this._grabPosition;

    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }

    private ServerGrabbable _holdGrabbable = null;
    private ClientPlayerGrabbingControl _client;

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
            if (Vector3.Distance(this._grabCollider.transform.position, hit.transform.position) >= Vector3.Distance(this._grabCollider.transform.position, targetHit.transform.position)){
                targetHit = hit;
            }
        }
        return targetHit.transform;
    }


    [ServerRpc(RequireOwnership = false)]
    public void GrabDropActionServerRpc(){
        print("GrabDropActionServerRpc");

        Transform targetInteractableTransform = this.GetExpectedTargetInteractableTransform();
        ServerInteractable targetInteractable = (targetInteractableTransform == null) ? null : targetInteractableTransform.GetComponent<ServerInteractable>();

        if (this._holdGrabbable != null) {

            if (targetInteractable != null){
                print("holdGrabbable and targetInteractable");
                // with both _holdGrabbable and _targetInteractable
                switch(targetInteractable){
                    case ServerGrabbable targetGrabbable:
                        print("holdGrabbable and targetGrabbable");
                        if (this._holdGrabbable.CanPlaceOn(targetGrabbable)){
                            switch(this._holdGrabbable){
                                case ServerIngredient holdIngredient:
                                    // holdIngredient combine with taregtGrabbable
                                    this.GrabbableInteract(targetGrabbable, holdIngredient);
                                    break;
                                case ServerTool holdTool:
                                    // targetGrabbable combine with holdIngredient
                                    this.GrabbableInteract(holdTool, targetGrabbable);
                                    break;
                            }
                            return;
                        }

                        break;
                    case ServerHolder targetHolder:
                        print("holdGrabbable and targetHolder");
                        // TODO: put _holdGrabbable onto the util
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
    public void GrabInfoUpdateServerRpc(NetworkObjectReference targetGrabbableReference){
        // if (!this.IsOwner) return;

        ServerGrabbable targetGrabbable = HelperFunc.dereference<ServerGrabbable>(targetGrabbableReference);
        print("Player Id " + this.OwnerClientId + " set grab info");

        this._holdGrabbable = targetGrabbable;
    }
    [ServerRpc(RequireOwnership = false)]
    public void GrabInfoClearServerRpc(){
        print("Player Id " + this.OwnerClientId + " clear grab info");
        this._holdGrabbable = null;
    }
    private void GrabDirect(ServerGrabbable targetGrabbable){
        targetGrabbable.GrabServerRpc(this.OwnerClientId);

        this._holdGrabbable = targetGrabbable;
    }
    private void TryGrabFromHolder(ServerHolder targetHolder){
        // TODO: check if sucess
        targetHolder.TakeServerRpc(this.OwnerClientId);
    }
    private void SpawnAndGrab(ServerSpawnable targetSpawnable){
        targetSpawnable.SpawnAndGrabServerRpc(this.OwnerClientId);
    }
    private void DropDirect(){
        this._holdGrabbable.DropServerRpc();
        // Assume drop must success, no need for a rpc round trip
        this._holdGrabbable = null;
    }
    private void TryDropToHolder(ServerHolder targetHolder){
        // TODO: check if sucess
        targetHolder.PlaceServerRpc(this._holdGrabbable.NetworkObjectReferenceBuf, this.OwnerClientId);
    }
    private void GrabbableInteract(ServerGrabbable retainedGrabbable, ServerGrabbable removedGrabbable){
        retainedGrabbable.InteractServerRpc(removedGrabbable.NetworkObjectReferenceBuf, this.OwnerClientId);
        // this._holdGrabbable = null;
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
