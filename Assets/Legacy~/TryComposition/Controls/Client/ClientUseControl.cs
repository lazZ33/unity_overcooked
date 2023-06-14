using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ClientUseControl: ClientInteractControl{
    [SerializeField] private ServerUseControl _server;
    [SerializeField] private ClientInteractableSharedData _clientData;
    public InteractableCompositionSO Info => this._server.Info;

    public event EventHandler OnUse;
    public event EventHandler OnUsing;
    public event EventHandler OnUnuse;

    internal bool CanUseOn(ServerInteractableComposition targetGrabbable) => this._clientData.Info.CanUseOn(targetGrabbable.Info);
    internal bool IsHoldToUse => (this.Info).IsHoldToUse;

    protected override void Awake(){
        base.Awake();
        this._server.OnInfoChange += OnServerInfoChange;
    }
    
    private void OnServerInfoChange(FixedString128Bytes previous, FixedString128Bytes current){
        // this.Info.RegisterObject();
    }

    [ClientRpc]
    internal void InteractionCallbackClientRpc(InteractionCallbackID id, double useHoldStartTime, double useHoldCurrentTime){
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