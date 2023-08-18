using Unity;
using System;

public class CombineEventArgs: EventArgs{
    internal CombineEventArgs(ICombinableSO info){ this.CombinedInfo = info; }
    public ICombinableSO CombinedInfo;
}