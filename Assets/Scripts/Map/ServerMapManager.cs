using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

// public class Point{
//     public int x, y;
//     public Point(int x, int y){
//         this.x = x;
//         this.y = y;
//     }
//     public static Point operator-(Point pt1, Point pt2){ return new Point(pt1.x - pt2.x, pt1.y - pt2.y); }
//     public static Point operator+(Point pt1, Point pt2){ return new Point(pt1.x + pt2.x, pt1.y + pt2.y); }
    
// }

public class ServerMapManager: NetworkBehaviour
{
    [SerializeField] private ClientMapManager _client;
    [SerializeField] private ServerInteractionManager _interactions;
    [SerializeField] private int _mapXSize, _mapYSize, _mapZHeight;
    [SerializeField] private int cellXSize, cellYSize;
    [SerializeField] private Vector3 _origin;
    [SerializeField] private MapComponentPrefabSO _mapComponentPrefabs;
    [SerializeField] private CombinableSO[] _targetDishesSO;
    [SerializeField] private int _interactableGroupAmount;
    [SerializeField] private int _minSpawnAmount = 0;
    [SerializeField] private int _minUtilityAmount = 0;
    [SerializeField] private int _toolAmount = 0;
    // private int _entranceAmount = 1;
    // private int _exitAmount = 1;

    public NetworkList<FixedString128Bytes> TargetDishesSOStrKeys { get; private set; }
    public NetworkVariable<int> TargetDishesAmount { get; } = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private CellState[,] _mapCellState;
    private ServerMapGenerator _mapGenerator;
    private List<CombinableSO> _requiredCombinableSOList;
    private List<StationeryUtilitySO> _requiredStationeryUtilitySOList;

