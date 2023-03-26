using UnityEngine;
using Unity;
using Unity.Netcode;

public abstract class ServerHolder: ServerInteractable{

    [SerializeField] private ClientHolder _client;
    [SerializeField] private Transform _placePosition = null;
    public Transform PlacePosition => this._placePosition;

    private Rigidbody _rigidbody = null;
    private ServerGrabbable _holdGrabbable = null;
    private bool IsHoldingGrabbable => this._holdGrabbable != null;

    protected override void Awake(){
        base.Awake();
        this.TryGetComponent<Rigidbody>(out this._rigidbody);
        if (this._rigidbody != null) this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HoldInfoClearServerRpc(){
        this._holdGrabbable = null;
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlaceServerRpc(NetworkObjectReference targetGrababbleReference, ulong grabClientId){
        if (this.IsHoldingGrabbable) return;
        ServerGrabbable targetGrabbable = HelperFunc.dereference<ServerGrabbable>(targetGrababbleReference);
        NetworkObject playerObject = NetworkManager.ConnectedClients[grabClientId].PlayerObject;
        ServerPlayerGrabbingControl grabbingControl = playerObject.GetComponent<ServerPlayerGrabbingControl>();
        print("PlaceServerRpc");

        this._holdGrabbable = targetGrabbable;
        grabbingControl.GrabInfoClearServerRpc();
        targetGrabbable.PlaceToServerRpc(this.NetworkObjectReferenceBuf, grabbingControl.OwnerClientId);
    }
    // function overload specifically for map populator
    [ServerRpc(RequireOwnership = false)]
    public void PlaceServerRpc(NetworkObjectReference targetGrababbleReference){
        if (this.IsHoldingGrabbable) return;
        ServerGrabbable targetGrabbable = HelperFunc.dereference<ServerGrabbable>(targetGrababbleReference);
        print("PlaceServerRpc");

        this._holdGrabbable = targetGrabbable;
        targetGrabbable.PlaceToServerRpc(this.NetworkObjectReferenceBuf);
    }

    // public void TryTransferGrabbableControlTo(PlayerGrabbingControl grabbingControl){
    //     if (!this.IsHoldingGrabbable) return;
    //     ClientGrabbable targetGrabbable = this.NetworkHoldGrabbable;
    //     print("base Take");

    //     targetGrabbable.RequestGrabBy(grabbingControl);
    //     this.TakeServerRpc();
    //     // this.NetworkHoldGrabbable = null;
    //     // return targetGrabbable;
    // }
    [ServerRpc(RequireOwnership = false)]
    public void TakeServerRpc(ulong grabClientId){
        if (!this.IsHoldingGrabbable) return;
        ServerPlayerGrabbingControl grabbingControl = NetworkManager.ConnectedClients[grabClientId].PlayerObject.GetComponent<ServerPlayerGrabbingControl>();
        ServerGrabbable targetGrabbable = this._holdGrabbable;
        print("base Take");

        targetGrabbable.GrabFromHolderServerRpc(grabbingControl.OwnerClientId, this.NetworkObjectReferenceBuf);
        this._holdGrabbable = null;
    }
}