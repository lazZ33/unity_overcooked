// using System.Collections;
// using System.Collections.Generic;
// using Unity.Collections;
// using UnityEngine;
// using Unity.Netcode;

// public class TPServerGrabbable : TPNetworkInteractable{

//     [SerializeField] private Rigidbody _rigidbody;
//     [SerializeField] protected GrabbableSO Info;
//     private static readonly ulong GRABBED_CLIENT_DEFAULT = ulong.MaxValue;
//     private static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";

//     private readonly NetworkVariable<ulong> GrabbedClientIdNetworkVariable = new NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
//     public ulong GrabbedClientId { get => GrabbedClientIdNetworkVariable.Value; set => GrabbedClientIdNetworkVariable.Value = value; }
//     public NetworkObjectReference NetworkObjectBufReference { get; private set; }
//     public NetworkObject NetworkObjectBuf { get; private set; }

//     private Transform _targetTransform = null;
//     private readonly NetworkVariable<FixedString128Bytes> _infoStrKeyNetworkVariable = new NetworkVariable<FixedString128Bytes>(INFO_STR_KEY_DEFAULT, NetworkVariableReadPermission.Everyone);
//     private FixedString128Bytes _infoStrKey { get => _infoStrKeyNetworkVariable.Value; set => _infoStrKeyNetworkVariable.Value = value; }
//     private MeshFilter _meshFilter;
//     private MeshCollider _meshCollider;
//     private Renderer _renderer;

//     void Awake(){
//         this._rigidbody = this.GetComponent<Rigidbody>();
//         this._meshFilter = this.GetComponent<MeshFilter>();
//         this._meshCollider = this.GetComponent<MeshCollider>();
//         this._renderer = this.GetComponent<Renderer>();
//         this.GrabbedClientId = TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT;
//         if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;

//     }
//     void Start(){
//         this._meshFilter.mesh = this.Info.mesh;
//         this._meshCollider.sharedMesh = this.Info.mesh;
//         this._renderer.material = this.Info.material;
//     }
//     public override void OnNetworkSpawn(){
//         this._infoStrKeyNetworkVariable.OnValueChanged += this.OnChangeInfo;
//         // this.GrabbedClientIdNetworkVariable.OnValueChanged += this.OnGrabBy;

//         if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
//         this.NetworkObjectBufReference = new NetworkObjectReference(this.NetworkObjectBuf);
//         this.OnChangeInfo(TPNetworkGrabbable.INFO_STR_KEY_DEFAULT, this._infoStrKey);
//     }


//     [ServerRpc(RequireOwnership = false)]
//     public void SetInfoServerRpc(string InfoStrKey){
//         print("SetInfoServerRpc");
//         this._infoStrKey = InfoStrKey;
//         // this.OnSyncInfo(TPNetworkGrabbable.INFO_STR_KEY_DEFAULT, this._infoStrKey.Value);
//     }
//     private void OnChangeInfo(FixedString128Bytes previous, FixedString128Bytes current){
//         if (current == TPNetworkGrabbable.INFO_STR_KEY_DEFAULT) return;
//         print("OnChangeInfo");
        
//         GrabbableSO grabbableSO = GrabbableSO.GetSO(current.ToString());

//         this.Info = grabbableSO;
//         this._meshFilter.mesh = this.Info.mesh;
//         this._meshCollider.sharedMesh = this.Info.mesh;
//         this._renderer.material = this.Info.material;
//     }

//     public bool CanPlaceOn(TPNetworkGrabbable targetGrabbable){
//         return this.Info.CanPlaceOn(targetGrabbable.Info);
//     }
//     public bool CanUseOn(TPNetworkGrabbable targetGrabbable){
//         return this.Info.CanUseOn(targetGrabbable.Info);
//     }
//     public bool IsGrabbed(){
//         return this.GrabbedClientId != TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT;
//     }
    

//     // public void PlaceTo(TPNetworkHolder targetHolder){
//     //     if (this.GrabbedClientId != NetworkManager.LocalClientId) return;
//     //     print("PlaceTo");

//     //     // this._rigidbody.useGravity = false;
//     //     // this._targetTransform = targetTransform;
//     //     // this.transform.position = this._targetTransform.position;
//     //     // this.transform.position = this._targetTransform.position;
//     //     // this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;

