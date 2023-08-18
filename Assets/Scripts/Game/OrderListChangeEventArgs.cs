using Unity;
using System;

public class OrderListChangeEventArgs: EventArgs{
    public OrderListChangeEventArgs(ICombinableSO requestedDish) { this.RequestedDish = requestedDish; }
    public ICombinableSO RequestedDish;
}