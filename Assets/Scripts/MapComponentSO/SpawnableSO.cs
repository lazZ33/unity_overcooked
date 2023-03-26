using System;
using UnityEngine;
using Unity.Netcode;

[Serializable]
[CreateAssetMenu(fileName = "Spawn", menuName = "ScriptableObject/Spawnable")]
public class SpawnableSO: InteractableSO{
    [SerializeField] public GrabbableSO SpawnningSO = null;
    [SerializeField] public Sprite SpawnDisplay = null;

    public static SpawnableSO GetSO(String strKey){
        SpawnableSO info = (SpawnableSO) Resources.Load("SpawnableSO/" + strKey);
        if (info == null) Debug.LogError("SO resource failed to load, possibly corrupted filename or SO name list. Tried path: SpawnableSO/" + strKey);
        return info;
    }
}