//     //     this.PlaceToServerRpc(new NetworkObjectReference(targetHolder.NetworkObjectBuf));
//     // }
//     [ServerRpc(RequireOwnership = false)]
//     public void PlaceToServerRpc(NetworkObjectReference holderReference){
//         TPNetworkHolder targetHolder = HelperFunc.dereference<TPNetworkHolder>(holderReference);
//         print("PlaceToServerRpc");

//         this.NetworkObjectBuf.RemoveOwnership();

//         this._rigidbody.useGravity = false;
//         // this._targetTransform = targetTransform;
//         this.transform.position = targetHolder.PlacePosition.position;
//         this.transform.position = targetHolder.PlacePosition.position;

//         this.NetworkObjectBuf.TrySetParent(targetHolder.transform);
//         this.GrabbedClientId = TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT;
//     }
    

//     public void GrabBy(ulong GrabbedClientId, Transform targetTransform){
//         if (this.GrabbedClientId != TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT) return;
//         this._targetTransform = targetTransform;
//         this.GrabServerRpc(GrabbedClientId);
//     }
//     [ServerRpc(RequireOwnership = false)]
//     private void GrabServerRpc(ulong grabClientId){
//         if (this.GrabbedClientId != TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT) return;
//         print("GrabServerRpc");

//         NetworkObject playerObject = NetworkManager.ConnectedClients[grabClientId].PlayerObject;
//         Transform playerTransform = playerObject.transform;
//         TPNetworkGrabbingControl playerGrabbingControl = playerObject.GetComponent<TPNetworkGrabbingControl>();

//         this.NetworkObjectBuf.TrySetParent(playerTransform);
//         this.NetworkObjectBuf.ChangeOwnership(grabClientId);

//         this._rigidbody.useGravity = false;
//         this.transform.localPosition = playerGrabbingControl.GrabPosition.localPosition;
//         this.transform.localRotation = playerGrabbingControl.GrabPosition.localRotation;
//         this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;

//         this.GrabbedClientId = grabClientId;
//         print("grabbed");
//         return;
//     }
//     private void OnGrabBy(ulong previous, ulong current){
//         if (this.GrabbedClientId == TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT) return; // prevent calling due to call other than grab
//         if (this.GrabbedClientId != NetworkManager.LocalClientId) return;
//         print("OnGrab");

//         this._rigidbody.useGravity = false;
//         this.transform.localPosition = this._targetTransform.localPosition;
//         this.transform.localRotation = this._targetTransform.localRotation;
//         this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
//     }
//     // public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject) {}


//     public void Drop(){
//         print("start Drop");
//         if (this.GrabbedClientId != NetworkManager.LocalClientId) return;
//         if (this.GrabbedClientId == TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT) return;
//         print("Drop");

//         this._targetTransform = null;
//         this.DropServerRpc();
//     }
//     [ServerRpc(RequireOwnership = false)]
//     private void DropServerRpc(){
//         print("DropServerRpc");
//         this.NetworkObjectBuf.RemoveOwnership();
//         this.NetworkObjectBuf.TryRemoveParent();
//         this._rigidbody.useGravity = true;
//         this._rigidbody.constraints = RigidbodyConstraints.None;
//         this.GrabbedClientId = TPNetworkGrabbable.GRABBED_CLIENT_DEFAULT;
//         print("Dropped");
//     }

//     public void Interact(TPNetworkGrabbable targetGrabbable){
//         this.InteractServerRpc(new NetworkObjectReference(targetGrabbable.NetworkObjectBuf));
//     }
//     [ServerRpc(RequireOwnership = false)]
//     private void InteractServerRpc(NetworkObjectReference grabbableReference){
//         TPNetworkGrabbable targetGrabbable = HelperFunc.dereference<TPNetworkGrabbable>(grabbableReference);

//         GrabbableSO nextSO = GrabbableSO.getNextSO(this.Info, targetGrabbable.Info);
//         this.SetInfoServerRpc(nextSO.name);
//         targetGrabbable.NetworkObjectBuf.Despawn();
//     }

//     public void Update(){
//         // if (!this.IsOwner) return;
//         // // Debug.Log(string.Format("OwnerId: {0}, LocalId: {1}, GrabbedClientId: {2} InfoStrKey: {3}", this.OwnerClientId, NetworkManager.LocalClientId, this.GrabbedClientId, this._infoStrKey));

//         // if (this.GrabbedClientId == NetworkManager.LocalClientId && this._targetTransform != null) {
//             // this._rigidbody.MovePosition(this._targetTransform.position);
//         // }
//     }

// }