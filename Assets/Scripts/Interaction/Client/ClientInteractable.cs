using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using TNRD;

public class ClientInteractable: NetworkBehaviour, IClientInteractable
{
    [SerializeField] private SerializableInterface<IServerInteractable> server;
    protected IServerInteractable _server => this.server.Value;
    [SerializeField] private SerializableInterface<IInteractableSO> info;
    protected IInteractableSO _info { get { return this.info.Value; } set { this.info.Value = value; } }
    public IInteractableSO Info => this.info.Value;


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
		IInteractableSO info = IInteractableSO.GetSO(SOstrKey.ToString());
		switch (id)
		{
			case InteractionCallbackID.OnGrab:
                this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs(info)); break;
			default:
				this._onInteractionCallbackExtensionHook?.Invoke(this, new InteractionEventExtensionEventArgs(id, info));
				break;
		}
	}
}