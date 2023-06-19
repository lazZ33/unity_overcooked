using Unity;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
[CreateAssetMenu(fileName = "UsableHolder", menuName = "ScriptableObject/UsableHolder")]
public class UsableHolderSO: HolderSO{
    [SerializeField] private bool _isHoldToUse;
    public bool IsHoldToUse => this._isHoldToUse;
    [SerializeField] private int _useTime;
    public int UseTime => this._useTime;
    [SerializeField] private double _onUsingUpdateInterval;
    public double OnUsingUpdateInterval => this._onUsingUpdateInterval;
    [SerializeField] private UtensilSO _bindingUtensil;
    internal UtensilSO BindingUtensil => this._bindingUtensil;

    public static new UsableHolderSO GetSO(string strKey) => (UsableHolderSO)HolderSO.GetSO(strKey);
    public static new UsableHolderSO TryGetSO(string strKey) => (UsableHolderSO)InteractableSO.TryGetSO(strKey);


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