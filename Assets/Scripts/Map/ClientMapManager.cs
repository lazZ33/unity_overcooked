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

public class ClientMapManager: NetworkBehaviour
{
    [SerializeField] private ServerMapManager _server;

    private List<CombinableSO> _targetDishesSO;
    private CellState[,] _mapCellState;
    private ServerMapGenerator _mapGenerator;
    private List<CombinableSO> _requiredCombinableSOList;
    private List<UsableHolderSO> _requiredUsableHolderSOList;

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!this.IsClient) return;

        this._server.TargetDishesSOStrKeys.OnListChanged += this.OnTargetDishesChanged;
    }

    private void OnTargetDishesChanged(NetworkListEvent<FixedString128Bytes> changeEvent){
        if (!this.IsClient | this.IsServer) return;
        if (!(changeEvent.Index == this._server.TargetDishesAmount.Value) | !(changeEvent.Type == NetworkListEvent<FixedString128Bytes>.EventType.Add)) return;
     
        foreach (FixedString128Bytes strKey in this._server.TargetDishesSOStrKeys){
            this._targetDishesSO.Add(CombinableSO.GetSO(strKey.ToString()));
        }
        CombinableSO.GetRequiredBaseSO(this._targetDishesSO, out _requiredCombinableSOList, out _requiredUsableHolderSOList);
        CombinableSO.LoadAllRequiredSO(_requiredCombinableSOList, _requiredUsableHolderSOList);
    }

    // public void SetOrigin(int originX, int originY){
    //     if (this._origin != null) { Debug.LogWarning("Duplicate assignment on the origin of grid system"); return;}
    //     this._origin = new Vector3(originX, originY, 0);
    // }
    // public Vector2 GetGridCoor(float x, float y){
    //     return new Vector2((int)Math.Floor((x - (this._origin.x)/this.cellXSize)), (int)Math.Floor((y - (this._origin.y)/this.cellYSize)));
    // }
    // public Vector3 GetWorldCoor(int x, int y){
    //     return new Vector3(this._origin.x + this.cellXSize * x, this._mapZHeight, this._origin.y + this.cellYSize * y);
    // }
}
