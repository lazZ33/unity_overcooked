using System;
using UnityEngine;
using Unity.Netcode;
using TNRD;

[Serializable]
[CreateAssetMenu(fileName = "Spawn", menuName = "ScriptableObject/Spawnable")]
public class SpawnerSO: InteractableSO, ISpawnerSO, IHolderSO
{
	// IHolderSO implementation
	[SerializeField] private Vector3 _localPlacePosition;
	public Vector3 LocalPlacePosition => this._localPlacePosition;
	[SerializeField] private Quaternion _localPlaceRotation;
	public Quaternion LocalPlaceRotation => this._localPlaceRotation;
	[SerializeField] private SerializableInterface<IHolderSO> _bindingHolder;
	public IHolderSO BindingHolder => this._bindingHolder.Value;


	// ISpawnableSO implementation
	[SerializeField] private Sprite _spawnDisplay = null;
    public Sprite SpawnDisplay => this._spawnDisplay;
	[SerializeField] private Vector3 _spawnningPosition;
	public Vector3 SpawnningPosition => this._spawnningPosition;
	[SerializeField] private Quaternion _spawnningRotation;
	public Quaternion SpawnningRotation => this._spawnningRotation;


	public static new SpawnerSO GetSO(string strKey) => (SpawnerSO)IInteractableSO.GetSO(strKey);
    public static new SpawnerSO TryGetSO(string strKey) => (SpawnerSO)IInteractableSO.TryGetSO(strKey);
}