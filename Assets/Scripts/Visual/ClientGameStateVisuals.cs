using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ClientGameStateVisuals: NetworkBehaviour{
    
    [SerializeField] private ClientGameManager _gameManager;
    
    protected virtual void Awake(){
        this._gameManager.OnFinishedOrder += this.OnFinishedOrder;
        this._gameManager.OnNewOrder += this.OnNewOrder;
        this._gameManager.OnOrderOverdue += this.OnOrderOverdue;
        this._gameManager.OnOrderWarning += this.OnOrderWarning;
        this._gameManager.OnScoreChange += this.OnScoreChange;
    }

    private void OnFinishedOrder(object sender, OrderListChangeEventArgs args){
        
    }
    private void OnNewOrder(object sender, OrderListChangeEventArgs args){

    }
    private void OnOrderWarning(object sender, OrderListChangeEventArgs args){

    }
    private void OnOrderOverdue(object sender, OrderListChangeEventArgs args){

    }
    private void OnScoreChange(object sender, ScoreChangeEventArgs args){
        
    }
}