using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerCombinable: ServerGrabbable{

    public new CombinableSO Info => (CombinableSO)base._client.Info;
    private new CombinableSO _info { get{ return (CombinableSO)base._info; } set{ base._info = value; } }
    private new ClientCombinable _client => (ClientCombinable)base._client;

    public event EventHandler<InteractionEventArgs> OnCombine;

    public bool CanCombineWith(ServerCombinable targetCombinable) => this._info.CanCombineWith(targetCombinable._info);

    internal void OnCombineServerInternal(ServerCombinable removedCombinable){
        print("OnCombineServerInternal");

        this.SetInfoServerInternal(CombinableSO.GetNextSOStrKey(this._info, removedCombinable._info));
        removedCombinable.NetworkObjectBuf.Despawn();

        this.OnCombine?.Invoke(this, new InteractionEventArgs(this._info, removedCombinable));
        this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnCombine);
        print("Combined");
    }

}