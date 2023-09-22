using Unity;
using System;

internal interface IScoreCalculator {
	internal int GetScoreGain(ICombinableSO targetCombinable);
	internal int GetScoreLoss(ICombinableSO targetCombinable);
}