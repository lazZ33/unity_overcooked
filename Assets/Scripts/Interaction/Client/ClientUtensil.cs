using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ClientUtensil: ClientInteractable, IClientHolder, IClientGrabbable {
	private new ServerUtensil _server => (ServerUtensil)base._server;
	private new UtensilSO Info => (UtensilSO)base._info;
	private new UtensilSO _info => (UtensilSO)base._info;


	IGrabbableSO IClientGrabbable.Info => this._info;
	IHolderSO IClientHolder.Info => this._info;


	public bool CanPlaceOn(IServerHolder targetHolder) => ((IGrabbableSO)this.Info).CanPlaceOn(targetHolder.Info);


	public event EventHandler<ClientGrabDropEventArgs> OnGrab;
	public event EventHandler<ClientGrabDropEventArgs> OnDrop;
	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
}