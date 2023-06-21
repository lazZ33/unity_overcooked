using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

internal class Order{
    internal Order(CombinableSO requestedDish, double warningTime, double overdueTime, IScoreCalculator scoreCalculator){
        this.RequestedDish = requestedDish;
        this.WarningTime = warningTime;
        this.OverdueTime = overdueTime;
        this.ScoreGain = scoreCalculator.GetScoreGain(requestedDish);
        this.ScoreLoss = scoreCalculator.GetScoreLoss(requestedDish);
    }
    public CombinableSO RequestedDish;
    public double WarningTime;
    public double OverdueTime;
    public int ScoreGain;
    public int ScoreLoss;
}