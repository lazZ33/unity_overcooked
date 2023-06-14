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
    [SerializeField] private StationeryUtilitySO RequiredStationeryUtility;
    
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

    public bool IsBaseCombinable => this.RequiredCombinables.Length == 0 && this.RequiredStationeryUtility == null;
    public bool CanCombineWith(CombinableSO targetCombinableSO) => this._existingCombinableTo.Contains(targetCombinableSO);
    public static new CombinableSO GetSO(string strKey) => (CombinableSO)InteractableSO.GetSO(strKey);
    public static new CombinableSO TryGetSO(string strKey) => (CombinableSO)InteractableSO.TryGetSO(strKey);

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
    public static string GetNextSOStrKey(CombinableSO CombinableInfo, StationeryUtilitySO stationeryUtilityInfo){
        List<string> strList = new List<string>(CombinableInfo.StrKeyList){stationeryUtilityInfo.StrKey};
        strList.Sort();
        return String.Concat(strList);
    }

    public static void LoadAllRequiredSO(IEnumerable<CombinableSO> providedCombinables, List<StationeryUtilitySO> providedStationeryUtilities){

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

                foreach (StationeryUtilitySO curStationeryUtilitySO in providedStationeryUtilities){
                    if (!curCombinable1.CanPlaceOn(curStationeryUtilitySO)) continue;

                    newCombinable = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curCombinable1, curStationeryUtilitySO));
                    if (newCombinable == null) continue;

                    newCombinableSet.Add(newCombinable);
                    curCombinable1._existingPlaceableTo.Add(curStationeryUtilitySO);
                    
                    if (curCombinable1.CanPlaceOn(curStationeryUtilitySO.BindingUtensil))
                        curCombinable1._existingComtapibleUtensil.Add(curStationeryUtilitySO.BindingUtensil);

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

    public static void IdentifyRequiredBaseSO(IEnumerable<CombinableSO> targetCombinables, out List<CombinableSO> requiredBaseCombinables, out List<StationeryUtilitySO> requiredStationeryUtilities){
        // init hashset
        HashSet<CombinableSO> newCombinableSet = new HashSet<CombinableSO>();
        HashSet<CombinableSO> curCombinableSet = new HashSet<CombinableSO>();
        HashSet<CombinableSO> existingCombinableSet = new HashSet<CombinableSO>();
        HashSet<StationeryUtilitySO> existingStationeryUtilitySet = new HashSet<StationeryUtilitySO>();
        HashSet<CombinableSO> tempCombinableSetBuffer;
        foreach (CombinableSO curCombinable in targetCombinables){
            // clear previous relationships, insert initial CombinableSO
            curCombinable._existingCombinableTo.Clear();
            newCombinableSet.Add(curCombinable);
            if (curCombinable.RequiredStationeryUtility != null)
                existingStationeryUtilitySet.Add(curCombinable.RequiredStationeryUtility);
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
                    if (curCombinable.RequiredStationeryUtility != null)
                        existingStationeryUtilitySet.Add(curCombinable.RequiredStationeryUtility);
                }
        }

        requiredBaseCombinables = new List<CombinableSO>(existingCombinableSet.Where(SO => SO.IsBaseCombinable));
        requiredStationeryUtilities = new List<StationeryUtilitySO>(existingStationeryUtilitySet);
        HelperFunc.LogEnumerable(targetCombinables);
        HelperFunc.LogEnumerable(requiredBaseCombinables);
        return;
    }

    private void StrKeyInit(){
        Debug.Log(this.name + ": " + this._strKey);
        if (this._strKeyList != null && this._strKey != null && this._strKey != "") return;

        if (RequiredCombinables == null | RequiredCombinables.Length == 0){
            this._strKeyList = new List<string>(1){this.name};
            this._strKey = this.name;
            return;
        }
        
        CombinableSO.IdentifyRequiredBaseSO(new List<CombinableSO>(){this}, out List<CombinableSO> requiredBaseCombinables, out List<StationeryUtilitySO> requiredStationeryUtilities);
        List<string> strKeyList = new List<string>(requiredBaseCombinables.Count + requiredStationeryUtilities.Count);

        foreach(CombinableSO curCombinable in requiredBaseCombinables)
            strKeyList.Add(curCombinable.StrKey);
        foreach(StationeryUtilitySO curStationeryUtility in requiredStationeryUtilities)
            strKeyList.Add(curStationeryUtility.StrKey);

        strKeyList.Sort(); // assume no strings with same hashcode but different characters exist

        this._strKeyList = strKeyList;
        this._strKey = String.Concat(this._strKeyList);
        Debug.Log(this._strKey);
    }

    // public static void IdentifyRequiredSO(IEnumerable<CombinableSO> targetCombinables, out List<CombinableSO> requiredCombinable, out List<StationeryUtilitySO> requiredStationeryUtilities){

    //     requiredCombinable = CombinableSO.IdentifyRequiredBaseCombinables(targetCombinables);
    //     // init hashset
    //     // Pair<CombinableSO, List of required stationery utility to make that combinable>
    //     Dictionary<CombinableSO, List<StationeryUtilitySO>> newCombinableDict = new Dictionary<CombinableSO, List<StationeryUtilitySO>>();
    //     Dictionary<CombinableSO, List<StationeryUtilitySO>> curCombinableDict = new Dictionary<CombinableSO, List<StationeryUtilitySO>>();
    //     Dictionary<CombinableSO, List<StationeryUtilitySO>> existingCombinableDict = new Dictionary<CombinableSO, List<StationeryUtilitySO>>();
    //     Dictionary<CombinableSO, List<StationeryUtilitySO>> tempCombinableDictBuffer;
    //     foreach (CombinableSO curCombinableSO in requiredCombinable){
    //         // clear previous relationships, insert initial CombinableSO
    //         curCombinableSO._existingCombinableTo.Clear();
    //         List<StationeryUtilitySO> newStationeryUtilityDependencyList = new List<StationeryUtilitySO>(){};
    //         if (curCombinableSO.RequiredStationeryUtility != null) newStationeryUtilityDependencyList.Add(curCombinableSO.RequiredStationeryUtility);

    //         newCombinableDict.Add(curCombinableSO, newStationeryUtilityDependencyList);
    //     }

    //     while (newCombinableDict.Count != 0){
    //         // update current looping list
    //         foreach(KeyValuePair<CombinableSO, List<StationeryUtilitySO>> newPair in newCombinableDict){
    //             existingCombinableDict.Add(newPair.Key, newPair.Value);
    //             Debug.Log(newPair.Key);
    //             HelperFunc.LogEnumerable<StationeryUtilitySO>(newPair.Value);
    //         }
    //         curCombinableDict.Clear();
    //         tempCombinableDictBuffer = newCombinableDict;
    //         newCombinableDict = curCombinableDict;
    //         curCombinableDict = tempCombinableDictBuffer;

    //         // checking and updating relationships
    //         foreach (KeyValuePair<CombinableSO, List<StationeryUtilitySO>> curPair1 in curCombinableDict){
    //         foreach (KeyValuePair<CombinableSO, List<StationeryUtilitySO>> curPair2 in curCombinableDict){

    //             CombinableSO newCombinableSO;
    //             List<StationeryUtilitySO> newStationeryUtilityDependencies = new List<StationeryUtilitySO>(curPair1.Value);

    //             foreach (HolderSO curHolderSO in curPair1.Key._placeableTo){
    //                 switch (curHolderSO){
    //                     case StationeryUtilitySO curStationeryUtilitySO:
    //                         newCombinableSO = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curPair1.Key, curStationeryUtilitySO));
    //                         if (newCombinableSO == null) break;

    //                         newStationeryUtilityDependencies.Add(curStationeryUtilitySO);

    //                         newCombinableDict.TryAdd(newCombinableSO, newStationeryUtilityDependencies);
    //                         break;
    //                     default:
    //                         break;
    //                 }
    //             }

    //             foreach(UtensilSO curUtensilSO in curPair1.Key._compatibleUtensils){
    //                 StationeryUtilitySO bindingStationeryUtility = curUtensilSO.BindingStationeryUtility;
    //                 newCombinableSO = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curPair1.Key, bindingStationeryUtility));
    //                 if (newCombinableSO == null) break;

    //                 newStationeryUtilityDependencies.Add(bindingStationeryUtility);
                    
    //                 newCombinableDict.TryAdd(newCombinableSO, newStationeryUtilityDependencies);
    //                 break;
    //             }

    //             if (curPair1.Key._combinableTo.Contains(curPair2.Key)){
    //                 string newCombinableStrKey = CombinableSO.GetNextSOStrKey(curPair1.Key, curPair2.Key);
    //                 newCombinableSO = CombinableSO.TryGetSO(CombinableSO.GetNextSOStrKey(curPair1.Key, curPair2.Key));
    //                 if (newCombinableSO == null) { Debug.LogError("_combinableTo contains SO that related (next) SO cannot be loaded/cannot be combinded, error strKey: " + newCombinableStrKey); break; }
                    
    //                 newStationeryUtilityDependencies.AddRange(curPair2.Value);

    //                 newCombinableDict.TryAdd(newCombinableSO, newStationeryUtilityDependencies);
    //             }
    //         }}
    //     }

    //     // check for utilities that can lead to the targetCombinables
    //     requiredStationeryUtilities = new List<StationeryUtilitySO>();
    //     foreach (KeyValuePair<CombinableSO, List<StationeryUtilitySO>> targetPair in existingCombinableDict.Where(pair => targetCombinables.Contains(pair.Key))){
    //         foreach (StationeryUtilitySO requiredStationeryUtility in targetPair.Value){
    //             if (!requiredStationeryUtilities.Contains(requiredStationeryUtility))
    //                 requiredStationeryUtilities.Add(requiredStationeryUtility);
    //         }
    //     }
    //     List<StationeryUtilitySO> requiredStationeryUtilitiesCopy = new List<StationeryUtilitySO>(requiredStationeryUtilities); // for using it in lambda func

    //     // flush key and SO pair into _existingCombinableTo and _existingPlaceableTo 
    //     IEnumerable<KeyValuePair<CombinableSO, List<StationeryUtilitySO>>> existingCombinablePairs = existingCombinableDict.Where(
    //         pair => 
    //         pair.Value.All(SO => requiredStationeryUtilitiesCopy.Contains(SO)) | pair.Value.Count == 0);

    //     foreach (KeyValuePair<CombinableSO, List<StationeryUtilitySO>> curPair in existingCombinablePairs){
    //         // register every possible CombinableSO
    //         InteractableSO._existingInteractable.Add(curPair.Key.StrKey, curPair.Key);

    //         IEnumerable<KeyValuePair<CombinableSO, List<StationeryUtilitySO>>> existingCombinablePairsToCurPair = existingCombinablePairs.Where(SO => existingCombinablePairs.Contains(SO));
    //         foreach (KeyValuePair<CombinableSO, List<StationeryUtilitySO>> existingCombinablePair in existingCombinablePairs){
    //             curPair.Key._existingCombinableTo.Add(existingCombinablePair.Key);
    //         }
    //         IEnumerable<StationeryUtilitySO> existingStationeryUtilityPairsToCurPair = requiredStationeryUtilities.Where(SO => requiredStationeryUtilitiesCopy.Contains(SO));
    //         foreach(StationeryUtilitySO curStationeryUtility in existingStationeryUtilityPairsToCurPair){
    //             curPair.Key._existingPlaceableTo.Add(curStationeryUtility);
    //         }
    //     }
        
    //     // Add basic interactables
    //     InteractableSO._existingInteractable.Add("Table", InteractableSO.GetSO("Table"));
    //     InteractableSO._existingInteractable.Add("Spawn", InteractableSO.GetSO("Spawn"));
    // }
}