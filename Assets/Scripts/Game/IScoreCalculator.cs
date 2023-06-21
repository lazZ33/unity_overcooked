using Unity;
using System;

internal interface IScoreCalculator{
	internal int GetScoreGain(CombinableSO targetCombinable);
	internal int GetScoreLoss(CombinableSO targetCombinable);
}