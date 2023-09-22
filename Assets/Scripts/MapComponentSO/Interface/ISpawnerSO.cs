using UnityEngine;

public interface ISpawnerSO: IInteractableSO {
	public Vector3 SpawnningPosition { get; }
	public Quaternion SpawnningRotation { get; }
}