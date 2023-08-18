using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ClientGameManager: NetworkBehaviour{
    
    [SerializeField] private ServerGameManager _server;

    public event EventHandler<OrderListChangeEventArgs> OnNewOrder;
    public event EventHandler<OrderListChangeEventArgs> OnFinishedOrder;
    public event EventHandler<OrderListChangeEventArgs> OnOrderWarning;
    public event EventHandler<OrderListChangeEventArgs> OnOrderOverdue;
    public event EventHandler<ScoreChangeEventArgs> OnScoreChange;

    [ClientRpc]
    internal void GameStateChangeCallbackClientRpc(string strKey, int previousScore, int currentScore, GameStateChangeCallbackID id){
        ICombinableSO newTargetDish;

        switch(id){
            case GameStateChangeCallbackID.OnFinishedOrder:
                newTargetDish = ICombinableSO.GetSO(strKey);
                this.OnFinishedOrder?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
                break;
            case GameStateChangeCallbackID.OnNewOrder:
                newTargetDish = ICombinableSO.GetSO(strKey);
                this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
                break;
            case GameStateChangeCallbackID.OnOrderOverdue:
                newTargetDish = ICombinableSO.GetSO(strKey);
                this.OnOrderOverdue?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
                break;
            case GameStateChangeCallbackID.OnOrderWarning:
                newTargetDish = ICombinableSO.GetSO(strKey);
                this.OnOrderWarning?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
                break;
            case GameStateChangeCallbackID.OnScoreChange:
                this.OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(previousScore, currentScore));
                break;
        }
    }

}