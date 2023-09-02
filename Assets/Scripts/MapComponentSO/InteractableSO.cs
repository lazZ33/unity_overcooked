using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Collections;

[Serializable]
public abstract class InteractableSO: ScriptableObject, IInteractableSO
{
    [SerializeField] private Mesh _mesh;
    public Mesh Mesh => this._mesh;
    [SerializeField] private Mesh _meshCollider;
    public Mesh MeshCollider => this._meshCollider;
    [SerializeField] private Material _material;
    public Material Material => this._material;

    string IInteractableSO.StrKey { get { return this._strKey; } }
    private string _strKey { get { return this.name; } }

    //protected static Dictionary<string, IInteractableSO> _existingInteractable = new Dictionary<string, IInteractableSO>();
    //protected static Dictionary<string, IInteractableSO> _allInteractable = new Dictionary<string, IInteractableSO>();

    public static void LoadAllSO(){
		IInteractableSO[] allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Combinable");
        foreach (IInteractableSO currentInteractableSO in allInteractableSO){
            IInteractableSO._allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }

        allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Holder");
        foreach (IInteractableSO currentInteractableSO in allInteractableSO){
			IInteractableSO._allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }

        allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Spawner");
        foreach (IInteractableSO currentInteractableSO in allInteractableSO){
            IInteractableSO._allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }

        allInteractableSO = Resources.LoadAll<InteractableSO>("InteractableSO/Utensil");
        foreach (IInteractableSO currentInteractableSO in allInteractableSO){
			IInteractableSO._allInteractable.TryAdd(currentInteractableSO.StrKey, currentInteractableSO);
        }

		// Add basic interactables to _existingInteractable
		// TODO: try not to hard code?
		IInteractableSO._existingInteractable.Add("Table", IInteractableSO.GetSO("Table"));
		IInteractableSO._existingInteractable.Add("Spawner", IInteractableSO.GetSO("Spawner"));
		IInteractableSO._existingInteractable.Add("DishExit", IInteractableSO.GetSO("DishExit"));
		IInteractableSO._existingInteractable.Add("Player", IInteractableSO.GetSO("Player"));

		HelperFunc.LogEnumerable("_allInteractable", IInteractableSO._allInteractable.Keys);
    }

    public static void UnloadAllSO(){
		IInteractableSO._allInteractable.Clear();
    }

    //public static IInteractableSO GetSO(string strKey){
    //    InteractableSO._existingInteractable.TryGetValue(strKey, out IInteractableSO result);
    //    if (result == null) result = InteractableSO._allInteractable[strKey];

    //    if (result == null) Debug.LogError("SO resource failed to load, possibly corrupted SO name list. Tried string key: " + strKey);        
    //    return result;
    //}
    //public static IInteractableSO TryGetSO(string strKey){
    //    InteractableSO._existingInteractable.TryGetValue(strKey, out IInteractableSO result);
    //    if (result == null) InteractableSO._allInteractable.TryGetValue(strKey, out result);
    //    return result;
    //}
}