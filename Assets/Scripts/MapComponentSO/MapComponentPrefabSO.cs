using UnityEngine;
using Unity;

[CreateAssetMenu(fileName = "MapComponentPrefabSO", menuName = "ScriptableObject/MapComponentPrefab")]
public class MapComponentPrefabSO: ScriptableObject {
	[SerializeField] private GameObject _CombinablePrefab;
	public GameObject CombinablePrefab => this._CombinablePrefab;
	[SerializeField] private GameObject _tablePrefab;
	public GameObject TablePrefab => this._tablePrefab;
	[SerializeField] private GameObject _utilityPrefab;
	public GameObject UtilityPrefab => this._utilityPrefab;
	[SerializeField] private GameObject _spawnablePrefab;
	public GameObject SpawnablePrefab => this._spawnablePrefab;
	//[SerializeField] private GameObject _toolPrefab;
	//public GameObject ToolPrefab => this._toolPrefab;
	[SerializeField] private GameObject _dishExitPrefab;
	public GameObject DishExitPrefab => this._dishExitPrefab;
	[SerializeField] private GameObject _gameFloorPrefab;
	public GameObject GameFloorPrefab => this._gameFloorPrefab;
}