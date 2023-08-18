using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity;
using Unity.Netcode;
using UnityEngine;

using GrabDropInitArgs = ServerGrabDropControl.GrabDropControlInitArgs;
using UseInitArgs = ServerUseControl.UseControlInitArgs;

internal class ServerTool : ServerInteractable, IServerGrabbable, IServerUsable
{
	[SerializeField] private ServerGrabDropControl grabDropControl;
	[SerializeField] private ServerUseControl useControl;


	// DI variables
	private NetworkVariable<ulong> _grabbedClientId = new NetworkVariable<ulong>(GRABBED_CLIENT_ID_DEFAULT);
	private static readonly ulong GRABBED_CLIENT_ID_DEFAULT = ulong.MaxValue;


	IGrabbableSO IServerGrabbable.Info => (IGrabbableSO)base._info;
	IUsableSO IServerUsable.Info => (IUsableSO)base._info;


	bool IServerUsable.IsHoldToUse => ((IServerUsable)this.Info).IsHoldToUse;
	bool IServerGrabbable.IsGrabbedByPlayer => this.grabDropControl.IsGrabbedByPlayer;
	bool IServerGrabbable.IsGrabbedByLocal => this.grabDropControl.IsGrabbedByPlayer;


	bool IServerGrabbable.CanPlaceOn(IServerHolder targetHolder) => ((IGrabbableSO)base._info).CanPlaceOn(targetHolder.Info);


	public event EventHandler<GrabDropEventArgs> OnGrab;
	public event EventHandler<GrabDropEventArgs> OnDrop;
	public event EventHandler<ServerUseEventArgs> OnUse;
	public event EventHandler<ServerUseEventArgs> OnUsing;
	public event EventHandler<ServerUseEventArgs> OnUnuse;
	public event EventHandler<ServerUseEventArgs> OnConvert;


	protected override void Awake()
	{
		base.Awake();

		if (this.grabDropControl == null || this.useControl == null)
		{
			throw new NullReferenceException("null controller detected");
		}

		// grab drop control DI
		{
			GrabDropInitArgs grabDropInitArgs = new GrabDropInitArgs();
			grabDropInitArgs.AddParentInstance(this);
			grabDropInitArgs.AddGetInfoFunc(() => { return this.Info; });
			grabDropInitArgs.AddGrabbedClientId(_grabbedClientId);
			grabDropInitArgs.AddGrabbedClientDefault(GRABBED_CLIENT_ID_DEFAULT);
			this.grabDropControl.OnGrab += (sender, args) => { this.OnGrab?.Invoke(sender, args); };
			this.grabDropControl.OnGrab += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnGrab, args.TargetHolderInfo.StrKey); };
			this.grabDropControl.OnDrop += (sender, args) => { this.OnDrop?.Invoke(sender, args); };
			this.grabDropControl.OnDrop += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnDrop, args.TargetHolderInfo.StrKey); };
			this.grabDropControl.DepsInit(grabDropInitArgs);
		}

		// use control DI
		{
			UseInitArgs useInitArgs = new UseInitArgs();
			useInitArgs.AddParentInstance(this);
			useInitArgs.AddGetInfoFunc(() => this.Info);
			useInitArgs.AddGetTargetFunc(() => null); // TODO: get target by calling IUsableSO Func(?) (have raycast in it or smth)
			this.useControl.OnUse += (sender, args) => { this.OnUse?.Invoke(sender, args); };
			this.useControl.OnUse += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.useControl.OnUsing += (sender, args) => { this.OnUsing?.Invoke(sender, args); };
			this.useControl.OnUsing += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.useControl.OnUnuse += (sender, args) => { this.OnUnuse?.Invoke(sender, args); };
			this.useControl.OnUnuse += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			this.useControl.OnConvert += (sender, args) => { this.OnConvert?.Invoke(sender, args); };
			this.useControl.OnConvert += (sender, args) =>
				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnUse, args.target.Info.StrKey); };
			useControl.DepsInit(useInitArgs);
		}
	}


	void IServerGrabbable.OnGrabServerInternal(IServerHolder targetHolder) => this.grabDropControl.OnGrabServerInternal(targetHolder);
	void IServerGrabbable.OnDropServerInternal() => this.grabDropControl.OnDropServerInternal();
	void IServerUsable.OnUseServerInternal() => this.useControl.OnUseServerInternal();
	void IServerUsable.OnUnuseServerInternal() => this.useControl.OnUnuseServerInternal();
}