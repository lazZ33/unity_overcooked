using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;

// public class Point{
//     public int x, y;
//     public Point(int x, int y){
//         this.x = x;
//         this.y = y;
//     }
//     public static Point operator-(Point pt1, Point pt2){ return new Point(pt1.x - pt2.x, pt1.y - pt2.y); }
//     public static Point operator+(Point pt1, Point pt2){ return new Point(pt1.x + pt2.x, pt1.y + pt2.y); }
    
// }

public class ClientMapManager: NetworkBehaviour
{
    [SerializeField] private int _mapXSize, _mapYSize, _mapZHeight;
    [SerializeField] private int cellXSize, cellYSize;
    [SerializeField] private Vector3 _origin;
    [SerializeField] private MapComponentPrefabSO _mapComponentPrefabs;
    [SerializeField] private GrabbableSO[] _targetDishesSO;
    [SerializeField] private int _interactableGroupAmount;
    [SerializeField] private int _minSpawnAmount = 0;
    [SerializeField] private int _minUtilityAmount = 0;
    [SerializeField] private int _toolAmount = 0;
    // private int _entranceAmount = 1;
    // private int _exitAmount = 1;
    private CellState[,] _mapCellState;
    private ServerMapGenerator _mapGenerator;
    private List<GrabbableSO> _requiredGrabbableSOList;
    private List<UtilitySO> _requiredUtilitySOList;

    void Awake(){
        this._mapGenerator = new ServerMapGenerator();
        // this._mapArray = this._mapGenerator.GenerateMapArray(this._mapXSize, this._mapYSize, this._utilityAmount, this._toolAmount, this._interactableGroupAmount);
    }

    public void GenerateMapArray(){
        this.GenerateRquiredInteractableList(out this._requiredGrabbableSOList, out this._requiredUtilitySOList);

        int spawnAmount = this._requiredGrabbableSOList.Count > this._minSpawnAmount ? this._requiredGrabbableSOList.Count : this._minSpawnAmount;
        int utilityAmount = this._requiredUtilitySOList.Count > this._minUtilityAmount ? this._requiredUtilitySOList.Count : this._minUtilityAmount;

        this._mapCellState = this._mapGenerator.GenerateMapArray(this._mapXSize, this._mapYSize, spawnAmount, utilityAmount, this._toolAmount, this._interactableGroupAmount);
    }

    private void PopulateMap(){
        // if (this._mapArray == null) this._mapArray = this._mapGenerator.GenerateMapArray(this._mapXSize, this._mapYSize, 5, 1);

        // List<IngredientSO> Ingredients = new List<IngredientSO>(this._recipe);
        int spawnableIdx = 0;
        HelperFunc.Shuffle(_requiredGrabbableSOList);

        for (int x = 0; x < _mapXSize; x++){
            for (int y = 0; y < _mapYSize; y++){
                switch (this._mapCellState[x,y]){
                    case CellState.Walkable:
                        break;
                    case CellState.Table:
                        this.CreateComponent(x, y, this._mapComponentPrefabs.TablePrefab);
                        break;
                    case CellState.Utility:
                        this.CreateComponent(x, y, this._mapComponentPrefabs.UtilityPrefab);
                        break;
                    case CellState.IngredientSpawn:
                        GameObject spawnable = this.CreateComponent(x, y, this._mapComponentPrefabs.SpawnablePrefab);

                        ServerSpawnable spawnableServer = spawnable.GetComponent<ServerSpawnable>();
                        spawnableServer.InfoStrKey.Value = this._requiredGrabbableSOList[spawnableIdx].name;
                        spawnableIdx = (spawnableIdx+1) % this._requiredGrabbableSOList.Count;

                        break;
                    case CellState.Tool:
                        GameObject table = this.CreateComponent(x, y, this._mapComponentPrefabs.TablePrefab);

                        ServerTable tableServer = table.GetComponent<ServerTable>();
                        ServerTool toolServer = this.CreateComponent(x, y, this._mapComponentPrefabs.ToolPrefab).GetComponent<ServerTool>();
                        tableServer.PlaceServerRpc(toolServer.NetworkObject);
                        break;
                    case CellState.Entrance:
                        this.CreateComponent(x, y, this._mapComponentPrefabs.EntrancePrefab);
                        break;
                    case CellState.Exit:
                        this.CreateComponent(x, y, this._mapComponentPrefabs.ExitPrefab);
                        break;
                    default:
                        Debug.LogWarning("invalid CellState found when populating map");
                        break;
                }
            }
        }
    }

