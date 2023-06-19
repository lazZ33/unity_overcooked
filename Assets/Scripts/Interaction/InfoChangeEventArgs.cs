using Unity;
using System;

public class InfoChangeEventArgs: EventArgs{
    public InfoChangeEventArgs(InteractableSO info){ this.Info = info; }
    public InteractableSO Info;
}