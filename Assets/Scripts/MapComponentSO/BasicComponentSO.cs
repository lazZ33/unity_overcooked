using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TNRD;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BasicComponentSO", menuName = "ScriptableObject/BasicComponentSO")]
public class BasicComponentSO : ScriptableObject {

    [SerializeField] private TableSO _tableSO;
    public TableSO TableSO => this._tableSO;
    [SerializeField] private SpawnerSO _spawnableSO;
    public SpawnerSO SpawnableSO => this._spawnableSO;
    [SerializeField] private StationeryUtilitySO _dishExitSO;
    public StationeryUtilitySO DishExitSO => this._dishExitSO;
}