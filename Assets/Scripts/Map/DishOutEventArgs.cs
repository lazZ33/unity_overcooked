using Unity;
using System;

public class DishOutEventArgs: EventArgs{
    public DishOutEventArgs(ICombinableSO dish){ this.Dish = dish; }
    public ICombinableSO Dish;
}