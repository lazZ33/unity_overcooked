using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ClientOrderManager: NetworkBehaviour {

	[SerializeField] private ServerOrderManager _server;


	public event EventHandler<OrderListChangeEventArgs> OnNewOrder;
	public event EventHandler<OrderListChangeEventArgs> OnFinishedOrder;
	public event EventHandler<OrderListChangeEventArgs> OnOrderWarning;
	public event EventHandler<OrderListChangeEventArgs> OnOrderOverdue;


	[ClientRpc]
	internal void OrderChangeCallbackClientRpc(string strKey, int previousScore, int currentScore, OrderListChangeCallbackID id) {
		ICombinableSO newTargetDish;

		switch (id) {
			case OrderListChangeCallbackID.OnFinishedOrder:
				newTargetDish = ICombinableSO.GetSO(strKey);
				this.OnFinishedOrder?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
				break;
			case OrderListChangeCallbackID.OnNewOrder:
				newTargetDish = ICombinableSO.GetSO(strKey);
				this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
				break;
			case OrderListChangeCallbackID.OnOrderOverdue:
				newTargetDish = ICombinableSO.GetSO(strKey);
				this.OnOrderOverdue?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
				break;
			case OrderListChangeCallbackID.OnOrderWarning:
				newTargetDish = ICombinableSO.GetSO(strKey);
				this.OnOrderWarning?.Invoke(this, new OrderListChangeEventArgs(newTargetDish));
				break;
		}
	}

}