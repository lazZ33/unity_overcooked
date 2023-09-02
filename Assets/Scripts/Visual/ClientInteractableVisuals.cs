using Unity;
using UnityEngine;
using Unity.Netcode;
using System;
using TNRD;

public abstract class ClientInteractableVisuals: NetworkBehaviour{

    [SerializeField] private SerializableInterface<IClientInteractable> interactableControl;
    protected IClientInteractable _interactableControl => this.interactableControl.Value;
	[SerializeField] protected MeshFilter _meshFilter;
    [SerializeField] protected Renderer _renderer;

    protected virtual void Awake(){
        this._interactableControl.OnInfoChange += this.OnInfoChange;
    }

    protected virtual void OnInfoChange(object sender, InfoChangeEventArgs args){
        //print("OnInfoChange: Visual");
        this._meshFilter.mesh = args.Info.Mesh;
        for (int i = 0; i < this._renderer.materials.Length; i++){
            this._renderer.materials[i] = args.Info.Material;
        }
    }
}