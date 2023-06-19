using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Order{
    public Order(CombinableSO requestedDish, float warningTime, float overdueTime){
        this.RequestedDish = requestedDish;
        this.WarningTime = warningTime;
        this.OverdueTime = overdueTime;
    }
    public CombinableSO RequestedDish;
    public float WarningTime;
    public float OverdueTime;
    public int ScoreGain;
    public int ScoreLoss;
}