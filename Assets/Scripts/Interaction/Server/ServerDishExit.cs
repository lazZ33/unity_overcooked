using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerDishExit: ServerHolder{

    private new ClientDishExit _client => (ClientDishExit)base._client;
    private new DishExitSO _info { get { return (DishExitSO)base._info; } set { base._info = value; } }
    public new DishExitSO Info => (DishExitSO)base._info;

    public event EventHandler<DishOutEventArgs> OnDishOut;

    internal override void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        base.OnPlaceServerInternal(targetGrabbable);
        print("OnPlaceServerInternal: ServerDishExit");

        this.OnDishOut?.Invoke(this, new DishOutEventArgs(targetGrabbable.Info));

        targetGrabbable.NetworkObjectBuf.Despawn();
    }

}