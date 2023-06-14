using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

[Serializable]
public abstract class InteractableCompositionSO: ScriptableObject{

    [SerializeField] private MapComponentType _type;
    public MapComponentType Type => this._type;
    [SerializeField] private Mesh _mesh;
    public Mesh Mesh => this._mesh;
    [SerializeField] private Mesh _meshCollider;
    public Mesh MeshCollider => this._meshCollider;
    [SerializeField] private Material _material;
    public Material Material => this._material;


    [SerializeField] private string _usableName;
    public string UsableName => this._usableName;
    [SerializeField] private bool _isHoldToUse;
    public bool IsHoldToUse => this._isHoldToUse;
    [SerializeField] private int _useTime;
    public int UseTime => this._useTime;
    [SerializeField] private double _onUsingUpdateInterval;
    public double OnUsingUpdateInterval => this._onUsingUpdateInterval;


    public List<string> StrKeyList { get; private set; }
    public string StrKey { get; private set; }


    [SerializeField] private List<InteractableCompositionSO> _placeableTo = new List<InteractableCompositionSO>();
    [SerializeField] private List<InteractableCompositionSO> _combinableWith = new List<InteractableCompositionSO>();
    [SerializeField] private List<InteractableCompositionSO> _usableTo = new List<InteractableCompositionSO>();
    [SerializeField] private List<InteractableCompositionSO> _containedIngredient = new List<InteractableCompositionSO>();
    [SerializeField] private List<InteractableCompositionSO> _requiredStationeryUtilities = new List<InteractableCompositionSO>();


    private static HashSet<InteractableCompositionSO> _existingInteractable = new HashSet<InteractableCompositionSO>();
    private static Dictionary<string, InteractableCompositionSO> _nextCombinableDict = new Dictionary<string, InteractableCompositionSO>();
    private List<InteractableCompositionSO> _existingPlaceableTo = new List<InteractableCompositionSO>();
    private List<InteractableCompositionSO> _existingCombinableWith = new List<InteractableCompositionSO>();
    private List<InteractableCompositionSO> _existingUsableTo = new List<InteractableCompositionSO>();


    public bool CanPlaceOn(InteractableCompositionSO targetHolderSO) => this._existingPlaceableTo.Contains(targetHolderSO);
    public bool CanCombineWith(InteractableCompositionSO targetInteractableCompositionSO) => this._existingCombinableWith.Contains(targetInteractableCompositionSO);
    public bool CanUseOn(InteractableCompositionSO targetGrabbableSO) => this._existingUsableTo.Contains(targetGrabbableSO);


    void OnEnable(){
        // foreach(string str in ContainedInteractableNames) Debug.Log(str);
        // foreach(string str in RequiredUtilityNames) Debug.Log(str);
        List<string> ContainedIngredientNames = new List<string>(this._containedIngredient.Count);
        List<string> RequiredStationeryUtilitiesNames = new List<string>(this._requiredStationeryUtilities.Count);
        
        foreach(InteractableCompositionSO ContainedIngredientSO in this._containedIngredient){
            if (ContainedIngredientSO.Type != MapComponentType.Ingredient) Debug.LogError(String.Format("List _containedIngredient containes non-Ingredient SO, error SO: {0}", this.name));
            ContainedIngredientNames.Add(ContainedIngredientSO.name);
        }
        foreach(InteractableCompositionSO ContainedStationeryUtilitySO in this._requiredStationeryUtilities){
            if (ContainedStationeryUtilitySO.Type != MapComponentType.StationeryUtility) Debug.LogError(String.Format("List _containedIngredient containes non-StationeryUtility SO, error SO: {0}", this.name));
            RequiredStationeryUtilitiesNames.Add(ContainedStationeryUtilitySO.name);
        }
        
        List<string> strKeyList = new List<string>(ContainedIngredientNames.Concat(RequiredStationeryUtilitiesNames));
        strKeyList.Sort(); // assume no strings with same hashcode but different characters exist
        // foreach(string str in StrKeyList) Debug.Log(str);
        this.StrKeyList = strKeyList;
        this.StrKey = String.Concat(strKeyList);
    }

    public static InteractableCompositionSO getNextSO(InteractableCompositionSO info1, InteractableCompositionSO info2){
        List<string> strList = new List<string>(info1.StrKeyList.Concat(info2.StrKeyList));
        strList.Sort();
        return InteractableCompositionSO.GetSO(String.Concat(strList));
    }
    public static InteractableCompositionSO GetNextSO(InteractableCompositionSO InteractableInfo, StationeryUtilitySO stationeryUtilityInfo){
        List<string> strList = new List<string>(InteractableInfo.StrKeyList){stationeryUtilityInfo.name};
        strList.Sort();
        return InteractableCompositionSO.GetSO(String.Concat(strList));
    }
    public static string GetNextSOStrKey(InteractableCompositionSO info1, InteractableCompositionSO info2){
        List<string> strList;
        strList = new List<string>(info1.StrKeyList.Concat(info2.StrKeyList));
        strList.Sort();
        return String.Concat(strList);
    }
    public static string GetNextSOStrKey(InteractableCompositionSO InteractableInfo, StationeryUtilitySO stationeryUtilityInfo){
        List<string> strList = new List<string>(InteractableInfo.StrKeyList){stationeryUtilityInfo.name};
        strList.Sort();
        return String.Concat(strList);
    }
    public static InteractableCompositionSO GetSO(string strKey){
        InteractableCompositionSO._nextCombinableDict.TryGetValue(strKey, out InteractableCompositionSO result);
        if (result == null) result = (InteractableCompositionSO) Resources.Load("InteractableCompositionSO/Interactable/" + strKey);
        if (result == null) Debug.LogError("SO resource failed to load, possibly corrupted filename or SO name list. Tried string key: " + strKey);
        return result;
    }

}