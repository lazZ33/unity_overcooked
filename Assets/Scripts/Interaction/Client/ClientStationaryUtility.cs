using Unity;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;

public class ClientStationaryUtility : ClientInteractable, IClientUsable, IClientHolder
{
	private new ServerStationaryUtility _server => (ServerStationaryUtility)base._server;
	private new StationaryUtilitySO Info => (StationaryUtilitySO)base._info;
	private new StationaryUtilitySO _info => (StationaryUtilitySO)base._info;


	public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChangeFromNV { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


	IConverterSO IClientUsable.Info => (IConverterSO)base._info;
	IHolderSO IClientHolder.Info => (IHolderSO)base._info;
	bool IClientUsable.IsHoldToConvert => ((IConverterSO)base._info).IsHoldToConvert;


	public event EventHandler<ClientUseEventArgs> OnUse;
	public event EventHandler<ClientUseEventArgs> OnUsing;
	public event EventHandler<ClientUseEventArgs> OnUnuse;
	public event EventHandler<ClientUseEventArgs> OnConvert;
	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
}