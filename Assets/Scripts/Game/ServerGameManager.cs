using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using TNRD;
using System.Linq;

public class ServerGameManager: NetworkBehaviour{

    [SerializeField] private ClientGameManager _client;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private ServerMapManager _mapManager;
	[SerializeField] private GameObject _playerPrefab;
    [SerializeField] private SerializableInterface<ICombinableSO>[] _targetDishes;
    [SerializeField] private int _playerAmount = 4;
    [SerializeField] private double _gameDurationInit;
    [SerializeField] private double _newOrderTimeInterval;
    [SerializeField] private double _orderWarningTime = 20;
    [SerializeField] private double _orderOverdueTime = 27;
    [SerializeField] private int _maxOrderCount = 7;

    public event EventHandler<OrderListChangeEventArgs> OnNewOrder;
    public event EventHandler<OrderListChangeEventArgs> OnFinishedOrder;
    public event EventHandler<OrderListChangeEventArgs> OnOrderWarning;
    public event EventHandler<OrderListChangeEventArgs> OnOrderOverdue;
    public event EventHandler<ScoreChangeEventArgs> OnScoreChange;


    public static double CurGameTime => ServerGameManager._networkTimeSystem.ServerTime;
    public static double TimeLeft => ServerGameManager._gameDuration.Value - (ServerGameManager.CurGameTime - ServerGameManager._gameStartTime.Value);
    public static bool IsGameStarted => ServerGameManager._gameStartTime.Value != 0;


	private Vector3[] _initialSpawnPoints;
	private int _initialSpawnPointsIdx = 0;

    private static NetworkTimeSystem _networkTimeSystem = NetworkTimeSystem.ServerTimeSystem();
	private static NetworkVariable<double> _gameStartTime = new NetworkVariable<double>(0);
	private static NetworkVariable<double> _gameDuration;
    private double _lastNewOrderTime;

    private int _gameScore;
    private List<Order> _existingOrders;
    private IScoreCalculator _scoreCalculator = new EasyScoreCalculator();

    void Awake(){
		ServerGameManager._gameDuration = new NetworkVariable<double>(this._gameDurationInit);

		// client connect callback called before serverStarted
        // solution(?): add IsServer check in callback functions
		// https://forum.unity.com/threads/order-of-callbacks-when-starting-a-host.1349018/
		this._networkManager.OnServerStarted += this.SpawnMap;
        this._networkManager.OnServerStarted += this.SpawnPlayerIfHost;
        this._networkManager.OnClientConnectedCallback += this.SpawnPlayer;

        // this._networkManager.OnCleintConnectedCallback = (TODO: UI callback to client for starting lobby UI)
    }

