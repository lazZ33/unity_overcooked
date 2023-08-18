using UnityEngine;
using Unity.Netcode;
using System;

public class ClientTool: ClientInteractable, IClientUsable
{
	private new ServerTool _server => (ServerTool)base._server;
	private new ToolSO Info => (ToolSO)base._info;
	private new ToolSO _info => (ToolSO)base._info;


	IUsableSO IClientUsable.Info => (IUsableSO)base._info;
	bool IClientUsable.IsHoldToUse => ((IUsableSO)base._info).IsHoldToUse;


	public event EventHandler<ServerUseEventArgs> OnUse;
	public event EventHandler<ServerUseEventArgs> OnUsing;
	public event EventHandler<ServerUseEventArgs> OnUnuse;
	public event EventHandler<ServerUseEventArgs> OnConvert;
}