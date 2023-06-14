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
    
    [SerializeField] protected List<HolderSO> _placeableTo = new List<HolderSO>();
    [SerializeField] protected List<UtensilSO> _compatibleUtensils = new List<UtensilSO>(); // TODO: get a better name(?)

    protected HashSet<HolderSO> _existingPlaceableTo { get; } = new HashSet<HolderSO>();
    protected HashSet<UtensilSO> _existingComtapibleUtensil { get; } = new HashSet<UtensilSO>();

    public bool CanPlaceOn(HolderSO targetHolderSO) => this._existingPlaceableTo.Contains(targetHolderSO);
    public bool CanPlaceOn(UtensilSO targetUtensilSO) => this._existingComtapibleUtensil.Contains(targetUtensilSO);
    public new static GrabbableSO GetSO(string strKey) => (GrabbableSO)InteractableSO.GetSO(strKey);
    public new static GrabbableSO TryGetSO(string strKey) => (GrabbableSO)InteractableSO.TryGetSO(strKey);

    protected virtual void OnEnable(){
        this._existingComtapibleUtensil.Clear();
        this._existingPlaceableTo.Clear();
    }

}