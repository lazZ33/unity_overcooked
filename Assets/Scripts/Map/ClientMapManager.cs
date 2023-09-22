using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class ClientMapManager: NetworkBehaviour {
	[SerializeField] private ServerMapManager _server;

	private List<ICombinableSO> _targetDishesSO;
	private List<ICombinableSO> _requiredCombinableSOList;
	private List<IConverterSO> _requiredConverterSOList;


	public override void OnNetworkSpawn() {
		base.OnNetworkSpawn();
		if (!this.IsClient | this.IsServer)
			return;

		Debug.Log(this._server.TargetDishesSOStrKeys.Count);
		this._server.TargetDishesSOStrKeys.OnListChanged += this.OnTargetDishesChanged;
		//this.OnTargetDishesChanged(new NetworkListEvent<FixedString128Bytes>());
	}


	private void OnTargetDishesChanged(NetworkListEvent<FixedString128Bytes> changeEvent) {
		if (!this.IsClient | this.IsServer)
			return;

		Debug.Log(this._server.TargetDishesSOStrKeys.Count);
		foreach (FixedString128Bytes strKey in this._server.TargetDishesSOStrKeys) {
			ICombinableSO targetDishSO = (ICombinableSO)IInteractableSO.GetSO(strKey.ToString());
			this._targetDishesSO.Add(targetDishSO);
			Debug.Log(targetDishSO);
		}
		ICombinableSO.GetRequiredBaseSO(this._targetDishesSO, out _requiredCombinableSOList, out _requiredConverterSOList);
		ICombinableSO.LoadAllRequiredSO(_requiredCombinableSOList, _requiredConverterSOList);
	}
}
