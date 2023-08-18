using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ClientUtensil: ClientInteractable, IGrabbableSO, IUsableSO
{
	private new ServerUtensil _server => (ServerUtensil)base._server;
	private new UtensilSO Info => (UtensilSO)base._info;
	private new UtensilSO _info => (UtensilSO)base._info;


	private HashSet<IHolderSO> _existingPlaceableTo = new HashSet<IHolderSO>();
	HashSet<IHolderSO> IGrabbableSO._existingPlaceableTo => this._existingPlaceableTo;


	public string StrKey => ((IUsableSO)base._info).StrKey;
	public bool IsConverter => ((IUsableSO)base._info).IsConverter;
	public bool IsHoldToUse => ((IUsableSO)base._info).IsHoldToUse;
	public double OnUsingUpdateInterval => ((IUsableSO)base._info).OnUsingUpdateInterval;
}