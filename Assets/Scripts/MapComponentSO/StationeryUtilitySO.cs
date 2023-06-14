using Unity;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
[CreateAssetMenu(fileName = "StationeryUtility", menuName = "ScriptableObject/StationeryUtility")]
public class StationeryUtilitySO: HolderSO{
    [SerializeField] private bool _isHoldToUse;
    public bool IsHoldToUse => this._isHoldToUse;
    [SerializeField] private int _useTime;
    public int UseTime => this._useTime;
    [SerializeField] private double _onUsingUpdateInterval;
    public double OnUsingUpdateInterval => this._onUsingUpdateInterval;
    [SerializeField] private UtensilSO _bindingUtensil;
    internal UtensilSO BindingUtensil => this._bindingUtensil;

    public static new StationeryUtilitySO GetSO(string strKey) => (StationeryUtilitySO)HolderSO.GetSO(strKey);
    public static new StationeryUtilitySO TryGetSO(string strKey) => (StationeryUtilitySO)InteractableSO.TryGetSO(strKey);


    // public void RegisterObject(){
    //     Debug.Log("Registering HolderSO: " + this.name);
    //     InteractableSO._existingInteractable.Add(this);

    //     foreach (InteractableSO curExistingInteractableSO in InteractableSO._existingInteractable){
    //         switch (curExistingInteractableSO){
    //             case GrabbableSO curExistingGrabbableSO:
    //                 if (this._usableTo.Contains(curExistingGrabbableSO)){
    //                     this._existingUsableTo.Add(curExistingGrabbableSO);
    //                 }
    //                 break;
    //         }
    //     }
    // }

}