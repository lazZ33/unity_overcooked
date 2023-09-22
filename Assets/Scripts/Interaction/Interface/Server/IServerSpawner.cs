using System;
using Unity;

public interface IServerSpawner: IServerInteractable {
	public new ISpawnerSO Info { get; }

	public event EventHandler<SpawnEventArgs> OnSpawn;

	public void SpawnningGrabbableInfoInit(IGrabbableSO info);

	internal IServerGrabbable OnSpawnServerInternal();
}