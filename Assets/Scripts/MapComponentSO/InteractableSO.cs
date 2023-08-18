using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Collections;

[Serializable]
public abstract class InteractableSO: ScriptableObject
{
    [SerializeField] private Mesh _mesh;
    public Mesh Mesh => this._mesh;
    [SerializeField] private Mesh _meshCollider;
    public Mesh MeshCollider => this._meshCollider;
    [SerializeField] private Material _material;
    public Material Material => this._material;

    public virtual string StrKey { get { return this.name; } set {} }

    protected static Dictionary<string, InteractableSO> _existingInteractable = new Dictionary<string, InteractableSO>();
    protected static Dictionary<string, InteractableSO> _allInteractable = new Dictionary<string, InteractableSO>();

    public static void LoadAllSO(){
        InteractableSO[] allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Combinable");
        foreach (InteractableSO currentInteractableSO in allInteractableSO){
            _allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }

        allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Holder");
        foreach (InteractableSO currentInteractableSO in allInteractableSO){
            _allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }

        allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Spawnable");
        foreach (InteractableSO currentInteractableSO in allInteractableSO){
            _allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }

        allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Utensil");
        foreach (InteractableSO currentInteractableSO in allInteractableSO){
            _allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }
    }

    public static void UnloadAllSO(){
        InteractableSO._allInteractable.Clear();
    }

    public static InteractableSO GetSO(string strKey){
        InteractableSO._existingInteractable.TryGetValue(strKey, out InteractableSO result);
        if (result == null) result = InteractableSO._allInteractable[strKey];

        if (result == null) Debug.LogError("SO resource failed to load, possibly corrupted SO name list. Tried string key: " + strKey);        
        return result;
    }
    public static InteractableSO TryGetSO(string strKey){
        InteractableSO._existingInteractable.TryGetValue(strKey, out InteractableSO result);
        if (result == null) InteractableSO._allInteractable.TryGetValue(strKey, out result);
        return result;
    }
}