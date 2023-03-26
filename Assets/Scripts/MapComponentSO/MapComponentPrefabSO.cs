using UnityEngine;
using Unity;

[CreateAssetMenu(fileName = "MapComponentPrefabSO", menuName = "ScriptableObject/MapComponentPrefab")]
public class MapComponentPrefabSO: ScriptableObject {
    [SerializeField] private GameObject _ingredientPrefab;
    public GameObject IngredientPrefab => this._ingredientPrefab;
    [SerializeField] private GameObject _tablePrefab;
    public GameObject TablePrefab => this._tablePrefab;
    [SerializeField] private GameObject _utilityPrefab;
    public GameObject UtilityPrefab => this._utilityPrefab;
    [SerializeField] private GameObject _spawnablePrefab;
    public GameObject SpawnablePrefab => this._spawnablePrefab;
    [SerializeField] private GameObject _toolPrefab;
    public GameObject ToolPrefab => this._toolPrefab;
    [SerializeField] private GameObject _entrancePrefab;
    public GameObject EntrancePrefab => this._entrancePrefab;
    [SerializeField] private GameObject _exitPrefab;
    public GameObject ExitPrefab => this._exitPrefab;

}