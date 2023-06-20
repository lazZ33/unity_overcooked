using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerDishExit: ServerHolder{

    private new ClientDishExit _client => (ClientDishExit)base._client;
    public new DishExitSO Info { get { return (DishExitSO)base._info; } set { base._info = value; } }

    public event EventHandler<DishOutEventArgs> OnDishOut;

    internal override void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        base.OnPlaceServerInternal(targetGrabbable);

        this.OnDishOut?.Invoke(this, new DishOutEventArgs(targetGrabbable.Info));

        targetGrabbable.NetworkObjectBuf.Despawn();
    }

}