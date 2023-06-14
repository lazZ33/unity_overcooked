using UnityEngine;
using Unity.Netcode;

public class ClientInteractControl: NetworkBehaviour{
    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public NetworkObject NetworkObjectBuf { get; private set; }
    protected virtual void Awake(){
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
    }
    public override void OnNetworkSpawn(){
        if (!IsServer) this.enabled = false;

        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
    }
}