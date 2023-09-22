using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TNRD;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BasicComponentSO", menuName = "ScriptableObject/BasicComponentSO")]
public class BasicComponentSO: ScriptableObject {

	[SerializeField] private TableSO _tableSO;
	public TableSO TableSO => this._tableSO;
	[SerializeField] private SpawnerSO _spawnableSO;
	public SpawnerSO SpawnerSO => this._spawnableSO;
	[SerializeField] private StationaryUtilitySO _dishExitSO;
	public StationaryUtilitySO DishExitSO => this._dishExitSO;
	[SerializeField] private GameFloorSO _gameFloorSO;
	public GameFloorSO GameFloorSO => this._gameFloorSO;
}