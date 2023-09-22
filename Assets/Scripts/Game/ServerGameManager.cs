using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using TNRD;
using System.Linq;

public class ServerGameManager: NetworkBehaviour {

	[SerializeField] private ClientGameManager _client;
	[SerializeField] private NetworkManager _networkManager;
	[SerializeField] private ServerMapManager _mapManager;
	[SerializeField] private ServerOrderManager _orderManager;
	[SerializeField] private ServerScoreManager _scoreManager;
	[SerializeField] private ServerGameTimer _gameTimer;


	[SerializeField] private GameObject _playerPrefab;
	[SerializeField] private SerializableInterface<ICombinableSO>[] _targetDishes;
	[SerializeField] private int _playerAmount = 4;
	[SerializeField] private double _gameDuration;


	private Vector3[] _initialSpawnPoints;
	private int _initialSpawnPointsIdx = 0;


	void Awake() {

		// client connect callback called before serverStarted
		// solution(?): add IsServer check in callback functions
		// https://forum.unity.com/threads/order-of-callbacks-when-starting-a-host.1349018/
		this._networkManager.OnServerStarted += this.SpawnMap;
		//this._networkManager.OnServerStarted += this.SpawnPlayerIfHost;
		this._networkManager.OnClientConnectedCallback += this.SpawnPlayer;

		// this._networkManager.OnCleintConnectedCallback = (TODO: UI callback to client for starting lobby UI)
	}

	public void ResetGame() {
		this._mapManager.ResetMapArray();
		this._mapManager.DespawnMap();
		this._gameTimer.TimerStop();
		this._scoreManager.ScoreReset();
		this._orderManager.OrderClear();
		this._networkManager.DisconnectClient(this.OwnerClientId);
	}

	public void GameStart() {
		if (!this.IsServer)
			return;
		print("Game Start");

		// TODO: UI events to client for starting in-game UI

		this._gameTimer.TimerStart();
		this._scoreManager.ScoreReset();

		ICombinableSO[] targetDishes = new ICombinableSO[this._targetDishes.Length];
		for (int i = 0; i < this._targetDishes.Length; i++) {
			targetDishes[i] = this._targetDishes[i].Value;
		}
		this._orderManager.OrderInit(targetDishes);
	}

	private void GamePause() {
		if (!this.IsServer)
			return;

		this._gameTimer.TimerPause();
		// TODO: game pause UI
	}

	private void GameContinue() {
		if (this.IsServer)
			return;

		this._gameTimer.TimerContinue();
		// TODO: close game pause UI
	}

	private void GameEnd() {
		if (!this.IsServer)
			return;

		this._gameTimer.TimerStop();
		// TODO: ending animations, pass control to the next scene
	}

	private void SpawnMap() {
		if (!this.IsServer)
			return;

		ICombinableSO[] targetDishes = new ICombinableSO[this._targetDishes.Length];
		for (int i = 0; i < this._targetDishes.Length; i++) {
			targetDishes[i] = this._targetDishes[i].Value;
		}

		Vector2Int[] spawnCells = _mapManager.GenerateMapArray(targetDishes, this._playerAmount);
		this._initialSpawnPoints = new Vector3[spawnCells.Length];
		for (int i = 0; i < spawnCells.Length; i++) {
			this._initialSpawnPoints[i] = this._mapManager.GetWorldCoor(spawnCells[i]);
		}
		this._mapManager.SpawnMap();
	}

	private void SpawnPlayer(ulong clientId) {
		if (!this.IsServer)
			return;

		Vector2 spawnPoint = this._initialSpawnPoints[this._initialSpawnPointsIdx];
		this._initialSpawnPointsIdx = this._initialSpawnPointsIdx++ % this._initialSpawnPoints.Length;

		GameObject newPlayerInstance = Instantiate(this._playerPrefab, spawnPoint, Quaternion.identity, this.transform);
		newPlayerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
		Debug.Log(String.Format("New player spawned with client Id {0},\nLocation x:{1} y:{2}", clientId, spawnPoint.x, spawnPoint.y));
	}

	void FixedUpdate() {
		if (!this.IsServer | ServerGameTimer.State != GameTimerState.Running)
			return;

		this._mapManager.OnMapUpdate();
		this._orderManager.OnOrderUpdate();

		if (ServerGameTimer.CurrentGameTime >= this._gameDuration) {
			this.GameEnd();
			return;
		}
	}

}