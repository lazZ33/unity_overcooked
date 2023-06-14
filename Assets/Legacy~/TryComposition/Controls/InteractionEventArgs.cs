using Unity;
using System;

public class InteractionEventArgs: EventArgs{
    internal InteractionEventArgs(InteractableCompositionSO info, object obj){ this.Info = info; this.Object = obj; }
    public InteractableCompositionSO Info;
    public object Object; // for generic use
}