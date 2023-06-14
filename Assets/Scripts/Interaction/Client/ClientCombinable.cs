using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public abstract class ClientCombinable: ClientGrabbable{

    public new CombinableSO Info => (CombinableSO)base._info;
    private new CombinableSO _info { get{ return (CombinableSO)base._info; } set{ base._info = value; } }
    private new ServerCombinable _server => (ServerCombinable)base._server;

    public event EventHandler<InteractionEventArgs> OnCombine;

    protected override void Awake(){
        base.Awake();
    }

    public override void OnNetworkSpawn(){
        base.OnInteractionCallbackExtensionHook += OnInteractionCallback;
    }

    // internal void OnInfoStrKeyChange(FixedString128Bytes previous, FixedString128Bytes cur){
    //     if (cur == ServerGrabbable.INFO_STR_KEY_DEFAULT) return;
    //     print("OnInfoStrKeyChange");
        
    //     CombinableSO CombinableSO = CombinableSO.GetSO(cur.ToString());

    //     this._info = CombinableSO;
    //     // this._meshCollider.sharedMesh = this.Info.MeshCollider; // not needed as collider in client should always be disabled
    //     this.OnInfoChange?.Invoke(this, new InteractionEventArgs(this._info));
    // }

    private void OnInteractionCallback(object sender, InteractionCallbackExtensionEventArgs args){
        switch (args.id){
            case InteractionCallbackID.OnCombine:
                this.OnCombine?.Invoke(this, args.Args);
                break;
        }
    }
}