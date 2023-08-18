using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;
using TNRD;

public class ServerInteractable : NetworkBehaviour, IServerInteractable
{
	[SerializeField] protected ClientInteractable _client;
	[SerializeField] protected SerializableInterface<IInteractableSO> _info = null;
	public IInteractableSO Info => this._info.Value;


	public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
	public NetworkObject NetworkObjectBuf { get; private set; }
	public event EventHandler<InfoChangeEventArgs> OnInfoChange;
	public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChangeFromNV
		{ get { return this._infoStrKey.OnValueChanged; } set { this._infoStrKey.OnValueChanged = value; } }


	internal static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";
	private NetworkVariable<FixedString128Bytes> _infoStrKey { get; } = new NetworkVariable<FixedString128Bytes>(INFO_STR_KEY_DEFAULT, NetworkVariableReadPermission.Everyone);


	protected virtual void Awake()
	{
		if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
	}

	public override void OnNetworkSpawn()
	{
		if (!IsServer) this.enabled = false;

		if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
		if (this._info != null)
		{
			this._infoStrKey.Value = this._info.Value.StrKey;
			this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs(this._info.Value));
			this._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnInfoChange, this._info.Value.StrKey);
		}
		this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
	}

	public void OnMapDespawn(object sender, EventArgs args)
	{
		this.NetworkObjectBuf.Despawn();
	}

	public void InfoInit(IInteractableSO info)
	{
		if (this.IsSpawned)
		{
			Debug.LogError("Attempt to initialize info after an object have network spawned");
			return;
		}

		this._info.Value = info;
	}

	void IServerInteractable.SetInfoServerInternal(IInteractableSO newInfo)
	{
		print("SetInfoServerInternal");
		this._info.Value = newInfo;
		this._infoStrKey.Value = newInfo.StrKey;

		this.OnInfoChange?.Invoke(this, new InfoChangeEventArgs(this._info.Value));
		this._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnInfoChange, this._info.Value.StrKey);
	}
}