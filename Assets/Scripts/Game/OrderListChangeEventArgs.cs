using Unity;
using System;

public class OrderListChangeEventArgs: EventArgs{
    public OrderListChangeEventArgs(CombinableSO requestedDish) { this.RequestedDish = requestedDish; }
    public CombinableSO RequestedDish;
}