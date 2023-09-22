using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGameTimer: MonoBehaviour {
	public static double CurrentGameTime { get; private set; } = 0;
	public static GameTimerState State { get; private set; } = GameTimerState.Stop;

	private void Awake() {
		CurrentGameTime = 0;
	}

	private void FixedUpdate() {
		if (!(State == GameTimerState.Running))
			return;

		CurrentGameTime += Time.deltaTime;
	}

	public void TimerStart() {
		State = GameTimerState.Running;
	}

	public void TimerPause() {
		if (!(State == GameTimerState.Running))
			return;

		State = GameTimerState.Pause;
	}

	public void TimerContinue() {
		if (!(State == GameTimerState.Pause))
			return;

		State = GameTimerState.Running;
	}

	public void TimerStop() {
		State = GameTimerState.Stop;
		CurrentGameTime = 0;
	}


}
