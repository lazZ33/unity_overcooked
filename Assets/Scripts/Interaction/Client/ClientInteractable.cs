using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;


public class ClientInteractable: NetworkBehaviour{

    [SerializeField] protected ServerInteractable _server;
    [SerializeField] protected InteractableSO _info;
    public InteractableSO Info => this._info;

    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public NetworkObject NetworkObjectBuf { get; private set; }

    protected virtual void Awake(){
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        this._server.OnInfoChange += this.OnInfoChangeCallback;
    }

    public override void OnNetworkSpawn(){
        if (!this.IsClient) this.enabled = false;

        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
    }

    protected virtual void OnInfoChangeCallback(FixedString128Bytes previous, FixedString128Bytes current){
        this._info = InteractableSO.GetSO(current.ToString());
    }

}