	public void GameStart()
    {
        if (!this.IsServer) return;
        print("Game Start");

        // TODO: UI events to client for starting in-game UI

        ServerGameManager._gameStartTime.Value = ServerGameManager.CurGameTime;

        this._mapManager.OnDishOut += this.OnDishOut;

        this._lastNewOrderTime = ServerGameManager.CurGameTime;
        this._existingOrders = new List<Order>(this._maxOrderCount);

        for (int i = 0; i < this._maxOrderCount; i++){
            this.AddNewRandomOrder(out Order newOrder);
        }

        this.OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(0, this._gameScore));
        this._client.GameStateChangeCallbackClientRpc(null, 0, this._gameScore, GameStateChangeCallbackID.OnScoreChange);
    }

    private void GamePause()
    {
        if (!this.IsServer) return;

        // TODO: game pause UI, pause timer
    }

    private void GameEnd()
    {
        if (!this.IsServer) return;

        // TODO: ending animations, pass control to the next scene
    }

    private void SpawnMap(){
		if (!this.IsServer) return;

        ICombinableSO[] targetDishes = new ICombinableSO[this._targetDishes.Length];
        for(int i = 0; i < this._targetDishes.Length; i++) {
            targetDishes[i] = this._targetDishes[i].Value;
        }

		Vector2Int[] spawnCells = _mapManager.GenerateMapArray(targetDishes, this._playerAmount);
		this._initialSpawnPoints = new Vector3[spawnCells.Length];
		for (int i = 0; i < spawnCells.Length; i++){
			this._initialSpawnPoints[i] = this._mapManager.GetWorldCoor(spawnCells[i]);
        }
		this._mapManager.SpawnMap();
	}

    private void SpawnPlayerIfHost()
    {
        if (this.IsHost) this.SpawnPlayer(this.OwnerClientId);
    }

	private void SpawnPlayer(ulong clientId)
	{
        if (!this.IsServer) return;

		Vector2 spawnPoint = this._initialSpawnPoints[this._initialSpawnPointsIdx];
		this._initialSpawnPointsIdx = this._initialSpawnPointsIdx++ % this._initialSpawnPoints.Length;

		GameObject newPlayerInstance = Instantiate(this._playerPrefab, spawnPoint, Quaternion.identity);
		newPlayerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        Debug.Log(String.Format("New player spawned with client Id {0}", clientId));
	}

    private void AddNewRandomOrder(out Order newOrder)
    {
        if (!this.IsServer) { newOrder = null;  return; }
        print("AddNewRandomOrder");

        int randomIdx = (int)(UnityEngine.Random.value) * int.MaxValue % this._targetDishes.Length;
        ICombinableSO newRandomTargetDish = this._targetDishes[randomIdx].Value;
        double newOrderWarningTime = ServerGameManager.CurGameTime + this._orderWarningTime;
        double newOrderOverdueTime = ServerGameManager.CurGameTime + this._orderOverdueTime;

        newOrder = new Order(newRandomTargetDish, newOrderWarningTime, newOrderOverdueTime, this._scoreCalculator);
        this._existingOrders.Add(newOrder);
        
        this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newRandomTargetDish));
        this._client.GameStateChangeCallbackClientRpc(newRandomTargetDish.StrKey, 0, 0, GameStateChangeCallbackID.OnNewOrder);
    }

    private void OnDishOut(object sender, DishOutEventArgs args)
    {
        if (!this.IsServer) return;

        Order matchingOrder = this._existingOrders.Find(order => order.RequestedDish == args.Dish);

        if (matchingOrder != null){

            int previousScore = this._gameScore;
            this._gameScore += matchingOrder.ScoreGain;

            this.OnFinishedOrder?.Invoke(this, new OrderListChangeEventArgs(matchingOrder.RequestedDish));
            this._client.GameStateChangeCallbackClientRpc(matchingOrder.RequestedDish.StrKey, 0, 0, GameStateChangeCallbackID.OnFinishedOrder);
            this.OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(previousScore, this._gameScore));
            this._client.GameStateChangeCallbackClientRpc(null, previousScore, this._gameScore, GameStateChangeCallbackID.OnScoreChange);
        }
    }

    void FixedUpdate(){
        if (!this.IsServer | !ServerGameManager.IsGameStarted) return;

        if (ServerGameManager.CurGameTime >= ServerGameManager._gameDuration.Value) {
            this.GameEnd();
            return;
        }

        // check if any order warning/overdue
        foreach (Order curOrder in this._existingOrders){
            if (ServerGameManager.CurGameTime >= curOrder.OverdueTime){
                this._existingOrders.Remove(curOrder);

                int previousScore = this._gameScore;
                this._gameScore -= curOrder.ScoreLoss;

                this.OnOrderOverdue?.Invoke(this, new OrderListChangeEventArgs(curOrder.RequestedDish));
                this._client.GameStateChangeCallbackClientRpc(curOrder.RequestedDish.StrKey, 0, 0, GameStateChangeCallbackID.OnOrderOverdue);
                this.OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(previousScore, this._gameScore));
                this._client.GameStateChangeCallbackClientRpc(null, previousScore, this._gameScore, GameStateChangeCallbackID.OnScoreChange);
            }
            else if (ServerGameManager.CurGameTime >= curOrder.WarningTime){
                this.OnOrderWarning?.Invoke(this, new OrderListChangeEventArgs(curOrder.RequestedDish));
                this._client.GameStateChangeCallbackClientRpc(curOrder.RequestedDish.StrKey, 0, 0, GameStateChangeCallbackID.OnOrderWarning);
            }

        }

        // periodically add new random orders
        if (this._existingOrders.Count < this._maxOrderCount && ServerGameManager.CurGameTime >= this._lastNewOrderTime + this._newOrderTimeInterval){
            this.AddNewRandomOrder(out Order newOrder);

            this.OnNewOrder?.Invoke(this, new OrderListChangeEventArgs(newOrder.RequestedDish));
            this._client.GameStateChangeCallbackClientRpc(newOrder.RequestedDish.StrKey, 0, 0, GameStateChangeCallbackID.OnNewOrder);
        }

    }

}