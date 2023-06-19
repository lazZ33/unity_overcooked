using Unity;
using System;

public class GrabDropEventArgs: EventArgs{
    internal GrabDropEventArgs(GrabbableSO info, object obj){ this.Info = info; this.Object = obj; }
    public GrabbableSO Info;
    public object Object; // for generic use
}