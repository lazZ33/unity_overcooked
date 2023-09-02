using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;
using TNRD;

public abstract class ServerInteractablePhysics: NetworkBehaviour{
    [SerializeField] protected MeshCollider _meshCollider;
    [SerializeField] private SerializableInterface<IServerInteractable> interactableControl;
    protected IServerInteractable _interactableControl => this.interactableControl.Value;


	protected virtual void Awake(){
        this._interactableControl.OnInfoChange += this.OnInfoChange;
    }

    protected virtual void OnInfoChange(object sender, InfoChangeEventArgs args){
        // print("OnInfoChange: ServerInteractablePhysics");
        this._meshCollider.sharedMesh = args.Info.MeshCollider;
    }
}