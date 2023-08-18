using Unity;
using UnityEngine;
using System;
using System.Collections.Generic;
using TNRD;

[Serializable]
[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObject/Ingredient")]
public class IngredientSO : InteractableSO, ICombinableSO
{
	// ICombinableSO implementation
	[SerializeField] private ICombinableSO[] _requiredCombinables;
	ICombinableSO[] ICombinableSO.RequiredCombinables => this._requiredCombinables;
	[SerializeField] private SerializableInterface<ICombinableSO[]> _combinableTo;
	ICombinableSO[] ICombinableSO._combinableTo => this._combinableTo.Value;
	[SerializeField] private SerializableInterface<IUsableSO> _requiredConverter;
	IUsableSO ICombinableSO._requiredConverter => this._requiredConverter.Value;
	[SerializeField] private Sprite _displaySprite;
	Sprite ICombinableSO.DisplaySprite => this._displaySprite;
	[SerializeField] private bool _isFinalCombinable;
	public bool IsFinalCombinable => this._isFinalCombinable;
	public bool IsBaseCombinable => ((ICombinableSO)this).IsBaseCombinable;


	private List<string> _strKeyList;
	List<string> ICombinableSO.StrKeyList
	{
		get
		{
			if (this._strKeyList == null)
				this.StrKeyInit();
			return this._strKeyList;
		}
		set { this._strKeyList = value; }
	}
	private string _strKey;
	string ICombinableSO.StrKey { 
		get
		{
			if (this._strKey == null)
				this.StrKeyInit();
			return this._strKey;
		} 
		set { this._strKey = value; }
	}
	

	HashSet<ICombinableSO> ICombinableSO._existingCombinableTo { get; } = new HashSet<ICombinableSO>();
	HashSet<IHolderSO> IGrabbableSO._existingPlaceableTo { get; } = new HashSet<IHolderSO>();


	// TODO: prevent stack overflow by log error on cyclic referencing/reaching max recursion on calling StrKey
	protected void StrKeyInit()
	{
		if (this._strKeyList != null && this._strKey != null && this._strKey != "") return;
		Debug.Log(this.name + " _strKeyInit");

		if (this.IsBaseCombinable)
		{
			this._strKeyList = new List<string>(1) { this.name };
			this._strKey = this.name;
			Debug.Log(this.name + ": " + this._strKey);
			return;
		}

		ICombinableSO.GetRequiredBaseSO(new List<ICombinableSO>() { this }, out List<ICombinableSO> requiredBaseCombinables, out List<IUsableSO> requiredConverters);
		List<string> _strKeyList = new List<string>(requiredBaseCombinables.Count + requiredConverters.Count - 1);

		foreach (ICombinableSO curCombinable in requiredBaseCombinables)
			//if (curCombinable != this)
			_strKeyList.Add(curCombinable.StrKey);
		foreach (IUsableSO curUsableHolder in requiredConverters)
			_strKeyList.Add(curUsableHolder.StrKey);

		_strKeyList.Sort(); // assume no strings with same hashcode but different characters exist

		this._strKeyList = _strKeyList;
		this._strKey = String.Concat(this._strKeyList);
		Debug.Log(this.name + ": " + this._strKey);
	}
}