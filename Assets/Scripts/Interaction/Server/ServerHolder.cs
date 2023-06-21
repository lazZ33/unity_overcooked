using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public abstract class ServerHolder: ServerInteractable, IHolder{

    private new ClientHolder _client => (ClientHolder)base._client;
    private new HolderSO _info { get { return (HolderSO)base._info; } set { base._info = value; } }
    public new HolderSO Info => (HolderSO)base._info;

    internal ServerGrabbable HoldGrabbable { get; private set; }

    internal bool IsHoldingGrabbable => this.HoldGrabbable != null;

    internal virtual void OnPlaceServerInternal(ServerGrabbable targetGrabbable){
        if (this.IsHoldingGrabbable) return;
        print("OnPlaceServerInternal");

        this.HoldGrabbable = targetGrabbable;
    }

    internal virtual void OnTakeServerInternal(out ServerGrabbable takenGrabbable){
        if (!this.IsHoldingGrabbable) { Debug.LogError("OnTakeServerInternal called while not holding any grabbable"); takenGrabbable = null; return; }
        print("OnTakeServerInternal");

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