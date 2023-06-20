using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ServerGameManager: NetworkBehaviour{

    [SerializeField] private ClientGameManager _client;
    [SerializeField] private CombinableSO[] _targetDishes;
    [SerializeField] private float _newOrderTimeInterval;
    [SerializeField] private float _orderWarningTime = 20;
    [SerializeField] private float _orderOverdueTime = 27;
    [SerializeField] private int _maxOrderCount = 7;

    public event EventHandler<OrderListChangeEventArgs> OnNewOrder;
    public event EventHandler<OrderListChangeEventArgs> OnFinishedOrder;
    public event EventHandler<OrderListChangeEventArgs> OnOrderWarning;
    public event EventHandler<OrderListChangeEventArgs> OnOrderOverdue;
    public event EventHandler<ScoreChangeEventArgs> OnScoreChange;


    public float TimePassed => Time.fixedUnscaledTime -  this._gameStartTime;
    public static float CurGameTime => Time.fixedUnscaledTime;

    private float _gameStartTime;
    private float _lastNewOrderTime;
    private int _gameScore;
    private ServerDishExit[] _allExits;
    private List<Order> _existingOrders;

    void Awake(){
        if (!this.IsServer) return;
        
        this._existingOrders = new List<Order>(this._maxOrderCount);
        this._allExits = GameObject.FindObjectsOfType<ServerDishExit>();
        foreach (ServerDishExit curExits in _allExits)
            curExits.OnDishOut += this.OnDishOut;
    }

    public void OnGameStart(){
        this._gameStartTime = ServerGameManager.CurGameTime;
        this._lastNewOrderTime = this._gameStartTime;
    }

    private void AddNewRandomOrder(out Order newOrder){
        int randomIdx = (int)(UnityEngine.Random.value) * int.MaxValue % this._targetDishes.Length;
        CombinableSO newRandomTargetDish = this._targetDishes[randomIdx];
        float newOrderWarningTime = ServerGameManager.CurGameTime + this._orderWarningTime;
        float newOrderOverdueTime = ServerGameManager.CurGameTime + this._orderOverdueTime;

        newOrder = new Order(newRandomTargetDish, newOrderWarningTime, newOrderOverdueTime);
        this._existingOrders.Add(newOrder);
        
        this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newRandomTargetDish));
        this._client.GameStateChangeCallbackClientRpc(newRandomTargetDish.StrKey, 0, GameStateChangeCallbackID.OnNewOrder);
    }

    private void OnDishOut(object sender, DishOutEventArgs args){
        // TODO: add score according to current existing orders
        Order matchingOrder = this._existingOrders.Find(order => order.RequestedDish == args.Dish);
        if (matchingOrder != null){
            this._gameScore += matchingOrder.ScoreGain;

            this.OnFinishedOrder?.Invoke(this, new OrderListChangeEventArgs(matchingOrder.RequestedDish));
            this.OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(this._gameScore));
        }
    }

    void FixedUpdate(){
        // check if any order warning/overdue
        foreach (Order curOrder in this._existingOrders){
            if (ServerGameManager.CurGameTime >= curOrder.OverdueTime){
                this._existingOrders.Remove(curOrder);

                this._gameScore -= curOrder.ScoreLoss;

                this.OnOrderOverdue?.Invoke(this, new OrderListChangeEventArgs(curOrder.RequestedDish));
                this.OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(this._gameScore));
            }
            else if (ServerGameManager.CurGameTime >= curOrder.WarningTime){
                this.OnOrderWarning?.Invoke(this, new OrderListChangeEventArgs(curOrder.RequestedDish));
            }

        }

        // periodically add new random orders
        if (this._existingOrders.Count < this._maxOrderCount && ServerGameManager.CurGameTime >= this._lastNewOrderTime + this._newOrderTimeInterval){
            this.AddNewRandomOrder(out Order newOrder);

            this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newOrder.RequestedDish));
        }

    }

}