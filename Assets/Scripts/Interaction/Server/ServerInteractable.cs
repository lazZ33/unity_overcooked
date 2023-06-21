using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using Codice.Client.Common.GameUI;
using System.Runtime.InteropServices.WindowsRuntime;

public class ServerInteractable: NetworkBehaviour{

    [SerializeField] protected ClientInteractable _client;
    [SerializeField] protected InteractableSO _info = null;
    public InteractableSO Info => this._info;

    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public NetworkObject NetworkObjectBuf { get; private set; }
    public event EventHandler<InfoChangeEventArgs> OnInfoChange;
    public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChangeFromNV { get{ return this._infoStrKey.OnValueChanged; } set { this._infoStrKey.OnValueChanged = value; }}


    internal static readonly ulong GRABBED_CLIENT_DEFAULT = ulong.MaxValue;
    internal static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";
    private NetworkVariable<FixedString128Bytes> _infoStrKey { get; } = new NetworkVariable<FixedString128Bytes>(ServerInteractable.INFO_STR_KEY_DEFAULT, NetworkVariableReadPermission.Everyone);

    protected virtual void Awake(){
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
    }

    public override void OnNetworkSpawn(){
        if (!IsServer) this.enabled = false;

        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        if (this._info != null){
            this._infoStrKey.Value = this._info.StrKey;
            this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs(this._info));
        }
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
    }

    public void InfoInit(InteractableSO info){
        if (this.IsSpawned){
            Debug.LogError("Attempt to initialize info after an object have network spawned");
            return;
        }
        print("InfoInit");

        this._info = info;
    }

    internal virtual void SetInfoServerInternal(string infoStrKey){
        print("SetInfoServerInternal");
        this._info = InteractableSO.GetSO(infoStrKey);
        this._infoStrKey.Value = infoStrKey;

        this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs(this._info));
    }
}