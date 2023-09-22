using Unity;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

public class ServerGrabbablePhysics: ServerInteractablePhysics {
	[SerializeField] private Rigidbody _rigidBody;
	private IServerGrabbable _grabbableControl => (IServerGrabbable)base._interactableControl;

	protected override void Awake() {
		base.Awake();
		this._grabbableControl.OnGrab += this.OnGrab;
		this._grabbableControl.OnDrop += this.OnDrop;
	}

	private void OnGrab(object sender, ServerGrabDropEventArgs args) {
		IServerGrabbable grabbableControl = (IServerGrabbable)sender;
		IGrabbableSO grabbableInfo = args.GrabbableInfo;
		IServerHolder holderControl = (IServerHolder)args.Object;

		grabbableControl.NetworkObjectBuf.TrySetParent(holderControl.transform, false);
		this.transform.localPosition = holderControl.Info.LocalPlacePosition;
		this.transform.localRotation = holderControl.Info.LocalPlaceRotation;

		this._rigidBody.useGravity = false;
		this._rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		grabbableControl.NetworkObjectBuf.ChangeOwnership(holderControl.OwnerClientId);
	}

	private void OnDrop(object sender, ServerGrabDropEventArgs args) {
		IServerGrabbable grabbableControl = (IServerGrabbable)sender;

		this._rigidBody.useGravity = true;
		this._rigidBody.constraints = RigidbodyConstraints.None;
		grabbableControl.NetworkObjectBuf.TryRemoveParent();
		grabbableControl.NetworkObjectBuf.RemoveOwnership();
	}
}