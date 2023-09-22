using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerScoreManager: NetworkBehaviour {
	[SerializeField] private ClientScoreManager _client;


	public static int GameScore { get; private set; } = 0;


	public void ScoreChange(int deltaScore) {
		GameScore += deltaScore;
		this._client.ScoreChangeCallbackClientRpc(GameScore);
	}

	public void ScoreReset() {
		GameScore = 0;
		this._client.ScoreChangeCallbackClientRpc(0);
	}
}
