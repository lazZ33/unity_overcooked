using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;

public class ServerPlayerInteractionControl: ServerInteractable, IServerHolder {

	// Private serialized fields
	[SerializeField] private ServerInteractionManager _interactions;
	[SerializeField] private LayerMask _interactableLayerMask;
	[SerializeField] private Collider _grabCollider;


	// Control logic implementations
	[SerializeField] private ServerHoldTakeControl holdTakeControl;


	// DI variables
	private IServerGrabbable _holdGrabbable = null;


	// interface fields implementations/linkages
	IHolderSO IServerHolder.Info => (IHolderSO)base._info.Value;
	ulong IServerHolder.OwnerClientId => this.OwnerClientId;
	IServerGrabbable IServerHolder.HoldGrabbable => this._holdGrabbable;
	bool IServerHolder.IsHoldingGrabbable => this.holdTakeControl.IsHoldingGrabbable;


	// Events
	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;


	// Unity callbacks
	protected override void Awake() {
		base.Awake();

		if (this.holdTakeControl == null) {
			throw new NullReferenceException("null controller detected");
		}

		// hold take control DI
		{
			HoldTakeInitArgs holdTakeInitArgs = new HoldTakeInitArgs();
			holdTakeInitArgs.AddParentInstance(this);
			holdTakeInitArgs.AddGetInfoFunc(() => this.Info);
			holdTakeInitArgs.AddGetHoldGrabbableFunc(() => this._holdGrabbable);
			holdTakeInitArgs.AddSetHoldGrabbableFunc((IServerGrabbable holdGrabbable) => this._holdGrabbable = holdGrabbable);
			holdTakeControl.DepsInit(holdTakeInitArgs);
		}
	}


	// Interface logic linkages
	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this.holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this.holdTakeControl.OnTakeServerInternal(out takenGrabbable);


	// ServerRpcs
	[ServerRpc(RequireOwnership = false)]
	internal void GrabDropActionServerRpc() {
		Debug.Log("GrabDropActionServerRpc");

		Transform targetInteractableTransform = this.GetExpectedTargetInteractableTransform();
		IServerInteractable targetInteractable = (targetInteractableTransform == null) ? null : targetInteractableTransform.GetComponent<IServerInteractable>();

		// check all handled conditions
		switch (targetInteractable, this._holdGrabbable) {
			// both side have stuff
			case (IServerHolder targetHolder, IServerHolder holdHolder):
				// both are holders
				switch (targetHolder.HoldGrabbable, holdHolder.HoldGrabbable) {
					case (IServerCombinable targetContainedCombinable, IServerCombinable holdContainedCombinable):
						if (targetContainedCombinable.CanCombineWith(holdContainedCombinable))
							this._interactions.CombineOnHolderServerInternal(targetHolder, holdHolder);
						break;
				}
				break;
			case (IServerHolder targetHolder, IServerGrabbable holdGrabbable):
				// target is holder
				switch (targetHolder.HoldGrabbable, holdGrabbable) {
					case (IServerCombinable containedCombinable, IServerCombinable holdCombinable):
						if (holdCombinable.CanCombineWith(containedCombinable))
							this._interactions.CombineOnHolderServerInternal(targetHolder, this);
						break;
					case (null, IServerGrabbable):
						this._interactions.TransferServerInternal(targetHolder, this);
						break;
					default:
						break;
				}
				break;
			case (IServerGrabbable targetGrabbable, IServerHolder holdHolder):
				// holding holder
				switch (targetGrabbable, holdHolder.HoldGrabbable) {
					case (IServerCombinable targetCombinable, IServerCombinable containedCombinable):
						if (targetCombinable.CanCombineWith(containedCombinable))
							this._interactions.CombineOnHolderServerInternal(targetCombinable, holdHolder);
						break;
					case (IServerGrabbable, null):
						this._interactions.GrabServerInternal(holdHolder, targetGrabbable);
						break;
					default:
						break;
				}
				break;
			// both are combinables
			case (IServerCombinable targetCombinable, IServerCombinable holdCombinable):
				this._interactions.CombineOnHolderServerInternal(targetCombinable, this);
				break;


			// target side have stuff
			case (IServerSpawner targetSpawner, null): {
					IServerHolder targetHolder = targetSpawner as IServerHolder;
					if (targetHolder != null && targetHolder.IsHoldingGrabbable) {
						this._interactions.TransferServerInternal(this, targetHolder);
					} else
						this._interactions.SpawnAndGrabServerInternal(targetSpawner, this);
				}
				break;
			case (IServerHolder targetHolder, null):
				this._interactions.TransferServerInternal(this, targetHolder);
				break;
			case (IServerGrabbable targetGrabbable, null):
				this._interactions.GrabServerInternal(this, targetGrabbable);
				break;


			// player side have stuff
			case (null, IServerGrabbable holdGrabbable):
				this._interactions.DropServerInternal(this);
				break;


			// no cases matches from here
			default:
				Debug.Log("no cases matches");
				break;
		}
	}


