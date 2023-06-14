using Unity;
using UnityEngine;
using System;

public class ClientGrabbableVisuals: MonoBehaviour{

    [SerializeField] private ClientGrabbable _grabbableControl;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Renderer _renderer;

    void Awake(){
        this._grabbableControl.OnGrab += this.OnGrab;
        this._grabbableControl.OnDrop += this.OnDrop;
        this._grabbableControl.OnPlace += this.OnPlace;
        this._grabbableControl.OnTake += this.OnTake;
        this._grabbableControl.OnInfoChange += this.OnInfoChange;
    }

    private void OnGrab(object sender, ClientGrabbable.InteractionEventArgs args){
        
    }

    private void OnDrop(object sender, ClientGrabbable.InteractionEventArgs args){

    }

    private void OnPlace(object sender, ClientGrabbable.InteractionEventArgs args){

    }

    private void OnTake(object sender, ClientGrabbable.InteractionEventArgs args){

    }

    private void OnCombine(object sender, ClientGrabbable.InteractionEventArgs args){

    }

    private void OnInfoChange(object sender, ClientGrabbable.InteractionEventArgs args){
        print("OnInfoChange");
        this._meshFilter.mesh = args.Info.Mesh;
        for (int i = 0; i < this._renderer.materials.Length; i++){
            this._renderer.materials[i] = args.Info.Material;
        }
    }
}