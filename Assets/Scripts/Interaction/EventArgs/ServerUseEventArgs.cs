using System;
using System;

public class ServerUseEventArgs : EventArgs
{
	internal ServerUseEventArgs(IServerInteractable target) { this.target = target; }
	public IServerInteractable target;
}