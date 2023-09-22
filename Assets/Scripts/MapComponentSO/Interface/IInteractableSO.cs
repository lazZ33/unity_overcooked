using Unity;
using UnityEngine;
using System;
using System.Collections.Generic;

public interface IInteractableSO {
	public Mesh Mesh { get; }
	public Mesh MeshCollider { get; }
	public Material Material { get; }

	public string StrKey { get; }

	protected static Dictionary<string, IInteractableSO> _existingInteractable = new Dictionary<string, IInteractableSO>();
	protected static Dictionary<string, IInteractableSO> _allInteractable = new Dictionary<string, IInteractableSO>();

	public static IInteractableSO GetSO(string strKey) {
		IInteractableSO._existingInteractable.TryGetValue(strKey, out IInteractableSO result);
		if (result == null)
			result = IInteractableSO._allInteractable[strKey];

		if (result == null)
			Debug.LogError("SO resource failed to load, possibly corrupted SO name list. Tried string key: " + strKey);
		return result;
	}
	public static IInteractableSO TryGetSO(string strKey) {
		IInteractableSO._existingInteractable.TryGetValue(strKey, out IInteractableSO result);
		if (result == null)
			IInteractableSO._allInteractable.TryGetValue(strKey, out result);
		return result;
	}

}