using System;

public class ServerConvertEventArgs: EventArgs {
	internal ServerConvertEventArgs(IServerCombinable target) { this.target = target; }
	public IServerCombinable target;
}