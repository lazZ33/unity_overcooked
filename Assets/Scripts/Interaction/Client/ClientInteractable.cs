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

    public event EventHandler<InfoChangeEventArgs> OnInfoChange;

    protected virtual void Awake(){
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        this._server.OnInfoChangeFromNV += this.OnInfoChangeCallback;
    }

    public override void OnNetworkSpawn(){
        if (!this.IsClient) this.enabled = false;

        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
    }

    protected virtual void OnInfoChangeCallback(FixedString128Bytes previous, FixedString128Bytes current){
        this._info = InteractableSO.GetSO(current.ToString());

        this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs(this._info));
    }

}