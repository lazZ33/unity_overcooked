using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public abstract class ClientInteractablePhysics: NetworkBehaviour{
    [SerializeField] protected MeshCollider _meshCollider;
    [SerializeField] protected ClientInteractable _interactableControl;

    void Awake(){
        this._interactableControl.OnInfoChange += this.OnInfoChange;
    }

    protected virtual void OnInfoChange(object sender, InfoChangeEventArgs args){
        this._meshCollider.sharedMesh = args.Info.MeshCollider;
    }
}