using System;
using Unity;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public interface IClientInteractable {
	public IInteractableSO Info { get; }

	public NetworkObjectReference NetworkObjectReferenceBuf { get; }
	public NetworkObject NetworkObjectBuf { get; }


	public event EventHandler<InfoChangeEventArgs> OnInfoChange;
	//internal event EventHandler<InteractionCallbackID> OnInteractionCallbackExtensionHook;
}