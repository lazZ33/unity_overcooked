using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[Serializable]
public abstract class InteractableSO: ScriptableObject{

    [SerializeField] private Mesh _mesh;
    public Mesh Mesh => this._mesh;
    [SerializeField] private Mesh _meshCollider;
    public Mesh MeshCollider => this._meshCollider;
    [SerializeField] private Material _material;
    public Material Material => this._material;

    protected static HashSet<InteractableSO> ExistingInteractable = new HashSet<InteractableSO>();

}