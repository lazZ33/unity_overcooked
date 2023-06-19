using Unity;
using UnityEngine;
using Unity.Netcode;
using System;

public enum CellState{
    Walkable,
    Unwalkable,
    Table,
    Utility,
    IngredientSpawn,
    PlayerSpawn,
    Tool,
    DishExit,
}