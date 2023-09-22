using Unity;
using UnityEngine;
using System;
using System.Collections.Generic;
using TNRD;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObject/Ingredient")]
public class IngredientSO: InteractableSO, ICombinableSO {
	// ICombinableSO implementation
	[SerializeField] private SerializableInterface<ICombinableSO>[] _requiredCombinables;
	ICombinableSO[] ICombinableSO.RequiredCombinables {
		get {
			if (this._requiredCombinables == null)
				return new ICombinableSO[0];
			return Enumerable.Select(
				this._requiredCombinables,
				(SerializableInterface<ICombinableSO> curCombinable, int idx) => curCombinable.Value
			).ToArray();
		}
	}
	[SerializeField] private SerializableInterface<ICombinableSO>[] _combinableTo;
	ICombinableSO[] ICombinableSO._combinableTo =>
		Enumerable.Select(
			this._combinableTo,
			(SerializableInterface<ICombinableSO> curCombinable, int idx) => curCombinable.Value
		).ToArray();
	[SerializeField] private SerializableInterface<IConverterSO> _requiredConverter;
	IConverterSO ICombinableSO._requiredConverter => this._requiredConverter.Value;
	[SerializeField] private Sprite _displaySprite;
	Sprite ICombinableSO.DisplaySprite => this._displaySprite;
	[SerializeField] private bool _isFinalCombinable;
	public bool IsFinalCombinable => this._isFinalCombinable;
	//public bool IsBaseCombinable => ((ICombinableSO)this).IsBaseCombinable;


	private List<string> _strKeyList = null;
	List<string> ICombinableSO.StrKeyList {
		get {
			if (this._strKeyList == null)
				this.StrKeyInit();
			return this._strKeyList;
		}
		set { this._strKeyList = value; }
	}
	private string _strKey = null;
	string ICombinableSO.StrKey {
		get {
			if (this._strKey == "" || this._strKey == null) // default value changes sometimes(??)
				this.StrKeyInit();
			return this._strKey;
		}
		set { this._strKey = value; }
	}
	string IInteractableSO.StrKey => ((ICombinableSO)this).StrKey;


	HashSet<ICombinableSO> ICombinableSO._existingCombinableTo { get; } = new HashSet<ICombinableSO>();
	HashSet<IHolderSO> IGrabbableSO._existingPlaceableTo { get; } = new HashSet<IHolderSO>();


	// TODO: prevent stack overflow by log error on cyclic referencing/reaching max recursion on calling StrKey
	protected void StrKeyInit() {
		if (this._strKeyList != null && this._strKey != null && this._strKey != "")
			return;

		if (((ICombinableSO)this).IsBaseCombinable) {
			this._strKeyList = new List<string>(1) { this.name };
			this._strKey = this.name;
			return;
		}


		ICombinableSO.GetRequiredBaseSO(new List<ICombinableSO>() { this }, out List<ICombinableSO> requiredBaseCombinables, out List<IConverterSO> requiredConverters);
		List<string> _strKeyList = new List<string>(requiredBaseCombinables.Count + requiredConverters.Count - 1);

		foreach (ICombinableSO curCombinable in requiredBaseCombinables)
			//if (curCombinable != this)
			_strKeyList.Add(curCombinable.StrKey);
		foreach (IConverterSO curUsableHolder in requiredConverters)
			_strKeyList.Add(curUsableHolder.StrKey);

		_strKeyList.Sort(); // assume no strings with same hashcode but different characters exist

		this._strKeyList = _strKeyList;
		this._strKey = String.Concat(this._strKeyList);
	}
}