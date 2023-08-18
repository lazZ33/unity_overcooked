using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

internal class ServerSpawnControl : ServerInteractControl
{
    [SerializeField] private GameObject _grabbablePrefab;
    internal IGrabbableSO SpawnningGrabbableInfo;

	// shared dependencies to be injected
	private new ISpawnerSO _info { get { return (ISpawnerSO)base._info; } }
	private new IServerSpawner _parentInstance { get { return (IServerSpawner)base._parentInstance; } }


	public event EventHandler<SpawnEventArgs> OnSpawn;


	// builder DI
	internal class SpawnControlInitArgs: InteractControlInitArgs
	{
		internal SpawnControlInitArgs() { }
	}
	internal override void DepsInit(InteractControlInitArgs args)
	{
		base.DepsInit(args);
	}


	internal IServerGrabbable OnSpawnServerInternal(){
        // spawn target object
        GameObject newGrabbableObject = Instantiate(this._grabbablePrefab, this._info.SpawnningPosition, this._info.SpawnningRotation);
		IServerGrabbable newGrabbableControl = newGrabbableObject.GetComponent<IServerGrabbable>();
		newGrabbableControl.NetworkObjectBuf.Spawn(true);
		newGrabbableControl.SetInfoServerInternal(this.SpawnningGrabbableInfo);

        this.OnSpawn?.Invoke(this._parentInstance, new SpawnEventArgs(newGrabbableControl.Info));
        print("spawned");
        return newGrabbableControl;
    }
}