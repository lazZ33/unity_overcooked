using System;
using Unity;

public interface IClientHolder: IClientInteractable {
	public new IHolderSO Info { get; }
	public ulong OwnerClientId { get; }


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;
}