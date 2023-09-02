using System;

public class ClientUseEventArgs : EventArgs
{
	internal ClientUseEventArgs(IInteractableSO targetInfo) { this.targetInfo = targetInfo; }
	public IInteractableSO targetInfo;
}