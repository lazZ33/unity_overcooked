using Unity;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;

public class ClientStationeryUtility : ClientInteractable, IClientUsable, IClientHolder
{
	private new ServerStationaryUtility _server => (ServerStationaryUtility)base._server;
	private new StationeryUtilitySO Info => (StationeryUtilitySO)base._info;
	private new StationeryUtilitySO _info => (StationeryUtilitySO)base._info;


	public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChangeFromNV { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


	IUsableSO IClientUsable.Info => (IUsableSO)base._info;
	IHolderSO IClientHolder.Info => (IHolderSO)base._info;
	bool IClientUsable.IsHoldToUse => ((IUsableSO)base._info).IsHoldToUse;


	public event EventHandler<ServerUseEventArgs> OnUse;
	public event EventHandler<ServerUseEventArgs> OnUsing;
	public event EventHandler<ServerUseEventArgs> OnUnuse;
	public event EventHandler<ServerUseEventArgs> OnConvert;
	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
}