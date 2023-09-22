using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ServerOrderManager: NetworkBehaviour {
	[SerializeField] private ClientOrderManager _client;
	[SerializeField] private ServerScoreManager _scoreManager;

	[SerializeField] private double _newOrderTimeInterval;
	[SerializeField] private double _orderWarningTime = 20;
	[SerializeField] private double _orderOverdueTime = 27;
	[SerializeField] private int _maxOrderCount = 7;


	public event EventHandler<OrderListChangeEventArgs> OnNewOrder;
	public event EventHandler<OrderListChangeEventArgs> OnFinishedOrder;
	public event EventHandler<OrderListChangeEventArgs> OnOrderWarning;
	public event EventHandler<OrderListChangeEventArgs> OnOrderOverdue;


	private ICombinableSO[] _targetDishes;
	private double _lastNewOrderTime;
	private List<Order> _existingOrders;
	private List<Order> _ordersToBeRemoved;
	private IScoreCalculator _scoreCalculator = new EasyScoreCalculator();
	private bool _isInit = false;


	public void OrderInit(ICombinableSO[] targetDishes) {
		if (!this.IsServer)
			return;

		this._isInit = true;
		this._targetDishes = targetDishes;
		this._lastNewOrderTime = ServerGameTimer.CurrentGameTime;
		this._existingOrders = new List<Order>(this._maxOrderCount);
		this._ordersToBeRemoved = new List<Order>(this._maxOrderCount);
		Debug.Log(this._existingOrders);

		//for (int i = 0; i < this._maxOrderCount; i++)
		//{
		//	this.AddNewRandomOrder(out Order newOrder);
		//}
	}

	public void OrderClear() {
		if (!this.IsServer)
			return;

		this._lastNewOrderTime = ServerGameTimer.CurrentGameTime;
		this._existingOrders.Clear();
	}

	public void OnDishOut(object sender, DishOutEventArgs args) {
		if (!this.IsServer || !this._isInit)
			return;

		Order matchingOrder = this._existingOrders.Find(order => order.RequestedDish == args.Dish);

		if (matchingOrder != null) {
			this._scoreManager.ScoreChange(matchingOrder.ScoreGain);

			this.OnFinishedOrder?.Invoke(this, new OrderListChangeEventArgs(matchingOrder.RequestedDish));
			this._client.OrderChangeCallbackClientRpc(matchingOrder.RequestedDish.StrKey, 0, 0, OrderListChangeCallbackID.OnFinishedOrder);
		}
	}
	public void OnOrderUpdate() {
		if (!this.IsServer || !this._isInit || ServerGameTimer.State != GameTimerState.Running)
			return;

		// check if any order warning/overdue
		foreach (Order curOrder in this._existingOrders) {
			if (ServerGameTimer.CurrentGameTime >= curOrder.OverdueTime) {
				this._ordersToBeRemoved.Add(curOrder);

				this._scoreManager.ScoreChange(curOrder.ScoreLoss);

				this.OnOrderOverdue?.Invoke(this, new OrderListChangeEventArgs(curOrder.RequestedDish));
				this._client.OrderChangeCallbackClientRpc(curOrder.RequestedDish.StrKey, 0, 0, OrderListChangeCallbackID.OnOrderOverdue);
			} else if (ServerGameTimer.CurrentGameTime >= curOrder.WarningTime) {
				this.OnOrderWarning?.Invoke(this, new OrderListChangeEventArgs(curOrder.RequestedDish));
				this._client.OrderChangeCallbackClientRpc(curOrder.RequestedDish.StrKey, 0, 0, OrderListChangeCallbackID.OnOrderWarning);
			}

		}
		foreach (Order orderToBeremoved in this._ordersToBeRemoved)
			this._existingOrders.Remove(orderToBeremoved);

		// periodically add new random orders
		if (this._existingOrders.Count < this._maxOrderCount && ServerGameTimer.CurrentGameTime >= this._lastNewOrderTime + this._newOrderTimeInterval) {
			this.AddNewRandomOrder(out Order newOrder);

			this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newOrder.RequestedDish));
			this._client.OrderChangeCallbackClientRpc(newOrder.RequestedDish.StrKey, 0, 0, OrderListChangeCallbackID.OnNewOrder);
		}
	}

	private void AddNewRandomOrder(out Order newOrder) {
		if (!this.IsServer || !this._isInit) { newOrder = null; return; }
		print("AddNewRandomOrder");

		int randomIdx = (int)(UnityEngine.Random.value) * int.MaxValue % this._targetDishes.Length;
		ICombinableSO newRandomTargetDish = this._targetDishes[randomIdx];
		double newOrderWarningTime = ServerGameTimer.CurrentGameTime + this._orderWarningTime;
		double newOrderOverdueTime = ServerGameTimer.CurrentGameTime + this._orderOverdueTime;

		newOrder = new Order(newRandomTargetDish, newOrderWarningTime, newOrderOverdueTime, this._scoreCalculator);
		this._existingOrders.Add(newOrder);
		this._lastNewOrderTime = ServerGameTimer.CurrentGameTime;

		this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newRandomTargetDish));
		this._client.OrderChangeCallbackClientRpc(newRandomTargetDish.StrKey, 0, 0, OrderListChangeCallbackID.OnNewOrder);
	}
}
