using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ServerMapManager: NetworkBehaviour
{
    private ServerInteractionManager _interactions = ServerInteractionManager.Instance;

    [SerializeField] private ClientMapManager _client;
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
    private ICombinableSO[] _targetDishesSO;
    private ServerMapGenerator _mapGenerator;
    private List<ICombinableSO> _requiredCombinableSOList;
    private List<IUsableSO> _requiredConverterSOList;


    private void OnDishOutCallback(object sender, ServerUseEventArgs args) => this.OnDishOut?.Invoke(sender, new DishOutEventArgs((ICombinableSO)args.target));


    void Awake(){
        this.TargetDishesSOStrKeys = new NetworkList<FixedString128Bytes>(new FixedString128Bytes[10], NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        this._mapGenerator = new ServerMapGenerator();
    }


    public void GenerateMap(ICombinableSO[] targetDishesSO){
        if (!this.IsServer) return;

        this._targetDishesSO = targetDishesSO;

        this.TargetDishesAmount.Value = this._targetDishesSO.Length;
		//for (int i = 1; i < this._targetDishesSO.Length; i++){
		//	this.TargetDishesSOStrKeys[i] = this._targetDishesSO[i].StrKey;
		//	Debug.Log(this._targetDishesSO[i]);
		//}
        foreach (ICombinableSO targetDish in this._targetDishesSO)
            TargetDishesSOStrKeys.Add(new FixedString128Bytes(targetDish.StrKey));


		this.IdentifyRquiredInteractableList(this._targetDishesSO, out this._requiredCombinableSOList, out this._requiredConverterSOList);

        int spawnAmount = this._requiredCombinableSOList.Count > this._minSpawnAmount ? this._requiredCombinableSOList.Count : this._minSpawnAmount;
        int utilityAmount = this._requiredConverterSOList.Count > this._minUtilityAmount ? this._requiredConverterSOList.Count : this._minUtilityAmount;

        this._mapCellState = this._mapGenerator.GenerateMapArray(this._mapXSize, this._mapYSize, utilityAmount, spawnAmount, this._toolAmount, this._interactableGroupAmount);
    }
	public void DespawnMap(){
		this.OnMapDespawn?.Invoke(this, EventArgs.Empty);
	}
	public void SpawnMap(){
        if (!this.IsServer) return;

        int spawnableIdx = 0;
        int utilityIdx = 0;
        //IInteractable curInteractable;

        HelperFunc.Shuffle(this._requiredCombinableSOList);

        for (int x = 0; x < _mapXSize; x++)
            for (int y = 0; y < _mapYSize; y++)
                switch (this._mapCellState[x,y]){
                    case CellState.Walkable:
                        break;
                    case CellState.Unwalkable:
                    case CellState.Table:
                        GameObject tableOnly = Instantiate(this._mapComponentPrefabs.TablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        IServerInteractable tableServer = tableOnly.GetComponent<IServerInteractable>();
                        tableServer.InfoInit(this._basicComponentSO.TableSO);
                        tableServer.NetworkObjectBuf.Spawn();
                        this.OnMapDespawn += tableServer.OnMapDespawn;
						break;
                    case CellState.Utility:
                        GameObject stationeryUtility = Instantiate(this._mapComponentPrefabs.UtilityPrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        IServerInteractable stationeryUtilityServer = stationeryUtility.GetComponent<IServerInteractable>();
                        stationeryUtilityServer.InfoInit(this._requiredConverterSOList[utilityIdx]);
                        utilityIdx = (utilityIdx+1) % this._requiredConverterSOList.Count;

                        stationeryUtilityServer.NetworkObjectBuf.Spawn();
						this.OnMapDespawn += stationeryUtilityServer.OnMapDespawn;
						break;
                    case CellState.IngredientSpawn:
                        GameObject spawnable = Instantiate(this._mapComponentPrefabs.SpawnablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        IServerSpawner spawnableServer = spawnable.GetComponent<IServerSpawner>();
                        spawnableServer.SpawnningGrabbableInfoInit(this._requiredCombinableSOList[spawnableIdx]);
                        spawnableServer.InfoInit(this._basicComponentSO.SpawnableSO);
                        spawnableIdx = (spawnableIdx+1) % this._requiredCombinableSOList.Count;

                        spawnableServer.NetworkObjectBuf.Spawn();
						this.OnMapDespawn += spawnableServer.OnMapDespawn;
						break;
                    case CellState.Tool:
                        GameObject table = Instantiate(this._mapComponentPrefabs.TablePrefab, this.GetWorldCoor(x, y), Quaternion.identity);
                        tableServer = table.GetComponent<IServerHolder>();
                        tableServer.InfoInit(this._basicComponentSO.TableSO);
                        table.GetComponent<NetworkObject>().Spawn();
						this.OnMapDespawn += tableServer.OnMapDespawn;

                        IServerGrabbable toolServer = Instantiate(this._mapComponentPrefabs.ToolPrefab, this.GetWorldCoor(x, y), Quaternion.identity).GetComponent<IServerGrabbable>();
                        // TODO: toolServer.InfoInit(xxx);
                        toolServer.NetworkObjectBuf.Spawn();
                        this._interactions.GrabServerInternal((IServerHolder)tableServer, toolServer);

						this.OnMapDespawn += toolServer.OnMapDespawn;
						break;
                    case CellState.DishExit:
                        GameObject dishExit = Instantiate(this._mapComponentPrefabs.DishExitPrefab, this.GetWorldCoor(x, y), Quaternion.identity);

                        IServerUsable dishExitServer = dishExit.GetComponent<IServerUsable>();
                        dishExitServer.InfoInit(this._basicComponentSO.DishExitSO);
                        dishExitServer.NetworkObjectBuf.Spawn();

						this.OnMapDespawn += dishExitServer.OnMapDespawn;
                        dishExitServer.OnUnuse += this.OnDishOutCallback;
						break;
                    default:
                        Debug.LogWarning(String.Format("invalid CellState found when populating map, x:{0}, y:{1}", x, y));
                        break;
                }
    }


    private void IdentifyRquiredInteractableList(ICombinableSO[] targetDishesSO, out List<ICombinableSO> requiredCombinableSOList, out List<IUsableSO> requiredConverterSOList){
        if (!this.IsServer) { requiredCombinableSOList = null; requiredConverterSOList = null; return;}

        ICombinableSO.GetRequiredBaseSO(targetDishesSO, out requiredCombinableSOList, out requiredConverterSOList);
        ICombinableSO.LoadAllRequiredSO(requiredCombinableSOList, requiredConverterSOList);

        string combinableListMsg = String.Format("{0} Combinables loaded:", requiredCombinableSOList.Count);
        string stationeryUtilityListMsg = String.Format("{0} StationeryUtilities loaded:", requiredConverterSOList.Count);
        foreach (ICombinableSO curCombinableSO in requiredCombinableSOList) combinableListMsg += " " + curCombinableSO.ToString() + ",";
        foreach (IUsableSO curUsableHolderSO in requiredConverterSOList) stationeryUtilityListMsg += " " + curUsableHolderSO.ToString() + ",";
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


    public Vector3 GetWorldCoor(int x, int y){
        return new Vector3(this._origin.x + this.cellXSize * x, this._mapZHeight, this._origin.y + this.cellYSize * y);
    }
}
