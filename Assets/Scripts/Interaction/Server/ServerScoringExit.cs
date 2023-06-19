using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerDishExit: ServerHolder{

    private new ClientHolder _client => (ClientHolder)base._client;
    public new HolderSO Info { get { return (HolderSO)base._info; } set { base._info = value; } }

    public event EventHandler<DishOutEventArgs> OnDishOut;
    public class DishOutEventArgs: EventArgs{
        internal DishOutEventArgs(GrabbableSO dish){ this.Dish = dish; }
        public GrabbableSO Dish;
    }

    // TODO: make ServerScoringExit inherits ServerUsableHolder(?)

    internal override void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        base.OnPlaceServerInternal(targetGrabbable);

        this.OnDishOut?.Invoke(this, new DishOutEventArgs(targetGrabbable.Info));

        targetGrabbable.NetworkObjectBuf.Despawn();
    }

}