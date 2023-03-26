using UnityEngine;
using Unity.Netcode;

public class ClientSpawnable : ClientInteractable {
    // TODO: dynamically change _info according to received map config
    [SerializeField] private ServerSpawnable _server;
    private SpawnableSO _info;

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        this._info = SpawnableSO.GetSO(this._server.InfoStrKey.Value.ToString());
    }

    // public void RequestSpawnAndGrab(PlayerGrabbingControl grabbingControl){
    //     this._server.SpawnAndGrabServerRpc(grabbingControl.NetworkObjectReferenceBuf);
    // }

}