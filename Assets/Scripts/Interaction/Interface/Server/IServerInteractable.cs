using System;
using Unity;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public interface IServerInteractable
{
	public IInteractableSO Info { get; }

	public NetworkObjectReference NetworkObjectReferenceBuf { get; }
	public NetworkObject NetworkObjectBuf { get; }


	public event EventHandler<InfoChangeEventArgs> OnInfoChange;
	public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChangeFromNV { get; set; }


	public void OnMapDespawn(object sender, EventArgs args)
	{
		this.NetworkObjectBuf.Despawn();
	}

	public void InfoInit(IInteractableSO info);

	internal void SetInfoServerInternal(IInteractableSO newInfo);
}