using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class ClientMapManager: NetworkBehaviour
{
    [SerializeField] private ServerMapManager _server;

    private List<ICombinableSO> _targetDishesSO;
    private List<ICombinableSO> _requiredCombinableSOList;
    private List<IConverterSO> _requiredConverterSOList;


    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!this.IsClient) return;

        this._server.TargetDishesSOStrKeys.OnListChanged += this.OnTargetDishesChanged;
    }


    private void OnTargetDishesChanged(NetworkListEvent<FixedString128Bytes> changeEvent){
        if (!this.IsClient | this.IsServer) return;
        if (!(changeEvent.Index == this._server.TargetDishesAmount.Value) | !(changeEvent.Type == NetworkListEvent<FixedString128Bytes>.EventType.Add)) return;
     
        foreach (FixedString128Bytes strKey in this._server.TargetDishesSOStrKeys){
            this._targetDishesSO.Add((ICombinableSO) IInteractableSO.GetSO(strKey.ToString()));
        }
        ICombinableSO.GetRequiredBaseSO(this._targetDishesSO, out _requiredCombinableSOList, out _requiredConverterSOList);
        ICombinableSO.LoadAllRequiredSO(_requiredCombinableSOList, _requiredConverterSOList);
    }
}
