using Unity;
using UnityEngine;
using System;
using TNRD;

[Serializable]
[CreateAssetMenu(fileName = "StationaryUtility", menuName = "ScriptableObject/StationaryUtility")]
public class StationaryUtilitySO: InteractableSO, IHolderSO, IConverterSO{
	// IHolderSO implementation
	[SerializeField] private Vector3 _localPlacePosition;
	public Vector3 LocalPlacePosition => this._localPlacePosition;
	[SerializeField] private Quaternion _localPlaceRotation;
	public Quaternion LocalPlaceRotation => this._localPlaceRotation;
	[SerializeField] private SerializableInterface<IHolderSO> _bindingHolder;
	public IHolderSO BindingHolder => this._bindingHolder.Value;


	// IConverterSO implementation
	[SerializeField] private bool _isHoldToConvert;
	public bool IsHoldToConvert => this._isHoldToConvert;
	[SerializeField] private double _onConvertUpdateInterval;
	public double OnConvertUpdateInterval => this._onConvertUpdateInterval;
	[SerializeField] private double _convertDuration;
	public double ConvertDuration => this._convertDuration;
	[SerializeField] private bool _isConvertToVoid;
	public bool IsConvertToVoid => this._isConvertToVoid;


	public static new StationaryUtilitySO GetSO(string strKey) => (StationaryUtilitySO)IInteractableSO.GetSO(strKey);
	public static new StationaryUtilitySO TryGetSO(string strKey) => (StationaryUtilitySO)IInteractableSO.TryGetSO(strKey);
}