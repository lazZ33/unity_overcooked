using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public abstract class ServerInteractablePhysics: NetworkBehaviour{
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private ServerInteractable _interactableControl;

    protected virtual void Awake(){
        this._interactableControl.OnInfoChange += this.OnInfoChange;
    }

    protected virtual void OnInfoChange(object sender, InfoChangeEventArgs args){
        this._meshCollider.sharedMesh = args.Info.MeshCollider;
    }
}