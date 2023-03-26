using Unity;
using UnityEngine;
using Unity.Netcode;
using System;

public enum CellState{
    Walkable = 0,
    Unwalkable = 1,
    Table = 2,
    Utility = 3,
    IngredientSpawn = 4,
    PlayerSpawn = 5,
    Tool = 6,
    Entrance = 7,
    Exit = 8,
}