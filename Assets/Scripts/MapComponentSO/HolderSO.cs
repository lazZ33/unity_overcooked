using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class HolderSO: InteractableSO, IHolderSO{
    [SerializeField] public Vector3 LocalPlacePoint;

    public static new HolderSO GetSO(string strKey) => (HolderSO)InteractableSO.GetSO(strKey);

}