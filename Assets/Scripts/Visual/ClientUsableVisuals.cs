using Unity;
using UnityEngine;
using Unity.Collections;
using System;

public class ClientUsableVisuals: ClientInteractableVisuals {

	[SerializeField] private Animator _animator;
	private IClientUsable _stationaryUtilityControl => (IClientUsable)this._interactableControl;

	protected override void Awake() {
		base.Awake();
		this._stationaryUtilityControl.OnUse += this.OnUse;
		this._stationaryUtilityControl.OnUnuse += this.OnUnuse;
		this._stationaryUtilityControl.OnUsing += this.OnUsing;
		this._stationaryUtilityControl.OnConvert += this.OnConvert;
	}

	private void OnUse(object sender, ClientUseEventArgs args) {

	}

	private void OnUnuse(object sender, ClientUseEventArgs args) {

	}

	private void OnUsing(object sender, ClientUseEventArgs args) {

	}

	private void OnConvert(object sender, ClientUseEventArgs args) {

	}
}