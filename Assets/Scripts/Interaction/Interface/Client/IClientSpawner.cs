using System;
using Unity;

public interface IClientSpawner : IClientInteractable
{
	public new ISpawnerSO Info { get; }

	public event EventHandler<SpawnEventArgs> OnSpawn;
}