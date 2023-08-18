using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

internal class ServerCombineControl: ServerInteractControl
{
	// shared dependencies to be injected
	private new ICombinableSO _info { get { return (ICombinableSO) base._info; } }


    public event EventHandler<CombineEventArgs> OnCombine;


	public bool CanCombineWith(IServerCombinable targetCombineControl) => this._info.CanCombineWith(targetCombineControl.Info);


    // builder DI
    internal class CombineControlInitArgs: InteractControlInitArgs
    {
        internal CombineControlInitArgs() { }
    }
	internal override void DepsInit(InteractControlInitArgs args)
	{
		base.DepsInit(args);
	}


	internal void OnCombineServerInternal(){
        print("OnCombineServerInternal");

        this.OnCombine?.Invoke(this, new CombineEventArgs(this._info));
        // this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnCombine);
        print("Combined");
    }
}
