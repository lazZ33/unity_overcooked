using Unity;
using System;

public class UseEventArgs: EventArgs{
    internal UseEventArgs(GrabbableSO holdingGrabbableInfo){ this.holdingGrabbableInfo = holdingGrabbableInfo; }
    public GrabbableSO holdingGrabbableInfo;
}