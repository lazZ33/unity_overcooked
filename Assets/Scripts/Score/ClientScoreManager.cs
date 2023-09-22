using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ClientScoreManager: NetworkBehaviour {
	[SerializeField] private ServerScoreManager _server;


	public static int GameScore { get; private set; } = 0;


	public event EventHandler<ScoreChangeEventArgs> OnScoreChange;


	[ClientRpc]
	internal void ScoreChangeCallbackClientRpc(int score) {
		this.OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(ClientScoreManager.GameScore, score));
		ClientScoreManager.GameScore = score;
	}
}
