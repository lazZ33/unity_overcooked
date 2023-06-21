using Unity;
using UnityEngine;
using Unity.Collections;
using System;

public class ClientUsableHolderVisuals: ClientInteractableVisuals{

    [SerializeField] private ClientUsableHolder _stationeryUtilityControl;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;

    void Awake(){
        this._stationeryUtilityControl.OnUse += this.OnUse;
        this._stationeryUtilityControl.OnUnuse += this.OnUnuse;
        this._stationeryUtilityControl.OnUsing += this.OnUsing;
    }

    private void OnUse(object sender, EventArgs args){
        
    }

    private void OnUnuse(object sender, EventArgs args){

    }

    private void OnUsing(object sender, EventArgs args){

    }
}