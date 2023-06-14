using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

internal class ServerCombineControl: ServerInteractControl{
    [SerializeField] private ClientCombineControl _client;
    [SerializeField] private ServerInteractableSharedData _data;

    public event EventHandler<InteractionEventArgs> OnCombine;

    public bool CanCombineWith(ServerCombineControl targetCombineControl) => this._data.Info.CanCombineWith(targetCombineControl.Info);
    public InteractableCompositionSO Info => this._data.Info;

    internal void SetInfoServerInternal(string infoStrKey){
        print("SetInfoServerInternal");
        this._data.Info = InteractableCompositionSO.GetSO(infoStrKey);
    }

    internal void OnCombineServerInternal(ServerCombineControl targetCombineControl){
        print("OnCombineServerInternal");

        this.SetInfoServerInternal(InteractableCompositionSO.GetNextSOStrKey(this._data.Info, targetCombineControl.Info));
        targetCombineControl.NetworkObjectBuf.Despawn();

        this.OnCombine?.Invoke(this, new InteractionEventArgs(this._data.Info, targetCombineControl));
        // this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnCombine);
        print("Combined");
    }
}
