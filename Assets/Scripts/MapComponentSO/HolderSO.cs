using System.Collections.Generic;
using UnityEngine;

public abstract class HolderSO: InteractableSO{
    [SerializeField] private bool selfProcess = false;
    [SerializeField] private List<GrabbableSO> _holdable = new List<GrabbableSO>();
    public List<GrabbableSO> Holdable => this._holdable;
    public abstract void RegisterObject();

}