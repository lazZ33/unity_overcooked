using Unity;
using System;

public class GrabDropEventArgs : EventArgs
{
	internal GrabDropEventArgs(IGrabbableSO info, object obj) { this.TargetHolderInfo = info; this.Object = obj; }
	public IGrabbableSO TargetHolderInfo;
	public object Object; // for generic use
}