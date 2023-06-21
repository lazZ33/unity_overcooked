using Unity;
using System;

public class DishOutEventArgs: EventArgs{
    public DishOutEventArgs(GrabbableSO dish){ this.Dish = dish; }
    public GrabbableSO Dish;
}