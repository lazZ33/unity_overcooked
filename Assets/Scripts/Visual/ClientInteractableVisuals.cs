using Unity;
using UnityEngine;
using Unity.Netcode;
using System;

public abstract class ClientInteractableVisuals: NetworkBehaviour{

    [SerializeField] private ClientInteractable _interactableControl;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Renderer _renderer;

    protected virtual void Awake(){
        this._interactableControl.OnInfoChange += this.OnInfoChange;
    }

    protected virtual void OnInfoChange(object sender, InfoChangeEventArgs args){
        print("OnInfoChange");
        this._meshFilter.mesh = args.Info.Mesh;
        for (int i = 0; i < this._renderer.materials.Length; i++){
            this._renderer.materials[i] = args.Info.Material;
        }
    }
}