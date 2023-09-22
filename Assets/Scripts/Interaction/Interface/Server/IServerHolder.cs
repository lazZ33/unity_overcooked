using System;
using Unity;

public interface IServerHolder: IServerInteractable {
	public new IHolderSO Info { get; }
	public bool IsHoldingGrabbable { get; }
	public IServerGrabbable HoldGrabbable { get; }
	public ulong OwnerClientId { get; }


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;


	internal void OnHoldServerInternal(IServerGrabbable targetGrabbable);
	internal void OnTakeServerInternal(out IServerGrabbable takenGrabbable);
}