using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerInteractable: NetworkBehaviour{

    [SerializeField] protected ClientInteractable _client;
    [SerializeField] protected InteractableSO _info = null;
    public InteractableSO Info => this._info;

    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public NetworkObject NetworkObjectBuf { get; private set; }

    internal static readonly ulong GRABBED_CLIENT_DEFAULT = ulong.MaxValue;
    internal static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";
    private NetworkVariable<FixedString128Bytes> InfoStrKey { get; } = new NetworkVariable<FixedString128Bytes>(ServerInteractable.INFO_STR_KEY_DEFAULT, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChange { get { return this.InfoStrKey.OnValueChanged; } set { this.InfoStrKey.OnValueChanged = value; } }

    protected virtual void Awake(){
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
    }

    public override void OnNetworkSpawn(){
        if (!IsServer) this.enabled = false;

        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        if (this._info != null) this.InfoStrKey.Value = this._info.StrKey;
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
    }

    internal virtual void SetInfoServerInternal(string infoStrKey){
        print("SetInfoServerInternal");
        this._info = InteractableSO.GetSO(infoStrKey);
        this.InfoStrKey.Value = infoStrKey;
    }
}