using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ServerMapManager: NetworkBehaviour
{
    [SerializeField] private ClientMapManager _client;
    [SerializeField] private ServerInteractionManager _interactions;
    [SerializeField] private int _mapXSize, _mapYSize, _mapZHeight;
    [SerializeField] private int cellXSize, cellYSize;
    [SerializeField] private Vector3 _origin;
    [SerializeField] private MapComponentPrefabSO _mapComponentPrefabs;
    [SerializeField] private BasicComponentSO _basicComponentSO;
    [SerializeField] private int _interactableGroupAmount;
    [SerializeField] private int _minSpawnAmount = 0;
    [SerializeField] private int _minUtilityAmount = 0;
    [SerializeField] private int _toolAmount = 0;

    public NetworkVariable<int> TargetDishesAmount { get; } = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkList<FixedString128Bytes> TargetDishesSOStrKeys { get; private set; }

    public event EventHandler OnMapDespawn;
    public event EventHandler<DishOutEventArgs> OnDishOut;

    private CellState[,] _mapCellState;
    private CombinableSO[] _targetDishesSO;
    private ServerMapGenerator _mapGenerator;
    private List<CombinableSO> _requiredCombinableSOList;
    private List<UsableHolderSO> _requiredUsableHolderSOList;

    private void OnDishOutCallback(object sender, DishOutEventArgs args) => this.OnDishOut?.Invoke(sender, args);

    void Awake(){
        this.TargetDishesSOStrKeys = new NetworkList<FixedString128Bytes>(new FixedString128Bytes[10], NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        this._mapGenerator = new ServerMapGenerator();
    }

    public void GenerateMap(CombinableSO[] targetDishesSO){
        if (!this.IsServer) return;

        this._targetDishesSO = targetDishesSO;

        this.TargetDishesAmount.Value = this._targetDishesSO.Length;
		//for (int i = 1; i < this._targetDishesSO.Length; i++){
		//	this.TargetDishesSOStrKeys[i] = this._targetDishesSO[i].StrKey;
		//	Debug.Log(this._targetDishesSO[i]);
		//}
        foreach (CombinableSO targetDish in this._targetDishesSO)
            TargetDishesSOStrKeys.Add(new FixedString128Bytes(targetDish.StrKey));


		this.IdentifyRquiredInteractableList(this._targetDishesSO, out this._requiredCombinableSOList, out this._requiredUsableHolderSOList);

        int spawnAmount = this._requiredCombinableSOList.Count > this._minSpawnAmount ? this._requiredCombinableSOList.Count : this._minSpawnAmount;
        int utilityAmount = this._requiredUsableHolderSOList.Count > this._minUtilityAmount ? this._requiredUsableHolderSOList.Count : this._minUtilityAmount;

        this._mapCellState = this._mapGenerator.GenerateMapArray(this._mapXSize, this._mapYSize, utilityAmount, spawnAmount, this._toolAmount, this._interactableGroupAmount);
    }

	public void DespawnMap(){
		this.OnMapDespawn?.Invoke(this, EventArgs.Empty);
	}

	public void SpawnMap(){
        if (!this.IsServer) return;

        int spawnableIdx = 0;
        int utilityIdx = 0;
        ServerInteractable curInteractable;

        HelperFunc.Shuffle(this._requiredCombinableSOList);

        for (int x = 0; x < _mapXSize; x++){
            for (int y = 0; y < _mapYSize; y++){
                switch (this._mapCellState[x,y]){
                    case CellState.Walkable:
                        break;
                    case CellState.Unwalkable:
                    case CellState.Table:
                        GameObject tableOnly = Instantiate(this._mapComponentPrefabs.TablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        ServerTable tableServer = tableOnly.GetComponent<ServerTable>();
                        tableServer.InfoInit(this._basicComponentSO.TableSO);
                        tableServer.GetComponent<NetworkObject>().Spawn();
                        this.OnMapDespawn += tableServer.OnMapDespawn;
						break;
                    case CellState.Utility:
                        GameObject stationeryUtility = Instantiate(this._mapComponentPrefabs.UtilityPrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        ServerUsableHolder stationeryUtilityServer = stationeryUtility.GetComponent<ServerUsableHolder>();
                        stationeryUtilityServer.InfoInit(this._requiredUsableHolderSOList[utilityIdx]);
                        utilityIdx = (utilityIdx+1) % this._requiredUsableHolderSOList.Count;

                        stationeryUtilityServer.NetworkObjectBuf.Spawn();
						this.OnMapDespawn += stationeryUtilityServer.OnMapDespawn;
						break;
                    case CellState.IngredientSpawn:
                        GameObject spawnable = Instantiate(this._mapComponentPrefabs.SpawnablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        ServerSpawnable spawnableServer = spawnable.GetComponent<ServerSpawnable>();
                        spawnableServer.SpawnningCombinableInfo = this._requiredCombinableSOList[spawnableIdx];
                        spawnableServer.InfoInit(this._basicComponentSO.SpawnableSO);
                        spawnableIdx = (spawnableIdx+1) % this._requiredCombinableSOList.Count;

                        spawnableServer.NetworkObjectBuf.Spawn();
						this.OnMapDespawn += spawnableServer.OnMapDespawn;
						break;
                    case CellState.Tool:
                        GameObject table = Instantiate(this._mapComponentPrefabs.TablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);
                        tableServer = table.GetComponent<ServerTable>();
                        tableServer.InfoInit(this._basicComponentSO.TableSO);
                        table.GetComponent<NetworkObject>().Spawn();
						this.OnMapDespawn += tableServer.OnMapDespawn;

						tableServer = table.GetComponent<ServerTable>();
                        ServerTool toolServer = Instantiate(this._mapComponentPrefabs.ToolPrefab, this.GetWorldCoor(x, y), Quaternion.identity).GetComponent<ServerTool>();
                        // TODO: toolServer.InfoInit(xxx);
                        toolServer.NetworkObjectBuf.Spawn();
                        this._interactions.PlaceToServerInternal(toolServer, tableServer);

						this.OnMapDespawn += toolServer.OnMapDespawn;
						break;
                    case CellState.DishExit:
                        GameObject dishExit = Instantiate(this._mapComponentPrefabs.DishExitPrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        ServerDishExit dishExitServer = dishExit.GetComponent<ServerDishExit>();
                        dishExitServer.InfoInit(this._basicComponentSO.DishExitSO);
                        dishExitServer.GetComponent<NetworkObject>().Spawn();

						this.OnMapDespawn += dishExitServer.OnMapDespawn;
                        dishExitServer.OnDishOut += this.OnDishOutCallback;
						break;
                    default:
                        Debug.LogWarning(String.Format("invalid CellState found when populating map, x:{0}, y:{1}", x, y));
                        break;
                }
            }
        }
    }

    private void IdentifyRquiredInteractableList(CombinableSO[] targetDishesSO, out List<CombinableSO> requiredCombinableSOList, out List<UsableHolderSO> requiredUsableHolderSOList){
        if (!this.IsServer) { requiredCombinableSOList = null; requiredUsableHolderSOList = null; return;}

        CombinableSO.GetRequiredBaseSO(targetDishesSO, out requiredCombinableSOList, out requiredUsableHolderSOList);
        CombinableSO.LoadAllRequiredSO(requiredCombinableSOList, requiredUsableHolderSOList);

        string combinableListMsg = String.Format("{0} Combinables loaded:", requiredCombinableSOList.Count);
        string stationeryUtilityListMsg = String.Format("{0} StationeryUtilities loaded:", requiredUsableHolderSOList.Count);
        foreach (CombinableSO curCombinableSO in requiredCombinableSOList) combinableListMsg += " " + curCombinableSO.name + ",";
        foreach (UsableHolderSO curUsableHolderSO in requiredUsableHolderSOList) stationeryUtilityListMsg += " " + curUsableHolderSO.name + ",";
        print(combinableListMsg);
        print(stationeryUtilityListMsg);

        return;
    }

    void OnDrawGizmos(){
        if (!this.IsServer) return;
        if (this._mapCellState == null) return;

        for (int x = 0; x < this._mapXSize; x++){
            for (int y = 0; y < this._mapYSize; y++){
                switch (this._mapCellState[x,y]){
                    case CellState.Walkable:
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), new Vector3(cellXSize, cellYSize, 1));
                        break;
                    case CellState.Unwalkable:
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), new Vector3(cellXSize, cellYSize, 1));
                        break;
                    case CellState.Utility:
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), (float) cellXSize);
                        break;
                    case CellState.IngredientSpawn:
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), (float) cellXSize);
                        break;
                    case CellState.DishExit:
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), (float) cellXSize);
                        break;
                    // case CellState.Tool:
                    // case CellState.Table:
                    default:
                        Debug.LogWarning("invalid CellState found when drawing map");
                        break;
                }
            }
        }
    }

    public void SetOrigin(int originX, int originY){
        if (this._origin != null) { Debug.LogWarning("Duplicate assignment on the origin of grid system"); return;}
        this._origin = new Vector3(originX, originY, 0);
    }
    public Vector2 GetGridCoor(float x, float y){
        return new Vector2((int)Math.Floor((x - (this._origin.x)/this.cellXSize)), (int)Math.Floor((y - (this._origin.y)/this.cellYSize)));
    }
    public Vector3 GetWorldCoor(int x, int y){
        return new Vector3(this._origin.x + this.cellXSize * x, this._mapZHeight, this._origin.y + this.cellYSize * y);
    }
}
