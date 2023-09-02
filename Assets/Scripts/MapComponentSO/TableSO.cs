using System;
using UnityEngine;
using Unity.Netcode;
using TNRD;

[Serializable]
[CreateAssetMenu(fileName = "Table", menuName = "ScriptableObject/Table")]
public class TableSO : InteractableSO, IHolderSO
{
	// IHolderSO
	[SerializeField] private Vector3 _localPlacePosition;
	public Vector3 LocalPlacePosition => this._localPlacePosition;
	[SerializeField] private Quaternion _localPlaceRotation;
	public Quaternion LocalPlaceRotation => this._localPlaceRotation;
	[SerializeField] private SerializableInterface<IHolderSO> _bindingHolder;
	public IHolderSO BindingHolder => this._bindingHolder.Value;


	public static new TableSO GetSO(string strKey) => (TableSO)IInteractableSO.GetSO(strKey);
}
