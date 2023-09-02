//using UnityEngine;
//using Unity.Netcode;
//using System;

//public class ClientTool: ClientInteractable, IClientUsable
//{
//	private new ServerTool _server => (ServerTool)base._server;
//	private new ToolSO Info => (ToolSO)base._info;
//	private new ToolSO _info => (ToolSO)base._info;


//	IConverterSO IClientUsable.Info => (IConverterSO)base._info;
//	bool IClientUsable.IsHoldToUse => ((IConverterSO)base._info).IsHoldToConvert;


//	public event EventHandler<ClientUseEventArgs> OnUse;
//	public event EventHandler<ClientUseEventArgs> OnUsing;
//	public event EventHandler<ClientUseEventArgs> OnUnuse;
//	public event EventHandler<ClientUseEventArgs> OnConvert;
//}