using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;


public class ClientInteractable: NetworkBehaviour, IClientInteractable
{
    [SerializeField] protected IServerInteractable _server;
    [SerializeField] protected IInteractableSO _info;
    public IInteractableSO Info => this._info;


    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public NetworkObject NetworkObjectBuf { get; private set; }
    public event EventHandler<InfoChangeEventArgs> OnInfoChange;
    internal EventHandler<InteractionEventExtensionEventArgs> _onInteractionCallbackExtensionHook;


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
        this._info = IInteractableSO.GetSO(current.ToString());

        this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs(this._info));
    }


    [ClientRpc]
    internal void InteractionEventCallbackClientRpc(InteractionCallbackID id, FixedString128Bytes SOstrKey)
    {
        InteractableSO info = InteractableSO.GetSO(SOstrKey.ToString());
		switch (id)
		{
			case InteractionCallbackID.OnGrab:
                this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs((IInteractableSO)info)); break;
			default:
				this._onInteractionCallbackExtensionHook?.Invoke(this, new InteractionEventExtensionEventArgs(id, (IInteractableSO)info));
				break;
		}
	}
}