using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
public class ClientGrabbable : ClientInteractable{

    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private GrabbableSO _info;
    [SerializeField] private ServerGrabbable _server;

    public GrabbableSO Info => this._info;

    public bool IsGrabbedByPlayer => this._server.IsGrabbedByPlayer;
    public bool IsGrabbedByLocal => this._server.IsGrabbedByLocal;
    // public bool CanPlaceOn(ClientGrabbable targetGrabbable) => this.Info.CanPlaceOn(targetGrabbable.Info);
    // public bool CanUseOn(ClientGrabbable targetGrabbable) => this.Info.CanUseOn(targetGrabbable.Info);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (this.IsClient && !this.IsServer) this._meshCollider.enabled = false;

        this._server._infoStrKey.OnValueChanged += this.OnInfoChange;
        this.OnInfoChange(ServerGrabbable.INFO_STR_KEY_DEFAULT, this._server._infoStrKey.Value);
    }

    public void OnInfoChange(FixedString128Bytes previous, FixedString128Bytes current){
        if (current == ServerGrabbable.INFO_STR_KEY_DEFAULT) return;
        print("OnChangeInfo");
        
        GrabbableSO grabbableSO = GrabbableSO.GetSO(current.ToString());

        this._info = grabbableSO;
        this._meshFilter.mesh = this.Info.Mesh;
        this._meshCollider.sharedMesh = this.Info.MeshCollider;
        for (int i = 0; i < this._renderer.materials.Length; i++){
            this._renderer.materials[i] = this.Info.Material;
        }
    }


    // public void RequestSetInfo(string InfoStrKey){
    //     print("RequestSetInfo");

    //     this._server.SetInfoServerRpc(new FixedString128Bytes(InfoStrKey));
    // }    

    // public void RequestGrabBy(PlayerGrabbingControl grabbingControl){
    //     if (this.IsGrabbedByPlayer()) return;
    //     print("RequestGrabBy");

    //     this._server.GrabServerRpc(grabbingControl.OwnerClientId);
    // }

    // public void RequestGrabFromHolder(PlayerGrabbingControl grabbingControl, ClientHolder holder){
    //     if (this.IsGrabbedByPlayer()) return;
    //     print("RequestGrabBy");

    //     this._server.GrabFromHolderServerRpc(grabbingControl.OwnerClientId, holder.NetworkObjectReferenceBuf);
    // }

    // public void RequestDrop(){
    //     print("RequestDrop");

    //     this._server.DropServerRpc();
    // }

    // public void RequestInteract(ClientGrabbable targetGrabbable, PlayerGrabbingControl grabbingControl){
    //     print("RequestInteract");

    //     this._server.InteractServerRpc(targetGrabbable.NetworkObjectReferenceBuf, grabbingControl.OwnerClientId);
    // }

    // public void RequestPlaceTo(ClientHolder targetHolder, PlayerGrabbingControl grabbingControl){
    //     print("RequestPlaceTo");

    //     this._server.PlaceToServerRpc(targetHolder.NetworkObjectReferenceBuf, grabbingControl.OwnerClientId);
    // }
    // public void RequestPlaceTo(ClientHolder targetHolder){
    //     print("RequestPlaceTo");

    //     this._server.PlaceToServerRpc(targetHolder.NetworkObjectReferenceBuf);
    // }


    public void Update(){
        // if (!this.IsOwner) return;
        // // Debug.Log(string.Format("OwnerId: {0}, LocalId: {1}, GrabbedClientId: {2} InfoStrKey: {3}", this.OwnerClientId, NetworkManager.LocalClientId, this.GrabbedClientId, this._infoStrKey));

        // if (this.GrabbedClientId == NetworkManager.LocalClientId && this._targetTransform != null) {
            // this._rigidbody.MovePosition(this._targetTransform.position);
        // }
    }

}