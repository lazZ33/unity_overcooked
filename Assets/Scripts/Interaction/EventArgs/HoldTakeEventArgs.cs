

using System;

public class HoldTakeEventArgs: EventArgs {
	internal HoldTakeEventArgs(IGrabbableSO targetGrabbableInfo) { this.TargetGrabbableInfo = targetGrabbableInfo; }
	public IGrabbableSO TargetGrabbableInfo;
}