	// Custom functions
	public Transform GetExpectedTargetInteractableTransform() {
		Vector3 grabCenter = new Vector3(this._grabCollider.transform.position.x, this._grabCollider.transform.position.y + this._grabCollider.bounds.extents.y, this._grabCollider.transform.position.z);
		Vector3 grabHalfSize = new Vector3(this._grabCollider.transform.lossyScale.x / 2, 0.01f, this._grabCollider.transform.lossyScale.z / 2); // this._grabCollider.transform.lossyScale/2;
		Vector3 grabDirection = new Vector3(this._grabCollider.transform.up.x, -this._grabCollider.transform.up.y, this._grabCollider.transform.up.z); //this._grabCollider.transform.up;
		Quaternion grabOrientation = this.transform.rotation;
		float grabMaxDist = this._grabCollider.bounds.size.y;

		RaycastHit[] allHits = Physics.BoxCastAll(grabCenter, grabHalfSize, grabDirection, grabOrientation, grabMaxDist, this._interactableLayerMask);

		if (allHits.Length == 0 | allHits == null)
			return null;

		RaycastHit targetHit = allHits[0];

		foreach (RaycastHit hit in allHits) {
			if (Vector3.Distance(this._grabCollider.transform.position, hit.transform.position)
			< Vector3.Distance(this._grabCollider.transform.position, targetHit.transform.position))
				targetHit = hit;
		}
		return targetHit.transform;
	}


	// Debugging
	void OnDrawGizmos() {

		Vector3 grabCenter = new Vector3(this._grabCollider.transform.position.x, this._grabCollider.transform.position.y + this._grabCollider.bounds.extents.y, this._grabCollider.transform.position.z);
		Vector3 grabHalfSize = new Vector3(this._grabCollider.transform.lossyScale.x / 2, 0.01f, this._grabCollider.transform.lossyScale.z / 2); // this._grabCollider.transform.lossyScale/2;
		Vector3 grabDirection = new Vector3(this._grabCollider.transform.up.x, -this._grabCollider.transform.up.y, this._grabCollider.transform.up.z); //this._grabCollider.transform.up;
		Quaternion grabOrientation = this.transform.rotation;
		float grabMaxDist = this._grabCollider.bounds.size.y;

		if (Physics.BoxCast(grabCenter, grabHalfSize, grabDirection, out RaycastHit hit, grabOrientation, grabMaxDist, this._interactableLayerMask)) {
			Gizmos.color = Color.red;
			Gizmos.DrawRay(grabCenter, grabDirection);
			Gizmos.DrawWireCube(grabCenter + grabDirection * hit.distance, grabHalfSize * 2);
		} else {
			Gizmos.color = Color.green;
			Gizmos.DrawRay(grabCenter, grabDirection * grabMaxDist);
			Gizmos.DrawWireCube(grabCenter, grabHalfSize * 2);
		}

	}
}
