using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public abstract class CombinableSO: GrabbableSO{
    
    [SerializeField] private CombinableSO[] _combinableTo;
    [SerializeField] private CombinableSO[] RequiredCombinables;
    [SerializeField] private UsableHolderSO RequiredUsableHolder;
    [SerializeField] private bool _isFinalCombinable;
    
    public List<string> StrKeyList { get {
        if (this._strKeyList == null){
            this.StrKeyInit();
        }
        return this._strKeyList;
    }}
    private List<string> _strKeyList = null;
    public override string StrKey { get{
        if (this._strKey == null | this._strKey == ""){
            this.StrKeyInit();
        }
        return this._strKey;
    }}
    private string _strKey = null;
    private HashSet<CombinableSO> _existingCombinableTo { get; } = new HashSet<CombinableSO>();

    public bool IsFinalCombinable => this._isFinalCombinable;
    public bool CanCombineWith(CombinableSO targetCombinableSO) => this._existingCombinableTo.Contains(targetCombinableSO);
    public static new CombinableSO GetSO(string strKey) => (CombinableSO)InteractableSO.GetSO(strKey);
    public static new CombinableSO TryGetSO(string strKey) => (CombinableSO)InteractableSO.TryGetSO(strKey);

    private bool IsBaseCombinable => this.RequiredCombinables.Length == 0 && this.RequiredUsableHolder == null;

    protected override void OnEnable(){
        base.OnEnable();
        this._existingCombinableTo.Clear();

        this.StrKeyInit();
    }

    public static string GetNextSOStrKey(CombinableSO info1, CombinableSO info2){
        List<string> strList = new List<string>(info1.StrKeyList.Concat(info2.StrKeyList));
        strList.Sort();
        return String.Concat(strList);
    }
    public static string GetNextSOStrKey(CombinableSO CombinableInfo, UsableHolderSO stationeryUtilityInfo){
        List<string> strList = new List<string>(CombinableInfo.StrKeyList){stationeryUtilityInfo.StrKey};
        strList.Sort();
        return String.Concat(strList);
    }

    public static void LoadAllRequiredSO(IEnumerable<CombinableSO> providedCombinables, List<UsableHolderSO> providedStationeryUtilities){

        InteractableSO.LoadAllSO();

        // init hashset
        HashSet<CombinableSO> newCombinableSet = new HashSet<CombinableSO>();
        HashSet<CombinableSO> curCombinableSet = new HashSet<CombinableSO>();
        HashSet<CombinableSO> tempCombinableSetBuffer;
        HashSet<CombinableSO> existingCombinableSet = new HashSet<CombinableSO>();
        foreach (CombinableSO curCombinableSO in providedCombinables){
            // clear previous relationships, insert initial CombinableSO
            curCombinableSO._existingCombinableTo.Clear();

            newCombinableSet.Add(curCombinableSO);
        }

        while (newCombinableSet.Count != 0){
            // update current looping list
            foreach(CombinableSO newCombinable in newCombinableSet){
                existingCombinableSet.Add(newCombinable);
                InteractableSO._existingInteractable.Add(newCombinable.StrKey, newCombinable);
                Debug.Log(newCombinable);
            }
            curCombinableSet.Clear();
            tempCombinableSetBuffer = newCombinableSet;
            newCombinableSet = curCombinableSet;
            curCombinableSet = tempCombinableSetBuffer;

            // checking and updating relationships
            foreach (CombinableSO curCombinable1 in curCombinableSet){
                CombinableSO newCombinable;

                foreach (UsableHolderSO curUsableHolderSO in providedStationeryUtilities){
                    if (!curCombinable1.CanPlaceOn(curUsableHolderSO)) continue;

                    newCombinable = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curCombinable1, curUsableHolderSO));
                    if (newCombinable == null) continue;

                    newCombinableSet.Add(newCombinable);
                    curCombinable1._existingPlaceableTo.Add(curUsableHolderSO);
                    
                    if (curCombinable1.CanPlaceOn(curUsableHolderSO.BindingUtensil))
                        curCombinable1._existingComtapibleUtensil.Add(curUsableHolderSO.BindingUtensil);

                }

                // loop between combinables from curCombinableSet and existingCombinableSet
                foreach (CombinableSO curCombinable2 in existingCombinableSet){

                    if (!curCombinable1._combinableTo.Contains(curCombinable2)) continue;
                    curCombinable1._existingCombinableTo.Add(curCombinable2);
                    if (!curCombinable2._combinableTo.Contains(curCombinable1)){
                        Debug.LogError(String.Format("Combinable {0} and {1} not bidirectionally combinable, assuming it is. Combinable missing reference in _combinableTo: {1} ", curCombinable1.name, curCombinable2.name));
                        curCombinable2._existingCombinableTo.Add(curCombinable1);
                    }
                    HelperFunc.LogEnumerable(curCombinable1._existingCombinableTo);

                    string newCombinableStrKey = CombinableSO.GetNextSOStrKey(curCombinable1, curCombinable2);
                    newCombinable = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curCombinable1, curCombinable2));
                    if (newCombinable == null) { Debug.LogError("_combinableTo contains SO that related (next) SO cannot be loaded/cannot be combinded, error strKey: " + newCombinableStrKey); break; }
                    
                    newCombinableSet.Add(newCombinable);
                }

                // loop between 2 combinables from curCombinableSet
                foreach (CombinableSO curCombinable2 in curCombinableSet){
                    Debug.Log(curCombinable1.name + " " + curCombinable2);

                    if (!curCombinable1._combinableTo.Contains(curCombinable2)) continue;
                    curCombinable1._existingCombinableTo.Add(curCombinable2);
                    if (!curCombinable2._combinableTo.Contains(curCombinable1)){
                        Debug.LogError(String.Format("Combinable {0} and {1} not bidirectionally combinable, assuming it is. Combinable missing reference in _combinableTo: {1} ", curCombinable1.name, curCombinable2.name));
                        curCombinable2._existingCombinableTo.Add(curCombinable1);
                    }
                    HelperFunc.LogEnumerable(curCombinable1._existingCombinableTo);

                    string newCombinableStrKey = CombinableSO.GetNextSOStrKey(curCombinable1, curCombinable2);
                    newCombinable = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curCombinable1, curCombinable2));
                    if (newCombinable == null) { Debug.LogError("_combinableTo contains SO that related (next) SO cannot be loaded/cannot be combinded, error strKey: " + newCombinableStrKey); break; }
                    
                    newCombinableSet.Add(newCombinable);
                }
            }
        }
        
        // Add basic interactables
        InteractableSO._existingInteractable.Add("Table", InteractableSO.GetSO("Table"));
        InteractableSO._existingInteractable.Add("Spawn", InteractableSO.GetSO("Spawn"));

        InteractableSO.UnloadAllSO();

        HelperFunc.LogEnumerable(_existingInteractable.Values);
        foreach(CombinableSO curCombinable in existingCombinableSet){
            HelperFunc.LogEnumerable(curCombinable.RequiredCombinables);
        }
    }

    public static void GetRequiredBaseSO(IEnumerable<CombinableSO> targetCombinables, out List<CombinableSO> requiredBaseCombinables, out List<UsableHolderSO> requiredStationeryUtilities){
        // init hashset
        HashSet<CombinableSO> newCombinableSet = new HashSet<CombinableSO>();
        HashSet<CombinableSO> curCombinableSet = new HashSet<CombinableSO>();
        HashSet<CombinableSO> existingCombinableSet = new HashSet<CombinableSO>();
        HashSet<UsableHolderSO> existingUsableHolderSet = new HashSet<UsableHolderSO>();
        HashSet<CombinableSO> tempCombinableSetBuffer;
        foreach (CombinableSO curCombinable in targetCombinables){
            // clear previous relationships, insert initial CombinableSO
            curCombinable._existingCombinableTo.Clear();
            newCombinableSet.Add(curCombinable);
            if (curCombinable.RequiredUsableHolder != null)
                existingUsableHolderSet.Add(curCombinable.RequiredUsableHolder);
        }

        while (newCombinableSet.Count != 0){
            // update cur looping list
            curCombinableSet.Clear();
            foreach(CombinableSO newCombinable in newCombinableSet) existingCombinableSet.Add(newCombinable);
            tempCombinableSetBuffer = newCombinableSet;
            newCombinableSet = curCombinableSet;
            curCombinableSet = tempCombinableSetBuffer;

            // checking and updating relationships
            foreach (CombinableSO curCombinable in curCombinableSet)
                foreach (CombinableSO containedCombinable in curCombinable.RequiredCombinables){
                    newCombinableSet.Add(containedCombinable);
                    if (curCombinable.RequiredUsableHolder != null)
                        existingUsableHolderSet.Add(curCombinable.RequiredUsableHolder);
                }
        }

        requiredBaseCombinables = new List<CombinableSO>(existingCombinableSet.Where(SO => SO.IsBaseCombinable));
        requiredStationeryUtilities = new List<UsableHolderSO>(existingUsableHolderSet);
        return;
    }

    // TODO: prevent stack overflow by log error on cyclic referencing/reaching max recursion on calling StrKey
    private void StrKeyInit(){
        if (this._strKeyList != null && this._strKey != null && this._strKey != "") return;
        Debug.Log(this.name + " StrKeyInit");

        if (this.IsBaseCombinable){
            this._strKeyList = new List<string>(1){this.name};
            this._strKey = this.name;
            Debug.Log(this.name + ": " + this._strKey);
            return;
        }
        
        CombinableSO.GetRequiredBaseSO(new List<CombinableSO>(){this}, out List<CombinableSO> requiredBaseCombinables, out List<UsableHolderSO> requiredStationeryUtilities);
        List<string> strKeyList = new List<string>(requiredBaseCombinables.Count + requiredStationeryUtilities.Count - 1);

        foreach(CombinableSO curCombinable in requiredBaseCombinables)
            if (curCombinable != this)
                strKeyList.Add(curCombinable.StrKey);
        foreach(UsableHolderSO curUsableHolder in requiredStationeryUtilities)
            strKeyList.Add(curUsableHolder.StrKey);

        strKeyList.Sort(); // assume no strings with same hashcode but different characters exist

        this._strKeyList = strKeyList;
        this._strKey = String.Concat(this._strKeyList);
        Debug.Log(this.name +  ": " + this._strKey);
    }

    // public static void IdentifyRequiredSO(IEnumerable<CombinableSO> targetCombinables, out List<CombinableSO> requiredCombinable, out List<UsableHolderSO> requiredStationeryUtilities){

    //     requiredCombinable = CombinableSO.IdentifyRequiredBaseCombinables(targetCombinables);
    //     // init hashset
    //     // Pair<CombinableSO, List of required stationery utility to make that combinable>
    //     Dictionary<CombinableSO, List<UsableHolderSO>> newCombinableDict = new Dictionary<CombinableSO, List<UsableHolderSO>>();
    //     Dictionary<CombinableSO, List<UsableHolderSO>> curCombinableDict = new Dictionary<CombinableSO, List<UsableHolderSO>>();
    //     Dictionary<CombinableSO, List<UsableHolderSO>> existingCombinableDict = new Dictionary<CombinableSO, List<UsableHolderSO>>();
    //     Dictionary<CombinableSO, List<UsableHolderSO>> tempCombinableDictBuffer;
    //     foreach (CombinableSO curCombinableSO in requiredCombinable){
    //         // clear previous relationships, insert initial CombinableSO
    //         curCombinableSO._existingCombinableTo.Clear();
    //         List<UsableHolderSO> newUsableHolderDependencyList = new List<UsableHolderSO>(){};
    //         if (curCombinableSO.RequiredUsableHolder != null) newUsableHolderDependencyList.Add(curCombinableSO.RequiredUsableHolder);

    //         newCombinableDict.Add(curCombinableSO, newUsableHolderDependencyList);
    //     }

    //     while (newCombinableDict.Count != 0){
    //         // update current looping list
    //         foreach(KeyValuePair<CombinableSO, List<UsableHolderSO>> newPair in newCombinableDict){
    //             existingCombinableDict.Add(newPair.Key, newPair.Value);
    //             Debug.Log(newPair.Key);
    //             HelperFunc.LogEnumerable<UsableHolderSO>(newPair.Value);
    //         }
    //         curCombinableDict.Clear();
    //         tempCombinableDictBuffer = newCombinableDict;
    //         newCombinableDict = curCombinableDict;
    //         curCombinableDict = tempCombinableDictBuffer;

    //         // checking and updating relationships
    //         foreach (KeyValuePair<CombinableSO, List<UsableHolderSO>> curPair1 in curCombinableDict){
    //         foreach (KeyValuePair<CombinableSO, List<UsableHolderSO>> curPair2 in curCombinableDict){

    //             CombinableSO newCombinableSO;
    //             List<UsableHolderSO> newUsableHolderDependencies = new List<UsableHolderSO>(curPair1.Value);

    //             foreach (HolderSO curHolderSO in curPair1.Key._placeableTo){
    //                 switch (curHolderSO){
    //                     case UsableHolderSO curUsableHolderSO:
    //                         newCombinableSO = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curPair1.Key, curUsableHolderSO));
    //                         if (newCombinableSO == null) break;

    //                         newUsableHolderDependencies.Add(curUsableHolderSO);

    //                         newCombinableDict.TryAdd(newCombinableSO, newUsableHolderDependencies);
    //                         break;
    //                     default:
    //                         break;
    //                 }
    //             }

    //             foreach(UtensilSO curUtensilSO in curPair1.Key._compatibleUtensils){
    //                 UsableHolderSO bindingUsableHolder = curUtensilSO.BindingUsableHolder;
    //                 newCombinableSO = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curPair1.Key, bindingUsableHolder));
    //                 if (newCombinableSO == null) break;

    //                 newUsableHolderDependencies.Add(bindingUsableHolder);
                    
    //                 newCombinableDict.TryAdd(newCombinableSO, newUsableHolderDependencies);
    //                 break;
    //             }

    //             if (curPair1.Key._combinableTo.Contains(curPair2.Key)){
    //                 string newCombinableStrKey = CombinableSO.GetNextSOStrKey(curPair1.Key, curPair2.Key);
    //                 newCombinableSO = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curPair1.Key, curPair2.Key));
    //                 if (newCombinableSO == null) { Debug.LogError("_combinableTo contains SO that related (next) SO cannot be loaded/cannot be combinded, error strKey: " + newCombinableStrKey); break; }
                    
    //                 newUsableHolderDependencies.AddRange(curPair2.Value);

    //                 newCombinableDict.TryAdd(newCombinableSO, newUsableHolderDependencies);
    //             }
    //         }}
    //     }

    //     // check for utilities that can lead to the targetCombinables
    //     requiredStationeryUtilities = new List<UsableHolderSO>();
    //     foreach (KeyValuePair<CombinableSO, List<UsableHolderSO>> targetPair in existingCombinableDict.Where(pair => targetCombinables.Contains(pair.Key))){
    //         foreach (UsableHolderSO requiredUsableHolder in targetPair.Value){
    //             if (!requiredStationeryUtilities.Contains(requiredUsableHolder))
    //                 requiredStationeryUtilities.Add(requiredUsableHolder);
    //         }
    //     }
    //     List<UsableHolderSO> requiredStationeryUtilitiesCopy = new List<UsableHolderSO>(requiredStationeryUtilities); // for using it in lambda func

    //     // flush key and SO pair into _existingCombinableTo and _existingPlaceableTo 
    //     IEnumerable<KeyValuePair<CombinableSO, List<UsableHolderSO>>> existingCombinablePairs = existingCombinableDict.Where(
    //         pair => 
    //         pair.Value.All(SO => requiredStationeryUtilitiesCopy.Contains(SO)) | pair.Value.Count == 0);

    //     foreach (KeyValuePair<CombinableSO, List<UsableHolderSO>> curPair in existingCombinablePairs){
    //         // register every possible CombinableSO
    //         InteractableSO._existingInteractable.Add(curPair.Key.StrKey, curPair.Key);

    //         IEnumerable<KeyValuePair<CombinableSO, List<UsableHolderSO>>> existingCombinablePairsToCurPair = existingCombinablePairs.Where(SO => existingCombinablePairs.Contains(SO));
    //         foreach (KeyValuePair<CombinableSO, List<UsableHolderSO>> existingCombinablePair in existingCombinablePairs){
    //             curPair.Key._existingCombinableTo.Add(existingCombinablePair.Key);
    //         }
    //         IEnumerable<UsableHolderSO> existingUsableHolderPairsToCurPair = requiredStationeryUtilities.Where(SO => requiredStationeryUtilitiesCopy.Contains(SO));
    //         foreach(UsableHolderSO curUsableHolder in existingUsableHolderPairsToCurPair){
    //             curPair.Key._existingPlaceableTo.Add(curUsableHolder);
    //         }
    //     }
        
    //     // Add basic interactables
    //     InteractableSO._existingInteractable.Add("Table", InteractableSO.GetSO("Table"));
    //     InteractableSO._existingInteractable.Add("Spawn", InteractableSO.GetSO("Spawn"));
    // }
}