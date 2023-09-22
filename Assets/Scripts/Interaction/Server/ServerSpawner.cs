using Unity;
using UnityEngine;
using Unity.Netcode;
using System;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;
using SpawnInitArgs = ServerSpawnControl.SpawnControlInitArgs;

internal class ServerSpawner: ServerInteractable, IServerHolder, IServerSpawner {
	[SerializeField] private ServerHoldTakeControl _holdTakeControl;
	[SerializeField] private ServerSpawnControl _spawnControl;


	// DI variables
	private IServerGrabbable _holdGrabbable = null;
	public event EventHandler<SpawnEventArgs> OnSpawn;
	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;

	ISpawnerSO IServerSpawner.Info => (ISpawnerSO)base._info.Value;
	IHolderSO IServerHolder.Info => (IHolderSO)base._info.Value;
	ulong IServerHolder.OwnerClientId => base.OwnerClientId;
	bool IServerHolder.IsHoldingGrabbable => this._holdTakeControl.IsHoldingGrabbable;
	IServerGrabbable IServerHolder.HoldGrabbable => this._holdGrabbable;


	protected override void Awake() {
		base.Awake();

		if (this._holdTakeControl == null || this._spawnControl == null) {
			throw new NullReferenceException("null controller detected");
		}

		// hold take control DI
		{
			HoldTakeInitArgs holdTakeInitArgs = new HoldTakeInitArgs();
			holdTakeInitArgs.AddParentInstance(this);
			holdTakeInitArgs.AddGetInfoFunc(() => { return base.Info; });
			holdTakeInitArgs.AddGetHoldGrabbableFunc(() => { return this._holdGrabbable; });
			holdTakeInitArgs.AddSetHoldGrabbableFunc((IServerGrabbable holdGrabbable) => { this._holdGrabbable = holdGrabbable; });
			this._holdTakeControl.OnHold += (sender, args) => { this.OnHold?.Invoke(sender, args); };
			this._holdTakeControl.OnHold += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnHold, args.TargetGrabbableInfo.StrKey); };
			this._holdTakeControl.OnTake += (sender, args) => { this.OnTake?.Invoke(sender, args); };
			this._holdTakeControl.OnTake += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnTake, args.TargetGrabbableInfo.StrKey); };
			this._holdTakeControl.DepsInit(holdTakeInitArgs);
		}

		// spawn control DI
		{
			SpawnInitArgs spawnInitArgs = new SpawnInitArgs();
			spawnInitArgs.AddParentInstance(this);
			spawnInitArgs.AddGetInfoFunc(() => { return this.Info; });
			this._spawnControl.OnSpawn += (sender, args) => { this.OnSpawn?.Invoke(sender, args); };
			this._spawnControl.OnSpawn += (sender, args) => { base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnSpawn, args.SpawnedGrabbableInfo.StrKey); };
			this._spawnControl.DepsInit(spawnInitArgs);
		}
	}

	public override void OnMapDespawn(object sender, EventArgs args) {
		base.OnMapDespawn(sender, args);
		this._spawnControl.OnMapDespawn();
	}

	public void SpawnningGrabbableInfoInit(IGrabbableSO info) {
		if (this.IsSpawned) {
			Debug.LogError("Attempt to initialize spawnning grabbable info after an object have network spawned");
			return;
		}

		this._spawnControl.SpawnningGrabbableInfo = info;
	}

	public override void OnMapUpdate() {
		this._holdTakeControl.OnMapUpdate();
		this._spawnControl.OnMapUpdate();
	}

	IServerGrabbable IServerSpawner.OnSpawnServerInternal() => this._spawnControl.OnSpawnServerInternal();
	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this._holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this._holdTakeControl.OnTakeServerInternal(out takenGrabbable);
}