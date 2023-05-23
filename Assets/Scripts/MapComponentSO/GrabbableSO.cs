using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


/// <summary>
/// Responsible for storing information about grabbable objects
/// <summary/>
[Serializable]
public abstract class GrabbableSO: InteractableSO{

    private static Dictionary<string, GrabbableSO> NextGrabbableDict = new Dictionary<string, GrabbableSO>();
    
    [SerializeField] public bool SelfUse = false;
    [SerializeField] private List<InteractableSO> _placeableTo = new List<InteractableSO>();
    public List<string> ContainedGrabbableNames { get; } = new List<string>();
    public List<string> RequiredUtilityNames { get; } = new List<string>();
    public List<string> StrKeyList { get{
                            List<string> strList = new List<string>(this.ContainedGrabbableNames.Concat(this.RequiredUtilityNames));
                            strList.Sort(); // assume no strings with same hashcode but different characters exist
                            return strList;
                            }
                        }
    // should be equal to filename after String.Concat() both name list
    private HashSet<InteractableSO> _existingPlaceableTo { get; } = new HashSet<InteractableSO>();

    void OnEnable(){
        // update name of this SO
        this.ContainedGrabbableNames.Clear();
        List<string> fileNameList = new List<string>(Regex.Split(this.name, @"(?<!^)(?=[A-Z])"));
        this.ContainedGrabbableNames.AddRange(fileNameList);
    }

    public bool CanPlaceOn(InteractableSO targetInteractableSO){
        return this._existingPlaceableTo.Contains(targetInteractableSO);
    }

    // public static GrabbableSO getNextSO(GrabbableSO info1, GrabbableSO info2){
    //     List<string> strList = new List<string>(info1.ContainedGrabbableNames.Concat(info2.ContainedGrabbableNames));
    //     strList.Sort();
    //     return GetSO(String.Concat(strList));
    // }
    public static string getNextSOStrKey(GrabbableSO info1, GrabbableSO info2){
        List<string> strList = new List<string>(info1.ContainedGrabbableNames.Concat(info2.ContainedGrabbableNames));
        strList.Sort();
        return String.Concat(strList);
    }
    public static GrabbableSO GetSO(string strKey){
        return GrabbableSO.NextGrabbableDict[strKey];
    }

    // update existing using/placing relationship for current SO
    // should be called after any UtilitySO's RegisterObject()
    public void RegisterObject(){
        Debug.Log("Registering GrabbableSO: " + this.name);
        InteractableSO.ExistingInteractable.Add(this);
        GrabbableSO.NextGrabbableDict.Add(this.name, this);

        this.UpdateRelationships();
    }

    public void UpdateRelationships(){
        // clear previous relationships
        this._existingPlaceableTo.Clear();

        // init hashset
        HashSet<Tuple<string, GrabbableSO>> newGrabbableList = new HashSet<Tuple<string, GrabbableSO>>();
        HashSet<GrabbableSO> curGrabbableList = new HashSet<GrabbableSO>();
        newGrabbableList.Add(new Tuple<string, GrabbableSO>(this.name, this));

        while (newGrabbableList.Count != 0){
            // update current looping list
            curGrabbableList.Clear();
            foreach ( Tuple<string, GrabbableSO> newGrabbable in newGrabbableList ){
                curGrabbableList.Add(newGrabbable.Item2);
            }
            newGrabbableList.Clear();

            // checking and updating relationships
            foreach ( GrabbableSO curGrabbableSO in curGrabbableList){
            foreach ( InteractableSO curExistingInteractableSO in InteractableSO.ExistingInteractable){


                switch (curExistingInteractableSO){

                    case GrabbableSO curExistingGrabbableSO:
                        if (curGrabbableSO._placeableTo.Contains(curExistingInteractableSO)){ do{
                            curGrabbableSO._existingPlaceableTo.Add(curExistingInteractableSO);

                            // attempt to load SO for the possible combination
                            string strKey = GrabbableSO.GetNewStrKey(curGrabbableSO, curExistingGrabbableSO);
                            GrabbableSO nextGrabbable = (GrabbableSO) Resources.Load("InteractableSO/Grabbable/" + strKey);
                            if (nextGrabbable == null){ Debug.LogError("SO resource failed to load, possibly corrupted filename or SO name list. Tried string key: " + strKey); break;} // break the inner do-while
                            
                            // buffer loaded SO into the list
                            newGrabbableList.Add(new Tuple<string, GrabbableSO>(strKey, nextGrabbable));

                            // // do the same for the curExistingGrabbableSO
                            if (curExistingGrabbableSO._placeableTo.Contains(curGrabbableSO)){
                                curExistingGrabbableSO._existingPlaceableTo.Add(curGrabbableSO);
                            }
                        } while(false); }
                        break;

                    case HolderSO curExistingHolderSO:

                        if (!curGrabbableSO._placeableTo.Contains(curExistingHolderSO)) continue;
                        curGrabbableSO._existingPlaceableTo.Add(curExistingHolderSO);
                            
                        switch (curExistingHolderSO){

                            case StationeryUtilitySO curExistingUtilitySO:
                                do{
                                // attempt to load SO for the possible combination
                                string strKey = GrabbableSO.GetNewStrKey(curGrabbableSO, curExistingUtilitySO);
                                GrabbableSO nextGrabbable = (GrabbableSO) Resources.Load("InteractableSO/Grabbable/" + strKey);
                                if (nextGrabbable == null){ Debug.LogError("SO resource failed to load, possibly corrupted filename or SO name list. Tried string key: " + strKey); break;} // break the inner do-while
                                
                                // buffer loaded SO into the list
                                newGrabbableList.Add(new Tuple<string, GrabbableSO>(strKey, nextGrabbable));
                                }while(false);
                                break;
                            
                            // TODO: add table into every grabbable's existingPlaceable list?
                            case TableSO curExistingTableSo:
                                break;

                        }
                        break;
                }

            }

            // flush key and SO pair into existingInteractable and NextGrabbableDict
            foreach (Tuple<string, GrabbableSO> keySO in newGrabbableList){
                InteractableSO.ExistingInteractable.Add(keySO.Item2);
                GrabbableSO.NextGrabbableDict.Add(keySO.Item1, keySO.Item2);
            }
        }}
        foreach (KeyValuePair<string,GrabbableSO> keySO in NextGrabbableDict){
            Debug.Log(keySO.Key + " " + keySO.Value.ToString());
        }
    }

    public static void UpdateAllRelationships(){
        foreach (GrabbableSO grabbableSO in NextGrabbableDict.Values){
            grabbableSO.UpdateRelationships();
        }
    }

    private static string GetNewStrKey(GrabbableSO SO1, GrabbableSO SO2){
        List<string> strKeyList = new List<string>(SO1.StrKeyList.Concat(SO2.StrKeyList));
        strKeyList.Sort();
        return String.Concat(strKeyList);
    }
    private static string GetNewStrKey(GrabbableSO SO1, StationeryUtilitySO SO2){
        List<string> strKeyList = SO1.StrKeyList;
        strKeyList.Add(SO2.name);
        strKeyList.Sort();
        return String.Concat(strKeyList);
    }
}