    void Awake(){
        FixedString128Bytes[] targetDishesSOStrKeys = new FixedString128Bytes[this._targetDishesSO.Length];
        for (int i = 1; i < targetDishesSOStrKeys.Length; i++){
            targetDishesSOStrKeys[i] = this._targetDishesSO[i].StrKey;
            Debug.Log(this._targetDishesSO[i]);}
        this.TargetDishesSOStrKeys = new NetworkList<FixedString128Bytes>(targetDishesSOStrKeys, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        this._mapGenerator = new ServerMapGenerator();
    }

    public void GenerateMapArray(){
        if (!this.IsServer) return;

        this.TargetDishesAmount.Value = this._targetDishesSO.Length;
        foreach (CombinableSO targetDish in this._targetDishesSO)
            TargetDishesSOStrKeys.Add(new FixedString128Bytes(targetDish.StrKey));

        this.GenerateRquiredInteractableList(this._targetDishesSO, out this._requiredCombinableSOList, out this._requiredStationeryUtilitySOList);
        string combinableListMsg = String.Format("{0} Combinables loaded:", this._requiredCombinableSOList.Count);
        string stationeryUtilityListMsg = String.Format("{0} StationeryUtilities loaded:", this._requiredStationeryUtilitySOList.Count);
        foreach (CombinableSO curCombinableSO in this._requiredCombinableSOList) combinableListMsg += " " + curCombinableSO.name + ",";
        foreach (StationeryUtilitySO curStationeryUtilitySO in this._requiredStationeryUtilitySOList) stationeryUtilityListMsg += " " + curStationeryUtilitySO.name + ",";
        print(combinableListMsg);
        print(stationeryUtilityListMsg);

        int spawnAmount = this._requiredCombinableSOList.Count > this._minSpawnAmount ? this._requiredCombinableSOList.Count : this._minSpawnAmount;
        int utilityAmount = this._requiredStationeryUtilitySOList.Count > this._minUtilityAmount ? this._requiredStationeryUtilitySOList.Count : this._minUtilityAmount;

        this._mapCellState = this._mapGenerator.GenerateMapArray(this._mapXSize, this._mapYSize, utilityAmount, spawnAmount, this._toolAmount, this._interactableGroupAmount);
    }

    public void PopulateMap(){
        if (!this.IsServer) return;

        int spawnableIdx = 0;
        int utilityIdx = 0;
        HelperFunc.Shuffle(this._requiredCombinableSOList);

        for (int x = 0; x < _mapXSize; x++){
            for (int y = 0; y < _mapYSize; y++){
                switch (this._mapCellState[x,y]){
                    case CellState.Walkable:
                        break;
                    case CellState.Unwalkable:
                    case CellState.Table:
                        GameObject tableOnly = Instantiate(this._mapComponentPrefabs.TablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);
                        // tablePrefab will have its own info
                        // tableServer.Info = xxx;
                        tableOnly.GetComponent<NetworkObject>().Spawn();
                        break;
                    case CellState.Utility:
                        GameObject stationeryUtility = Instantiate(this._mapComponentPrefabs.UtilityPrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        ServerStationeryUtility stationeryUtilityServer = stationeryUtility.GetComponent<ServerStationeryUtility>();
                        stationeryUtilityServer.Info = this._requiredStationeryUtilitySOList[utilityIdx];
                        utilityIdx = (utilityIdx+1) % this._requiredStationeryUtilitySOList.Count;

                        stationeryUtilityServer.NetworkObjectBuf.Spawn();
                        break;
                    case CellState.IngredientSpawn:
                        GameObject spawnable = Instantiate(this._mapComponentPrefabs.SpawnablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        ServerSpawnable spawnableServer = spawnable.GetComponent<ServerSpawnable>();
                        spawnableServer.SpawnningCombinableInfo = this._requiredCombinableSOList[spawnableIdx];
                        // spawnPrefab will have its own info
                        // spawnableServer.Info = xxx;
                        spawnableIdx = (spawnableIdx+1) % this._requiredCombinableSOList.Count;

                        spawnableServer.NetworkObjectBuf.Spawn();
                        break;
                    case CellState.Tool:
                        GameObject table = Instantiate(this._mapComponentPrefabs.TablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);
                        // tablePrefab will have its own info
                        // tableServer.Info = xxx;
                        table.GetComponent<NetworkObject>().Spawn();

                        ServerTable tableServer = table.GetComponent<ServerTable>();
                        ServerTool toolServer = Instantiate(this._mapComponentPrefabs.ToolPrefab, this.GetWorldCoor(x, y), Quaternion.identity).GetComponent<ServerTool>();
                        toolServer.NetworkObjectBuf.Spawn();
                        _interactions.PlaceToServerInternal(toolServer, tableServer);
                        break;
                    case CellState.Entrance:
                        GameObject entrance = Instantiate(this._mapComponentPrefabs.EntrancePrefab, this.GetWorldCoor(x, y), Quaternion.identity);
                        entrance.GetComponent<NetworkObject>().Spawn();
                        break;
                    case CellState.Exit:
                        GameObject exit = Instantiate(this._mapComponentPrefabs.ExitPrefab, this.GetWorldCoor(x, y), Quaternion.identity);
                        exit.GetComponent<NetworkObject>().Spawn();
                        break;
                    default:
                        Debug.LogWarning(String.Format("invalid CellState found when populating map, x:{0}, y:{1}", x, y));
                        break;
                }
            }
        }
    }

    private void GenerateRquiredInteractableList(CombinableSO[] targetDishesSO, out List<CombinableSO> requiredCombinableSOList, out List<StationeryUtilitySO> requiredStationeryUtilitySOList){
        if (!this.IsServer) { requiredCombinableSOList = null; requiredStationeryUtilitySOList = null; return;}

        CombinableSO.IdentifyRequiredBaseSO(targetDishesSO, out requiredCombinableSOList, out requiredStationeryUtilitySOList);
        CombinableSO.LoadAllRequiredSO(_requiredCombinableSOList, _requiredStationeryUtilitySOList);

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
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), (float) cellXSize);
                        break;
                    case CellState.IngredientSpawn:
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawSphere(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), (float) cellXSize);
                        break;
                    case CellState.Entrance:
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(this.GetWorldCoor(x,y)+(new Vector3(0,10,0)), (float) cellXSize);
                        break;
                    case CellState.Exit:
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
