using Unity;
using UnityEngine;
using System;
using TNRD;

[Serializable]
[CreateAssetMenu(fileName = "StationeryUtility", menuName = "ScriptableObject/StationeryUtility")]
public class StationeryUtilitySO: InteractableSO, IHolderSO, IUsableSO{
	// IHolderSO implementation
	[SerializeField] private Vector3 _localPlacePosition;
	public Vector3 LocalPlacePosition => this._localPlacePosition;
	[SerializeField] private Quaternion _localPlaceRotation;
	public Quaternion LocalPlaceRotation => this._localPlaceRotation;
	[SerializeField] private SerializableInterface<IHolderSO> _bindingHolder;
	public IHolderSO BindingHolder => this._bindingHolder.Value;


	// IUsableSO implementation
	[SerializeField] bool _isConverter;
	public bool IsConverter => this._isConverter;
	[SerializeField] bool _isHoldToUse;
	public bool IsHoldToUse => this._isHoldToUse;
	[SerializeField] double _onUsingUpdateInterval;
	public double OnUsingUpdateInterval => this._onUsingUpdateInterval;


	public static new StationeryUtilitySO GetSO(string strKey) => (StationeryUtilitySO)InteractableSO.GetSO(strKey);
	public static new StationeryUtilitySO TryGetSO(string strKey) => (StationeryUtilitySO)InteractableSO.TryGetSO(strKey);
}