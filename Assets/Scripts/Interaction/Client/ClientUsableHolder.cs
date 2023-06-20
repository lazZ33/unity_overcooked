using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ClientUsableHolder: ClientHolder{

    public event EventHandler OnUse;
    public event EventHandler OnUsing;
    public event EventHandler OnUnuse;

    private new ServerUsableHolder _server => (ServerUsableHolder)base._server;
    private new UsableHolderSO _info => (UsableHolderSO)base._info;
    public new UsableHolderSO Info => (UsableHolderSO)base._info;
    internal bool IsHoldToUse => (this.Info).IsHoldToUse;

    // protected override void Awake(){
    //     base.Awake();
    //     this._server.InfoStrKey.OnValueChanged += OnServerInfoChange;
    // }
    
    // protected override void OnServerInfoChange(FixedString128Bytes previous, FixedString128Bytes cur){
    //     base.OnServerInfoChange(previous, cur);
    //     // this.Info.RegisterObject();
    // }

    [ClientRpc]
    internal void InteractionCallbackClientRpc(InteractionCallbackID id, double useHoldStartTime, double useHoldCurTime){
        switch(id){
            case InteractionCallbackID.OnUse:
                this.OnUse?.Invoke(this, EventArgs.Empty);
                break;
            case InteractionCallbackID.OnUsing:
                this.OnUsing?.Invoke(this, EventArgs.Empty);
                break;
            case InteractionCallbackID.OnUnuse:
                this.OnUnuse?.Invoke(this, EventArgs.Empty);
                break;
        }
    }
}