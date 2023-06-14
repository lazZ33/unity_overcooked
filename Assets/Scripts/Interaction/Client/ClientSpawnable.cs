using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ClientSpawnable : ClientInteractable {

    private new ServerSpawnable _server => (ServerSpawnable)base._server;
    private new SpawnableSO _info => (SpawnableSO)base._info;
    public new SpawnableSO Info => (SpawnableSO)base._info;

    // protected override void Awake(){
    //     base.Awake();
    // }

    // public override void OnNetworkSpawn(){
    //     this._server.InfoStrKey.OnValueChanged += this.OnServerInfoInit;
    // }

    // internal void OnServerInfoInit(FixedString128Bytes previous, FixedString128Bytes cur){
    //     if (!this.IsClient) return;
    //     this._spawnningGrabbableInfo = CombinableSO.GetSO(this._server.InfoStrKey.Value.ToString());
    //     // if (this._spawnningGrabbableInfo == null) return;
    //     // this._spawnningGrabbableInfo.RegisterObject();
    // }

    // public void RequestSpawnAndGrab(PlayerGrabbingControl grabbingControl){
    //     this._server.SpawnAndGrabServerRpc(grabbingControl.NetworkObjectReferenceBuf);
    // }

}