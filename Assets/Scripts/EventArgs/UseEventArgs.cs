using Unity;
using System;

public class UseEventArgs: EventArgs{
    public UseEventArgs(GrabbableSO holdingGrabbableInfo){ this.holdingGrabbableInfo = holdingGrabbableInfo; }
    public GrabbableSO holdingGrabbableInfo;
}