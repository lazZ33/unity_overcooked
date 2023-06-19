using Unity;
using System;

public class ScoreChangeEventArgs: EventArgs{
    public ScoreChangeEventArgs(int score) { this.Score = score; }
    public int Score;
}