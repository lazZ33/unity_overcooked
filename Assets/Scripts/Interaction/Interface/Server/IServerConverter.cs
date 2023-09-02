using System;

public interface IServerConverter: IServerInteractable
{
	public new IConverterSO Info { get; }


	public event EventHandler<ServerUseEventArgs> OnConvertStart;
	public event EventHandler<ServerUseEventArgs> OnConverting;
	public event EventHandler<ServerUseEventArgs> OnConvertEnd;
	public event EventHandler<ServerUseEventArgs> OnConvert;


	public bool IsHoldToConvert { get; }


	internal void OnConvertStartServerInternal();
	internal void OnConvertEndServerInternal();
}