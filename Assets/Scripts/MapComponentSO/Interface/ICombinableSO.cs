using Unity;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public interface ICombinableSO: IGrabbableSO
{
	protected HashSet<ICombinableSO> _existingCombinableTo { get; }
	protected ICombinableSO[] _combinableTo { get; }
	public ICombinableSO[] RequiredCombinables { get; }
	protected IUsableSO _requiredConverter { get; }
	public Sprite DisplaySprite { get; }


	public new string StrKey { get; protected set; }
	public List<string> StrKeyList { get; protected set; }
	public string name { get; }


	public bool IsFinalCombinable { get; }
	public bool IsBaseCombinable
	{
		get
		{
			if (this.RequiredCombinables != null) return (this.RequiredCombinables.Length == 0 && this._requiredConverter == null);
			return true;
		}
	}
	public bool CanCombineWith(ICombinableSO targetCombinable) => this._existingCombinableTo.Contains(targetCombinable);
	public static ICombinableSO GetNextSO(ICombinableSO combinable1, ICombinableSO combinable2) => GetSO(GetNextSOStrKey(combinable1, combinable2));
	public static ICombinableSO GetNextSO(ICombinableSO combinable1, IUsableSO converter) => GetSO(GetNextSOStrKey(combinable1, converter));
	public static ICombinableSO TryGetNextSO(ICombinableSO combinable1, ICombinableSO combinable2) => TryGetSO(GetNextSOStrKey(combinable1, combinable2));
	public static ICombinableSO TryGetNextSO(ICombinableSO combinable1, IUsableSO converter) => TryGetSO(GetNextSOStrKey(combinable1, converter));
	private new static ICombinableSO GetSO(string strKey) => (ICombinableSO) IInteractableSO.GetSO(strKey);
	private new static ICombinableSO TryGetSO(string strKey) => (ICombinableSO) IInteractableSO.TryGetSO(strKey);

	private static string GetNextSOStrKey(ICombinableSO CombinableInfo1, ICombinableSO CombinableInfo2)
	{
		List<string> strList = new List<string>(CombinableInfo1.StrKeyList) { CombinableInfo2.StrKey };
		strList.Sort();
		return String.Concat(strList);
	}
	private static string GetNextSOStrKey(ICombinableSO CombinableInfo, IUsableSO converterInfo)
	{
		List<string> strList = new List<string>(CombinableInfo.StrKeyList) { converterInfo.StrKey };
		strList.Sort();
		return String.Concat(strList);
	}


	public static void LoadAllRequiredSO(IEnumerable<ICombinableSO> providedCombinables, List<IUsableSO> providedConverters)
	{

		InteractableSO.LoadAllSO();

		// init hashset
		HashSet<ICombinableSO> newCombinableSet = new HashSet<ICombinableSO>();
		HashSet<ICombinableSO> curCombinableSet = new HashSet<ICombinableSO>();
		HashSet<ICombinableSO> tempCombinableSetBuffer;
		HashSet<ICombinableSO> existingCombinableSet = new HashSet<ICombinableSO>();
		foreach (ICombinableSO curCombinableSO in providedCombinables)
		{
			// clear previous relationships, insert initial ICombinableSO
			curCombinableSO._existingCombinableTo.Clear();

			newCombinableSet.Add(curCombinableSO);
		}

		while (newCombinableSet.Count != 0)
		{
			// update current looping list
			foreach (ICombinableSO newCombinable in newCombinableSet)
			{
				existingCombinableSet.Add(newCombinable);
				IInteractableSO._existingInteractable.Add(newCombinable.StrKey, newCombinable);
				Debug.Log(newCombinable);
			}
			curCombinableSet.Clear();
			tempCombinableSetBuffer = newCombinableSet;
			newCombinableSet = curCombinableSet;
			curCombinableSet = tempCombinableSetBuffer;

			// checking and updating relationships
			foreach (ICombinableSO curCombinable1 in curCombinableSet)
			{
				ICombinableSO newCombinable;

				foreach (IUsableSO curConverter in providedConverters)
				{
					newCombinable = TryGetSO(GetNextSOStrKey(curCombinable1, curConverter));
					if (newCombinable == null) continue;

					newCombinableSet.Add(newCombinable);

					// TODO: can avoid reflection?
					switch (curConverter.GetType())
					{
						case IHolderSO curHolderConverter:
							curCombinable1._existingPlaceableTo.Add(curHolderConverter);
							if (curCombinable1.CanPlaceOn(curHolderConverter.BindingHolder))
								curCombinable1._existingPlaceableTo.Add(curHolderConverter.BindingHolder);
							break;
					}
				}

				// loop between combinables from curCombinableSet and existingCombinableSet
				foreach (ICombinableSO curCombinable2 in existingCombinableSet)
				{

					if (!curCombinable1._combinableTo.Contains(curCombinable2)) continue;
					curCombinable1._existingCombinableTo.Add(curCombinable2);
					if (!curCombinable2._combinableTo.Contains(curCombinable1))
					{
						Debug.LogError(String.Format("Combinable {0} and {1} not bidirectionally combinable, assuming it is. Combinable missing reference in _combinableTo: {1} ", curCombinable1.name, curCombinable2.name));
						curCombinable2._existingCombinableTo.Add(curCombinable1);
					}
					HelperFunc.LogEnumerable(curCombinable1._existingCombinableTo);

					string newCombinableStrKey = GetNextSOStrKey(curCombinable1, curCombinable2);
					newCombinable = TryGetSO(GetNextSOStrKey(curCombinable1, curCombinable2));
					if (newCombinable == null) { Debug.LogError("_combinableTo contains SO that related (next) SO cannot be loaded/cannot be combinded, error strKey: " + newCombinableStrKey); break; }

					newCombinableSet.Add(newCombinable);
				}

				// loop between 2 combinables from curCombinableSet
				foreach (ICombinableSO curCombinable2 in curCombinableSet)
				{
					//Debug.Log(curCombinable1.name + " " + curCombinable2);

					if (!curCombinable1._combinableTo.Contains(curCombinable2)) continue;
					curCombinable1._existingCombinableTo.Add(curCombinable2);
					if (!curCombinable2._combinableTo.Contains(curCombinable1))
					{
						Debug.LogError(String.Format("Combinable {0} and {1} not bidirectionally combinable, assuming it is. Combinable missing reference in _combinableTo: {1} ", curCombinable1.name, curCombinable2.name));
						curCombinable2._existingCombinableTo.Add(curCombinable1);
					}
					HelperFunc.LogEnumerable(curCombinable1._existingCombinableTo);

					string newCombinableStrKey = GetNextSOStrKey(curCombinable1, curCombinable2);
					newCombinable = TryGetSO(GetNextSOStrKey(curCombinable1, curCombinable2));
					if (newCombinable == null) { Debug.LogError("_combinableTo contains SO that related (next) SO cannot be loaded/cannot be combinded, error strKey: " + newCombinableStrKey); break; }

					newCombinableSet.Add(newCombinable);
				}
			}
		}

		// Add basic interactables
		// TODO: try not to hard code?
		_existingInteractable.Add("Table", GetSO("Table"));
		_existingInteractable.Add("Spawn", GetSO("Spawn"));
		_existingInteractable.Add("DishExit", GetSO("DishExit"));

		InteractableSO.UnloadAllSO();

		HelperFunc.LogEnumerable(_existingInteractable.Values);
		foreach (ICombinableSO curCombinable in existingCombinableSet)
		{
			HelperFunc.LogEnumerable(curCombinable.RequiredCombinables);
		}
	}

	public static void GetRequiredBaseSO(IEnumerable<ICombinableSO> targetCombinables, out List<ICombinableSO> requiredBaseCombinables, out List<IUsableSO> requiredConverters)
	{
		if (targetCombinables.All(e => e.IsBaseCombinable))
		{
			requiredBaseCombinables = new List<ICombinableSO>(targetCombinables);
			requiredConverters = new List<IUsableSO>(0);
			return;
		}

		// init hashset
		HashSet<ICombinableSO> newCombinableSet = new HashSet<ICombinableSO>();
		HashSet<ICombinableSO> curCombinableSet = new HashSet<ICombinableSO>();
		HashSet<ICombinableSO> existingCombinableSet = new HashSet<ICombinableSO>();
		HashSet<IUsableSO> existingUsableHolderSet = new HashSet<IUsableSO>();
		HashSet<ICombinableSO> tempCombinableSetBuffer;
		foreach (ICombinableSO curCombinable in targetCombinables)
		{
			// clear previous relationships, insert initial ICombinableSO
			curCombinable._existingCombinableTo.Clear();
			newCombinableSet.Add(curCombinable);
			if (curCombinable._requiredConverter != null)
				existingUsableHolderSet.Add(curCombinable._requiredConverter);
		}

		while (newCombinableSet.Count != 0)
		{
			// update cur looping list
			curCombinableSet.Clear();
			foreach (ICombinableSO newCombinable in newCombinableSet) existingCombinableSet.Add(newCombinable);
			tempCombinableSetBuffer = newCombinableSet;
			newCombinableSet = curCombinableSet;
			curCombinableSet = tempCombinableSetBuffer;

			// checking and updating relationships
			foreach (ICombinableSO curCombinable in curCombinableSet)
			{
				if (curCombinable.IsBaseCombinable) continue;
				foreach (ICombinableSO containedCombinable in curCombinable.RequiredCombinables)
				{
					newCombinableSet.Add(containedCombinable);
					if (curCombinable._requiredConverter != null)
						existingUsableHolderSet.Add(curCombinable._requiredConverter);
				}
			}
		}

		requiredBaseCombinables = new List<ICombinableSO>(existingCombinableSet.Where(SO => SO.IsBaseCombinable));
		requiredConverters = new List<IUsableSO>(existingUsableHolderSet);
		return;
	}
}