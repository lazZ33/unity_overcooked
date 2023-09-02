using System;

public interface IClientUsable : IClientInteractable
{
	public new IConverterSO Info { get; }


	public event EventHandler<ClientUseEventArgs> OnUse;
	public event EventHandler<ClientUseEventArgs> OnUsing;
	public event EventHandler<ClientUseEventArgs> OnUnuse;
	public event EventHandler<ClientUseEventArgs> OnConvert;


	internal bool IsHoldToConvert { get; }
}