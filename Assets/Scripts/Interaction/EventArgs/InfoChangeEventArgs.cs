using Unity;
using System;

public class InfoChangeEventArgs: EventArgs{
    internal InfoChangeEventArgs(IInteractableSO info){ this.Info = info; }
    public IInteractableSO Info;
}