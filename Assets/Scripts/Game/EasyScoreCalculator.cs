using Unity;
using System;

public class EasyScoreCalculator : IScoreCalculator{
	int IScoreCalculator.GetScoreGain(CombinableSO targetCombinable){
		return targetCombinable.InteractablesRequired * 3;
	}

	int IScoreCalculator.GetScoreLoss(CombinableSO targetCombinable)
	{
		return targetCombinable.InteractablesRequired;
	}
}