using Unity;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
[CreateAssetMenu(fileName = "StatioenryUtility", menuName = "ScriptableObject/StatioenryUtility")]
public class StationeryUtilitySO: HolderSO{
    [SerializeField] public string UsableName;
    [SerializeField] public bool HoldToUse;
    [SerializeField] public int UseTime;
    [SerializeField] private List<GrabbableSO> _usableTo = new List<GrabbableSO>();

    private List<GrabbableSO> _existingUsableTo = new List<GrabbableSO>();

    public bool CanUseOn(GrabbableSO targetGrabbableSO){
        return this._existingUsableTo.Contains(targetGrabbableSO);
    }

    public void RegisterObject(){
        Debug.Log("Registering HolderSO: " + this.name);
        InteractableSO.ExistingInteractable.Add(this);
        GrabbableSO.UpdateAllRelationships(); // assume each type of grabbable can only processed by 1 stationeryUtility

        foreach (InteractableSO curExistingInteractableSO in InteractableSO.ExistingInteractable){
            switch (curExistingInteractableSO){
                case GrabbableSO curExistingGrabbableSO:
                    if (this._usableTo.Contains(curExistingGrabbableSO)){
                        this._existingUsableTo.Add(curExistingGrabbableSO);
                    }
                    break;
            }
        }
    }

}