    private void GenerateRquiredInteractableList(out List<GrabbableSO> requiredGrabbableSOList, out List<UtilitySO> requiredUtilitySOList){
        HashSet<GrabbableSO> requiredGrabbableSOSet = new HashSet<GrabbableSO>(10);
        HashSet<UtilitySO> requiredUtilitySOSet = new HashSet<UtilitySO>(10);

        // assume this._targetDishesSO is not null
        foreach (GrabbableSO targetDishSO in this._targetDishesSO){
            foreach (String requiredGrabbableName in targetDishSO.ContainedGrabbableNames){
                GrabbableSO requiredGrabbableSO = (GrabbableSO) Resources.Load("InteractableSO/Grabbable/" + requiredGrabbableName);
                if (requiredGrabbableSO == null){
                    Debug.LogError("Failed to load required ingredient SO, required ingredient SO: " + requiredGrabbableName);
                    continue;
                }
                requiredGrabbableSOSet.Add(requiredGrabbableSO);
            }
            foreach (String requiredUtilityName in targetDishSO.RequiredUtilityNames){
                Debug.Log(requiredUtilityName);
                UtilitySO requiredUtilitySO = (UtilitySO) Resources.Load("InteractableSO/Grabbable/" + requiredUtilityName);
                if (requiredUtilitySO == null){
                    Debug.LogError("Failed to load required ingredient SO, required ingredient SO: " + requiredUtilityName);
                    continue;
                }
                requiredUtilitySOSet.Add(requiredUtilitySO);
            }
        }

        requiredGrabbableSOList = new List<GrabbableSO>(requiredGrabbableSOSet);
        requiredUtilitySOList = new List<UtilitySO>(requiredUtilitySOSet);
        return;
    }

    void OnDrawGizmos(){
        if (this._mapCellState == null) return;

        for (int x = 0; x < this._mapXSize; x++){
            for (int y = 0; y < this._mapYSize; y++){
                switch (this._mapCellState[x,y]){
                    case CellState.Walkable:
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(this._origin + new Vector3(x, 4, y), new Vector3(cellXSize, cellYSize, 1));
                        break;
                    case CellState.Unwalkable:
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(this._origin + new Vector3(x, 4, y), new Vector3(cellXSize, cellYSize, 1));
                        break;
                    case CellState.Utility:
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(this._origin + new Vector3(x, 4, y), (float) cellXSize);
                        break;
                    case CellState.IngredientSpawn:
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawSphere(this._origin + new Vector3(x, 4, y), (float) cellXSize);
                        break;
                    case CellState.Entrance:
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(this._origin + new Vector3(x, 4, y), (float) cellXSize);
                        break;
                    case CellState.Exit:
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(this._origin + new Vector3(x, 4, y), (float) cellXSize);
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
        return new Vector3(this._origin.x + this._mapXSize * x, this._origin.y + this._mapYSize * y, this._mapZHeight);
    }
    private GameObject CreateComponent(int x, int y, GameObject prefab){
        GameObject newComponent = Instantiate(prefab, this.GetWorldCoor(x, y), Quaternion.identity);
        newComponent.GetComponent<NetworkObject>().Spawn();
        return newComponent;
    }
    public void insertObject(InteractableSO obj, float x, float y){
        // Point coor = this.getGridCoor(x, y);

        // if (this._mapArray[coor.x, coor.y] == null) { this._mapArray[coor.x, coor.y] = obj; return; }

        // if (this._mapArray[coor.x, coor.y].GetType() == UsableSO){
            
        // }
    }

    public void get(){

    }
}
