using UnityEngine;
using Unity.Netcode;

public class ClientInteractable: NetworkBehaviour{
    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public NetworkObject NetworkObjectBuf { get; private set; }
    protected virtual void Awake(){
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
    }
    public override void OnNetworkSpawn()
    {
        if (!this.IsClient) this.enabled = false;
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
    }

}