using System;
using UnityEngine;
using Unity.Netcode;
using TNRD;
using System.Collections.Generic;

[Serializable]
[CreateAssetMenu(fileName = "Utensil", menuName = "ScriptableObject/Utensil")]
public class UtensilSO: InteractableSO, IHolderSO, IGrabbableSO {
	// IHolderSO implementation
	[SerializeField] private Vector3 _localPlacePosition;
	public Vector3 LocalPlacePosition => this._localPlacePosition;
	[SerializeField] private Quaternion _localPlaceRotation;
	public Quaternion LocalPlaceRotation => this._localPlaceRotation;
	[SerializeField] private SerializableInterface<IHolderSO> _bindingHolder;
	public IHolderSO BindingHolder => this._bindingHolder.Value;


	// IGrabbableSO implementation
	HashSet<IHolderSO> IGrabbableSO._existingPlaceableTo { get; } = new HashSet<IHolderSO>();


	public static new UtensilSO GetSO(string strKey) => (UtensilSO)IInteractableSO.GetSO(strKey);
	public static new UtensilSO TryGetSO(string strKey) => (UtensilSO)IInteractableSO.TryGetSO(strKey);
}
