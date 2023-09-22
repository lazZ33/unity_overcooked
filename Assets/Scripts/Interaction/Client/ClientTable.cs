using UnityEngine;
using Unity;
using Unity.Netcode;
using System;

public class ClientTable: ClientInteractable, IClientHolder {
	private new ServerTable _server => (ServerTable)base._server;
	private new TableSO Info => (TableSO)base._info;
	private new TableSO _info => (TableSO)base._info;


	IHolderSO IClientHolder.Info => (IHolderSO)base._info;


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
}