using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerUtensil: ServerGrabbable, IHolder{

    public new UtensilSO Info => (UtensilSO)base._client.Info;
    private new UtensilSO _info { get{ return (UtensilSO)base._info; } set{ base._info = value; } }
    private new ClientUtensil _client => (ClientUtensil)base._client;
    internal ServerGrabbable HoldGrabbable { get; private set; }

    public bool IsHoldingGrabbable => this.HoldGrabbable != null;

    internal void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        print("OnUtensilPlaceServerInternal");

        this.HoldGrabbable = targetGrabbable;
    }

    internal void OnTakeServerInternal(out ServerGrabbable takenGrabbable){
        print("OnUtensilTakeServerInternal");
        takenGrabbable = this.HoldGrabbable;
        this.HoldGrabbable = null;
    }

    void IHolder.OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        this.OnPlaceServerInternal(targetGrabbable);
    }
    void IHolder.OnTakeServerInternal(out ServerGrabbable targetGrabbable){
        this.OnTakeServerInternal(out targetGrabbable);
    }
}