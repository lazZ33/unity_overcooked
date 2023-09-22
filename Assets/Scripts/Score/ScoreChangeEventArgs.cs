using Unity;
using System;

public class ScoreChangeEventArgs: EventArgs {
	public ScoreChangeEventArgs(int preivousScore, int currentScore) {
		this.PreviousScore = preivousScore;
		this.CurrentScore = currentScore;
	}
	public int PreviousScore;
	public int CurrentScore;
}