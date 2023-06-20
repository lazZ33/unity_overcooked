using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "DishExit", menuName = "ScriptableObject/DishExit")]
public class DishExitSO: HolderSO{
    public static new DishExitSO GetSO(string strKey) => (DishExitSO)InteractableSO.GetSO(strKey);

}