using Unity;
using UnityEngine;
using Unity.Netcode;
using System;

using HoldTakeInitArgs = ServerHoldTakeControl.HoldTakeControlInitArgs;
using SpawnInitArgs = ServerSpawnControl.SpawnControlInitArgs;

internal class ServerSpawner : ServerInteractable, IServerHolder, IServerSpawner
{
	[SerializeField] private ServerHoldTakeControl holdTakeControl;
	[SerializeField] private ServerSpawnControl spawnControl;


	// DI variables
	private IServerGrabbable _holdGrabbable = null;
	public event EventHandler<SpawnEventArgs> OnSpawn;
	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;

	ISpawnerSO IServerSpawner.Info => (ISpawnerSO) base._info;
	IHolderSO IServerHolder.Info => (IHolderSO) base._info;
	ulong IServerHolder.OwnerClientId => base.OwnerClientId;
	bool IServerHolder.IsHoldingGrabbable => this.holdTakeControl.IsHoldingGrabbable;
	IServerGrabbable IServerHolder.HoldGrabbable => this._holdGrabbable;


	protected override void Awake()
	{
		base.Awake();

		if (this.holdTakeControl == null || this.spawnControl == null)
		{
			throw new NullReferenceException("null controller detected");
		}

		// hold take control DI
		{
			HoldTakeInitArgs holdTakeInitArgs = new HoldTakeInitArgs();
			holdTakeInitArgs.AddParentInstance(this);
			holdTakeInitArgs.AddGetInfoFunc(() => { return base.Info; });
			holdTakeInitArgs.AddGetHoldGrabbableFunc(() => { return this._holdGrabbable; });
			holdTakeInitArgs.AddSetHoldGrabbableFunc((IServerGrabbable holdGrabbable) => { this._holdGrabbable = holdGrabbable; });
			this.holdTakeControl.OnHold += (sender, args) => { this.OnHold?.Invoke(sender, args); };
			this.holdTakeControl.OnHold += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnHold, args.TargetGrabbableInfo.StrKey); };
			this.holdTakeControl.OnTake += (sender, args) => { this.OnTake?.Invoke(sender, args); };
			this.holdTakeControl.OnTake += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnTake, args.TargetGrabbableInfo.StrKey); };
			this.holdTakeControl.DepsInit(holdTakeInitArgs);
		}

		// spawn control DI
		{
			SpawnInitArgs spawnInitArgs = new SpawnInitArgs();
			spawnInitArgs.AddParentInstance(this);
			spawnInitArgs.AddGetInfoFunc(() => { return this.Info; });
			this.spawnControl.OnSpawn += (sender, args) => { this.OnSpawn?.Invoke(sender, args); };
			this.spawnControl.OnSpawn += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnSpawn, args.SpawnedGrabbableInfo.StrKey); };
			this.holdTakeControl.DepsInit(spawnInitArgs);
		}
	}

	public void SpawnningGrabbableInfoInit(IGrabbableSO info)
	{
		if (this.IsSpawned)
		{
			Debug.LogError("Attempt to initialize spawnning grabbable info after an object have network spawned");
			return;
		}

		this.spawnControl.SpawnningGrabbableInfo = info;
	}

	IServerGrabbable IServerSpawner.OnSpawnServerInternal() => this.spawnControl.OnSpawnServerInternal();
	void IServerHolder.OnHoldServerInternal(IServerGrabbable targetGrabbable) => this.holdTakeControl.OnHoldServerInternal(targetGrabbable);
	void IServerHolder.OnTakeServerInternal(out IServerGrabbable takenGrabbable) => this.holdTakeControl.OnTakeServerInternal(out takenGrabbable);
}