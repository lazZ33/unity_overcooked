using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerGrabbable: ServerInteractable{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ClientGrabbable _client;
    public static readonly ulong GRABBED_CLIENT_DEFAULT = ulong.MaxValue;
    public static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";

    private NetworkVariable<ulong> _grabbedClientId { get; } = new NetworkVariable<ulong>(ServerGrabbable.GRABBED_CLIENT_DEFAULT, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public ulong GrabbedClientId => this._grabbedClientId.Value;
    public NetworkVariable<FixedString128Bytes> _infoStrKey { get; } = new NetworkVariable<FixedString128Bytes>(ServerGrabbable.INFO_STR_KEY_DEFAULT, NetworkVariableReadPermission.Everyone);
    // public FixedString128Bytes InfoStrKey => _infoStrKey.Value;
    public GrabbableSO Info => this._client.Info; // can't use this immediately after change because haven't sync(?)


    public bool IsGrabbedByPlayer => this.GrabbedClientId != ServerGrabbable.GRABBED_CLIENT_DEFAULT;
    public bool IsGrabbedByLocal => this.GrabbedClientId == NetworkManager.LocalClientId;
    public bool CanPlaceOn(ServerGrabbable targetGrabbable) => this.Info.CanPlaceOn(targetGrabbable.Info);


    [ServerRpc(RequireOwnership = false)]
    public void SetInfoServerRpc(FixedString128Bytes InfoStrKey){
        print("SetInfoServerRpc");
        this._infoStrKey.Value = InfoStrKey;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceToServerRpc(NetworkObjectReference holderReference, ulong grabClientId){
        NetworkObject playerObject = NetworkManager.ConnectedClients[grabClientId].PlayerObject;
        ServerPlayerGrabbingControl grabbingControl = playerObject.GetComponent<ServerPlayerGrabbingControl>();
        print("PlaceToServerRpc");

        this.PlaceToServerRpc(holderReference);
        grabbingControl.GrabInfoClearServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlaceToServerRpc(NetworkObjectReference holderReference){
        ServerHolder targetHolder = HelperFunc.dereference<ServerHolder>(holderReference);
        print("PlaceToServerRpc");

        this.NetworkObjectBuf.RemoveOwnership();

        this.NetworkObjectBuf.TrySetParent(targetHolder.transform, false);
        this._rigidbody.useGravity = false;
        this.transform.localPosition = targetHolder.PlacePosition.localPosition;
        this.transform.localPosition = targetHolder.PlacePosition.localPosition;
        this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        // this.gameObject.layer = LayerMask.NameToLayer("GrabbedGrabbable");

        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;
    }

    [ServerRpc(RequireOwnership = false)]
    public void GrabServerRpc(ulong grabClientId){
        if (this.IsGrabbedByPlayer) return;
        NetworkObject playerObject = NetworkManager.ConnectedClients[grabClientId].PlayerObject;
        Transform playerTransform = playerObject.transform;
        ServerPlayerGrabbingControl grabbingControl = playerObject.GetComponent<ServerPlayerGrabbingControl>();
        print("GrabServerRpc");

        this.NetworkObjectBuf.TrySetParent(playerTransform, false);
        this._rigidbody.useGravity = false;
        this.transform.localPosition = grabbingControl.GrabPosition.localPosition;
        this.transform.localRotation = grabbingControl.GrabPosition.localRotation;
        this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        // this.gameObject.layer = LayerMask.NameToLayer("GrabbedGrabbable");

        this.NetworkObjectBuf.ChangeOwnership(grabClientId);

        grabbingControl.GrabInfoUpdateServerRpc(this._client.NetworkObjectReferenceBuf);
        this._grabbedClientId.Value = grabClientId;
        print("grabbed");
        return;
    }
    [ServerRpc(RequireOwnership = false)]
    public void GrabFromHolderServerRpc(ulong grabClientId, NetworkObjectReference holderReference){
        if (this.IsGrabbedByPlayer) return;
        ServerHolder holder = HelperFunc.dereference<ServerHolder>(holderReference);
        print("GrabFromHolderServerRpc");

        this.GrabServerRpc(grabClientId);
        holder.HoldInfoClearServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DropServerRpc(){
        print("DropServerRpc");
        this.NetworkObjectBuf.RemoveOwnership();
        this.NetworkObjectBuf.TryRemoveParent();
        this._rigidbody.useGravity = true;
        this._rigidbody.constraints = RigidbodyConstraints.None;
        this._grabbedClientId.Value = ServerGrabbable.GRABBED_CLIENT_DEFAULT;
        // this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        print("Dropped");
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractServerRpc(NetworkObjectReference grabbableReference, ulong grabClientId){
        ServerGrabbable targetGrabbable = HelperFunc.dereference<ServerGrabbable>(grabbableReference);
        NetworkObject playerObject = NetworkManager.ConnectedClients[grabClientId].PlayerObject;
        ServerPlayerGrabbingControl grabbingControl = playerObject.GetComponent<ServerPlayerGrabbingControl>();
        print("InteractServerRpc");

        // GrabbableSO nextSO = GrabbableSO.getNextSO(this._client.Info, targetGrabbable.Info);
        this._infoStrKey.Value = GrabbableSO.getNextSOStrKey(this._client.Info, targetGrabbable.Info);
        targetGrabbable.NetworkObjectBuf.Despawn();
    }

    public void Update(){
        // if (!this.IsOwner) return;
        // Debug.Log(string.Format("OwnerId: {0}, LocalId: {1}, GrabbedClientId: {2} InfoStrKey: {3}", this.OwnerClientId, NetworkManager.LocalClientId, this.GrabbedClientId, this._infoStrKey));

        // if (this.GrabbedClientId == NetworkManager.LocalClientId && this._targetTransform != null) {
            // this._rigidbody.MovePosition(this._targetTransform.position);
        // }
    }

}