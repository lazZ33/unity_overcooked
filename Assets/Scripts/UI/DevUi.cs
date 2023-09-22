using Unity;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class DevUi : NetworkBehaviour
{
	[SerializeField] private UIDocument _devUI;
	[SerializeField] private NetworkManager _networkManager;
	[SerializeField] private ServerGameManager _gameManager;
	[SerializeField] private ServerMapManager _mapManager;
	[SerializeField] private string _hostButtonName;
	[SerializeField] private string _clientButtonName;
	[SerializeField] private string _resetGameButtonName;
	[SerializeField] private string _disconnectButtonName;
	[SerializeField] private string _gameStartButtonName;
	[SerializeField] private string _mapSpawnName;
	[SerializeField] private string _mapDespawnName;

	void Start()
	{
		Button hostButton = this._devUI.rootVisualElement.Q<Button>(this._hostButtonName);
		Button clientButton = this._devUI.rootVisualElement.Q<Button>(this._clientButtonName);
		Button resetMapButton = this._devUI.rootVisualElement.Q<Button>(this._resetGameButtonName);
		Button disconnectButton = this._devUI.rootVisualElement.Q<Button>(this._disconnectButtonName);

		Button gameStartButton = this._devUI.rootVisualElement.Q<Button>(this._gameStartButtonName);
		Button mapSpawnButton = this._devUI.rootVisualElement.Q<Button>(this._mapSpawnName);
		Button mapDespawnButton = this._devUI.rootVisualElement.Q<Button>(this._mapDespawnName);

		hostButton.clicked += () => { this._networkManager.StartHost(); };
		clientButton.clicked += () => { this._networkManager.StartClient(); };
		resetMapButton.clicked += () => { this._gameManager.ResetGame(); };
		disconnectButton.clicked += () => { this._networkManager.Shutdown(); };

		gameStartButton.clicked += () => { this._gameManager.GameStart(); };
		mapSpawnButton.clicked += () => { this._mapManager.SpawnMap(); };
		mapDespawnButton.clicked += () => { this._mapManager.DespawnMap(); };
	}

}
