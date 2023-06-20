using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BasicComponentSO", menuName = "ScriptableObject/BasicComponentSO")]
public class BasicComponentSO : ScriptableObject {

    [SerializeField] private TableSO _tableSO;
    public TableSO TableSO => this._tableSO;
    [SerializeField] private SpawnableSO _spawnableSO;
    public SpawnableSO SpawnableSO => this._spawnableSO;
    [SerializeField] private DishExitSO _dishExitSO;
    public DishExitSO DishExitSO => this._dishExitSO;
}