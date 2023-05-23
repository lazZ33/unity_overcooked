using UnityEngine;
using Unity;
using Unity.Netcode;

public abstract class ServerHolder: ServerInteractable{

    [SerializeField] protected ClientHolder _client;
    [SerializeField] protected HolderSO _info = null;
    public HolderSO Info => this._info;
    [SerializeField] private Transform _placePosition = null;
    public Transform PlacePosition => this._placePosition;

    internal bool IsHoldingGrabbable => this._holdGrabbable != null;
    protected ServerGrabbable _holdGrabbable = null;
    private Rigidbody _rigidbody = null;

    protected override void Awake(){
        base.Awake();
        this.TryGetComponent<Rigidbody>(out this._rigidbody);
        if (this._rigidbody != null) this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    internal virtual void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        if (this.IsHoldingGrabbable) return;
        print("base OnPlaceServerInternal");

        this._holdGrabbable = targetGrabbable;
    }

    internal virtual void OnTakeServerInternal(out ServerGrabbable takenGrabbable){
        if (!this.IsHoldingGrabbable) { Debug.LogError("OnTakeServerInternal called while not holding any grabbable"); takenGrabbable = null; return; }
        print("base OnTakeServerInternal");

        takenGrabbable = this._holdGrabbable;
        this._holdGrabbable = null;
    }
}