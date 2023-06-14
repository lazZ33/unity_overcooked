using System;
using UnityEngine;
using Unity.Netcode;

[Serializable]
[CreateAssetMenu(fileName = "Spawn", menuName = "ScriptableObject/Spawnable")]
public class SpawnableSO: InteractableSO{
    [SerializeField] private Sprite _spawnDisplay = null;
    public Sprite SpawnDisplay => this._spawnDisplay;
    [SerializeField] private Vector3 _localSpawnPoint;
    public Vector3 LocalSpawnPoint => this._localSpawnPoint;

    public static new SpawnableSO GetSO(string strKey) => (SpawnableSO)InteractableSO.GetSO(strKey);
    public static new SpawnableSO TryGetSO(string strKey) => (SpawnableSO)InteractableSO.TryGetSO(strKey);

}