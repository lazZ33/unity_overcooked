using System;

public interface IServerUsable: IServerInteractable
{
	public new IUsableSO Info { get; }


	public event EventHandler<ServerUseEventArgs> OnUse;
	public event EventHandler<ServerUseEventArgs> OnUsing;
	public event EventHandler<ServerUseEventArgs> OnUnuse;
	public event EventHandler<ServerUseEventArgs> OnConvert;


	public bool IsHoldToUse { get; }


	internal void OnUseServerInternal();
	internal void OnUnuseServerInternal();
}