using Unity;
using System;

public class ServerGrabDropEventArgs: EventArgs {
	internal ServerGrabDropEventArgs(IGrabbableSO grabbableInfo, object obj) { this.GrabbableInfo = grabbableInfo; this.Object = obj; }
	public IGrabbableSO GrabbableInfo;
	public object Object; // for generic use
}