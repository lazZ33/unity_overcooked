using Unity;
using System;

public class EasyScoreCalculator : IScoreCalculator{
	int IScoreCalculator.GetScoreGain(ICombinableSO targetCombinable){
		return targetCombinable.RequiredCombinables.Length * 3;
	}

	int IScoreCalculator.GetScoreLoss(ICombinableSO targetCombinable)
	{
		return targetCombinable.RequiredCombinables.Length;
	}
}