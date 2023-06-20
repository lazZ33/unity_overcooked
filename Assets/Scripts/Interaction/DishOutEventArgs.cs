using Unity;
using System;

public class DishOutEventArgs: EventArgs{
    internal DishOutEventArgs(GrabbableSO dish){ this.Dish = dish; }
    public GrabbableSO Dish;
}