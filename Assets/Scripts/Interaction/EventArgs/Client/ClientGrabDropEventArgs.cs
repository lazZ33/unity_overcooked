using Unity;
using System;

public class ClientGrabDropEventArgs : EventArgs
{
	internal ClientGrabDropEventArgs(IHolderSO info) { this.TargetHolderInfo = info; }
	public IHolderSO TargetHolderInfo;
}