//using System;
//using System.Runtime.InteropServices.WindowsRuntime;
//using Unity;
//using Unity.Netcode;
//using UnityEngine;

//using GrabDropInitArgs = ServerGrabDropControl.GrabDropControlInitArgs;
//using UseInitArgs = ServerConvertControl.UseControlInitArgs;

//internal class ServerTool : ServerInteractable, IServerGrabbable
//{
//	[SerializeField] private ServerGrabDropControl grabDropControl;
//	[SerializeField] private ServerConvertControl useControl;


//	// DI variables
//	private NetworkVariable<ulong> _grabbedClientId = new NetworkVariable<ulong>(GRABBED_CLIENT_ID_DEFAULT);
//	private static readonly ulong GRABBED_CLIENT_ID_DEFAULT = ulong.MaxValue;


//	IGrabbableSO IServerGrabbable.Info => (IGrabbableSO)base._info.Value;


//	bool IServerGrabbable.IsGrabbedByPlayer => this.grabDropControl.IsGrabbedByPlayer;
//	bool IServerGrabbable.IsGrabbedByLocal => this.grabDropControl.IsGrabbedByPlayer;


//	bool IServerGrabbable.CanPlaceOn(IServerHolder targetHolder) => ((IGrabbableSO)base._info.Value).CanPlaceOn(targetHolder.Info);


//	public event EventHandler<ServerGrabDropEventArgs> OnGrab;
//	public event EventHandler<ServerGrabDropEventArgs> OnDrop;
//	public event EventHandler<ServerUseEventArgs> OnConvertStart;
//	public event EventHandler<ServerUseEventArgs> OnConverting;
//	public event EventHandler<ServerUseEventArgs> OnConvertEnd;
//	public event EventHandler<ServerUseEventArgs> OnConvert;


//	protected override void Awake()
//	{
//		base.Awake();

//		if (this.grabDropControl == null || this.useControl == null)
//		{
//			throw new NullReferenceException("null controller detected");
//		}

//		// grab drop control DI
//		{
//			GrabDropInitArgs grabDropInitArgs = new GrabDropInitArgs();
//			grabDropInitArgs.AddParentInstance(this);
//			grabDropInitArgs.AddGetInfoFunc(() => { return this.Info; });
//			grabDropInitArgs.AddGrabbedClientId(_grabbedClientId);
//			grabDropInitArgs.AddGrabbedClientDefault(GRABBED_CLIENT_ID_DEFAULT);
//			this.grabDropControl.OnGrab += (sender, args) => { this.OnGrab?.Invoke(sender, args); };
//			this.grabDropControl.OnGrab += (sender, args) =>
//				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnGrab, args.GrabbableInfo.StrKey); };
//			this.grabDropControl.OnDrop += (sender, args) => { this.OnDrop?.Invoke(sender, args); };
//			this.grabDropControl.OnDrop += (sender, args) =>
//				{ base._client.InteractionEventCallbackClientRpc(InteractionCallbackID.OnDrop, args.GrabbableInfo.StrKey); };
//			this.grabDropControl.DepsInit(grabDropInitArgs);
//		}

//		//// TODO: use control DI
//		//}
//	}


//	void IServerGrabbable.OnGrabServerInternal(IServerHolder targetHolder) => this.grabDropControl.OnGrabServerInternal(targetHolder);
//	void IServerGrabbable.OnDropServerInternal() => this.grabDropControl.OnDropServerInternal();
//}