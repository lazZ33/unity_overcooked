using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ClientSpawner : ClientInteractable, IClientSpawner
{
	ISpawnerSO IClientSpawner.Info => ((ISpawnerSO)base._info);


	public event EventHandler<SpawnEventArgs> OnSpawn;


	protected override void Awake()
	{
		base.Awake();
		base._onInteractionCallbackExtensionHook += this.InteractionEventCallback;
	}


	void InteractionEventCallback(object sender, InteractionEventExtensionEventArgs args)
	{
		switch (args.Id)
		{
			case InteractionCallbackID.OnSpawn:
				this.OnSpawn?.Invoke(sender, new SpawnEventArgs((IGrabbableSO)args.Info)); break;
		}
	}
}