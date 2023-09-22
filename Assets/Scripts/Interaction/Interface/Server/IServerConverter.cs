using System;

public interface IServerConverter: IServerInteractable {
	public new IConverterSO Info { get; }


	public event EventHandler<ServerConvertEventArgs> OnConvertStart;
	public event EventHandler<ServerConvertEventArgs> OnConverting;
	public event EventHandler<ServerConvertEventArgs> OnConvertEnd;
	public event EventHandler<ServerConvertEventArgs> OnConvert;


	public bool IsHoldToConvert { get; }


	internal void OnConvertStartServerInternal();
	internal void OnConvertEndServerInternal();
}