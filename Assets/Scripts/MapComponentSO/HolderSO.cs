using System.Collections.Generic;
using UnityEngine;

public abstract class HolderSO: InteractableSO{
    [SerializeField] private List<GrabbableSO> _holdable = new List<GrabbableSO>();
    public List<GrabbableSO> Holdable => this._holdable;

    private HashSet<InteractableSO> _existingPlaceableTo { get; } = new HashSet<InteractableSO>();

    public bool CanPlaceOn(InteractableSO targetInteractableSO){
        return this._existingPlaceableTo.Contains(targetInteractableSO);
    }
}