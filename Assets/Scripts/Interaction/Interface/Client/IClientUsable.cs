using System;

public interface IClientUsable : IClientInteractable
{
	public new IUsableSO Info { get; }


	public event EventHandler<ServerUseEventArgs> OnUse;
	public event EventHandler<ServerUseEventArgs> OnUsing;
	public event EventHandler<ServerUseEventArgs> OnUnuse;
	public event EventHandler<ServerUseEventArgs> OnConvert;


	internal bool IsHoldToUse { get; }
}