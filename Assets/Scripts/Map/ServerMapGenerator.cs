using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ServerMapGenerator {

    private Cell[,] _map;
    private int _mapX;
    private int _mapY;

    public CellState[,] GenerateMapArray(out Vector2Int[] playerSpawnPoints, int playerAmount, int XSize, int YSize, int utilityAmount, int spawnAmount, int toolAmount, int interactableGroupAmount){
        Debug.Log("Start generating map");

        // init map
        this._mapX = XSize;
        this._mapY = YSize;
        this._map = new Cell[this._mapX,this._mapY];
        for (int x = 0; x < this._mapX; x++){
            for (int y = 0; y < this._mapY; y++){
                Cell curCell = new Cell(x,y);
                this._map[x,y] = curCell;
            }
        }

        // required parameters
        int minUtilGroupDist = (int) Math.Floor((double) Math.Sqrt(3*XSize*YSize / Math.Pow(interactableGroupAmount,2)));
        int minTotalUtilGroupDist = (int) Math.Floor((double) Math.Sqrt(1.5*XSize*YSize / Math.Pow(interactableGroupAmount,2)) * Math.Pow(2, interactableGroupAmount));
        Debug.Log("minUtilGroupDist: " + minUtilGroupDist);
        Debug.Log("totalUtilGroupDist: " + minTotalUtilGroupDist);
        int obstaclesChance = 9; // 1 - 10, 10 = always add obstacles between pivots
        int requestedObstacleMaxSize = UnityEngine.Random.Range(4, (this._mapX+this._mapY)/10);
        CellState defaultCellState = UnityEngine.Random.Range(0, 2) == 0 ? CellState.Walkable : CellState.Unwalkable;
        bool isFilledMap = (defaultCellState == CellState.Unwalkable); // 0 = walkable, 1 = unwalkable
        int obstacleType = UnityEngine.Random.Range(0, 2); // 0 = square, 1 = random

        // decide if map is initially filled with Walkable or Unwalkable
        for (int x = 0; x < this._mapX; x++){
            for (int y = 0; y < this._mapY; y++){
                Cell curCell = this.GetCell(x, y);
                // if (curCell == null) continue;
                if (this.IsBorderCell(curCell)) curCell.cellState = CellState.Unwalkable;
                else curCell.cellState = defaultCellState;
            }
        }

        // // decide and write utility cells into map
        List<Cell> pivotsCells = this.GetRandomCells(interactableGroupAmount, minTotalUtilGroupDist, minUtilGroupDist, cell => !this.IsCornerCell(cell));

        if (isFilledMap){
            // connect all utilities together with path finding algo
            foreach (Cell startCell in pivotsCells){
                foreach (Cell endCell in pivotsCells){

                    List<Cell> path = this.GetPath(startCell, endCell, false);
                    foreach (Cell cell in path){
                        if (cell.cellState == CellState.Unwalkable && !this.IsBorderCell(cell)) cell.cellState = CellState.Walkable;

                        foreach (Cell neighbour in this.Get8Neighbour(cell)){
                            if (neighbour.cellState == CellState.Unwalkable && !this.IsBorderCell(neighbour)) neighbour.cellState = CellState.Walkable;
                        }
                    }

                }
            }
        }
        else {
            // add obstacles in paths between utilities
            foreach (Cell startCell in pivotsCells){
                foreach (Cell endCell in pivotsCells){
                    if (UnityEngine.Random.Range(0, 10) > obstaclesChance) continue;

                    List<Cell> path = this.GetPath(startCell, endCell, true);
                    
                    if (path.Count == 0) continue;

                    int pathIdx = UnityEngine.Random.Range(0, path.Count);
                    int seedPosDistToUtility = pathIdx > path.Count/2 ? path.Count - pathIdx : pathIdx;
                    Cell obstacleSeedCell = path[pathIdx];
                    int actualObstacleMaxSize = requestedObstacleMaxSize > seedPosDistToUtility ? seedPosDistToUtility : requestedObstacleMaxSize;
                    
                    switch(obstacleType){
                        case 0:
                            // generate square obstacles
                            this.GenerateObstacle(obstacleSeedCell, actualObstacleMaxSize, 10);
                            break;
                        case 1:
                            // generate random obstacles
                            this.GenerateObstacle(obstacleSeedCell, actualObstacleMaxSize, 7);
                            break;
                        // case 2:
                        //     break;
                    }

                }
            }
        }

        // Choose positions for putting utilites/spawns/exit/entrance
        int totalInterestedPointAmount = utilityAmount+spawnAmount+2;
        List<Cell> interestedCells = this.GetRandomCellsAround(totalInterestedPointAmount, pivotsCells, (this._mapX+this._mapY)/3 / interactableGroupAmount,
        cell => {
            if (cell.cellState != CellState.Unwalkable) return false;
            foreach(Cell neighbour in this.Get4Neighbour(cell)){
                if (neighbour.cellState == CellState.Walkable) return true;
            }
            return false;
        });
        HelperFunc.Shuffle(interestedCells);
        for (int i = 0; i < interestedCells.Count; i++){
            if (i >= utilityAmount+spawnAmount) interestedCells[i].cellState = CellState.DishExit;
            else if (i >= utilityAmount) interestedCells[i].cellState = CellState.IngredientSpawn;
            else interestedCells[i].cellState = CellState.Utility;
        }

        // output result
        CellState[,] outputMapArray = new CellState[this._mapX,this._mapY];
        for (int x = 0; x < this._mapX; x++){
            for (int y = 0; y < this._mapY; y++){
                outputMapArray[x,y] = this._map[x,y].cellState;
            }
        }

        // Generate player spawn points
        List<Cell> validPlayerSpawnPoints = this.GetRandomCells(playerAmount, (cell) =>
            {
                if (cell.cellState != CellState.Walkable) return false;
                foreach (Cell neighbour in this.Get8Neighbour(cell))
                {
                    if (neighbour.cellState != CellState.Walkable) return false;
                }
                return true;
            }
        );
        playerSpawnPoints = validPlayerSpawnPoints.Select((cell) => { 
            this._map[cell.x, cell.y].cellState = CellState.PlayerSpawn;
            return new Vector2Int(cell.x,cell.y);
        }).ToArray();

        return outputMapArray;
    }

    private bool IsBorderCell(Cell target){
        return (target.x == 0 | target.x == this._mapX-1 | target.y == 0 | target.y == this._mapY-1);
    }
    private bool IsCornerCell(Cell target){
        return (target.x == 0 | target.x == this._mapX-1) && (target.y == 0 | target.y == this._mapY-1);
    }
    private bool IsMapFullyAccessible(){
        Cell startPoint = null;
        int expectedAccessibleCellCount = 0;
        for (int x = 0; x < this._mapX; x++){
            for (int y = 0; y < this._mapY; y++){
                Cell curCell = this.GetCell(x, y);
                if (curCell.cellState == CellState.Unwalkable) continue;
                expectedAccessibleCellCount++;
                if (startPoint == null) startPoint = curCell; // pick arbitary walkable cell to start flood fill
            }
        }

        int approximateOuterEdgeSize = this._mapX + this._mapY;
        HashSet<Cell> curEdgeCells = new HashSet<Cell>(approximateOuterEdgeSize);
        HashSet<Cell> newEdgeCells = new HashSet<Cell>(approximateOuterEdgeSize);
        HashSet<Cell> walkedCells = new HashSet<Cell>(expectedAccessibleCellCount);
        curEdgeCells.Add(startPoint);

        do {
            foreach (Cell curEdgeCell in curEdgeCells){
                foreach (Cell curNeighbour in this.Get4Neighbour(curEdgeCell)){
                    if (curNeighbour.cellState != CellState.Unwalkable && !walkedCells.Contains(curNeighbour)){
                        newEdgeCells.Add(curNeighbour);
                    }
                }
            }

            if (newEdgeCells.Count == 0) return false;

            curEdgeCells.Clear();
            foreach (Cell cell in newEdgeCells){
                curEdgeCells.Add(cell);
                walkedCells.Add(cell);
            }
            newEdgeCells.Clear();
        } while(expectedAccessibleCellCount > walkedCells.Count);

        return true;
    }

    private Cell? GetCell(int x, int y){
        if (0 > x | x > this._mapX-1) {
            Debug.LogWarning("GetCell requested x dimension exceeded boundary, x: " + x + " y: " + y);
            return null;
        }
        if (0 > y | y > this._mapY-1) {
            Debug.LogWarning("GetCell requested y dimension exceeded boundary, x: " + x + " y: " + y);
            return null;
        }
        return this._map[x, y];
    }
    private List<Cell> Get4Neighbour(Cell target){
        List<Cell> neighbours = new List<Cell>(4);
        for (int dx = -1; dx < 2; dx+=2){
            int neighbourX = target.x + dx;
            if (0 > neighbourX | neighbourX > this._mapX-1) continue;
            if (0 > target.y | target.y > this._mapY-1) continue;
            Cell neighbourCell = this.GetCell(neighbourX, target.y);
            if (neighbourCell == null) Debug.LogWarning("Get4Neighbour null occured");
            neighbours.Add(neighbourCell);
        }
        for (int dy = -1; dy < 2; dy+=2){
            int neighbourY = target.y + dy;
            if (0 > target.x | target.x > this._mapX-1) continue;
            if (0 > neighbourY | neighbourY > this._mapY-1) continue;
            Cell neighbourCell = this.GetCell(target.x, neighbourY);
            if (neighbourCell == null) Debug.LogWarning("Get4Neighbour null occured");
            neighbours.Add(neighbourCell);
        }
        neighbours.TrimExcess();
        return neighbours;
    }
    private List<Cell> Get8Neighbour(Cell target){
        List<Cell> neighbours = new List<Cell>(8);
        for (int dx = -1; dx < 2; dx++){
            for (int dy = -1; dy < 2; dy++){
                if (dx == 0 && dy == 0) continue;
                int neighbourX = target.x + dx;
                int neighbourY = target.y + dy;
                if (0 > neighbourX | neighbourX > this._mapX-1) continue;
                if (0 > neighbourY | neighbourY > this._mapY-1) continue;
                Cell neighbourCell = this.GetCell(neighbourX, neighbourY);
                if (neighbourCell == null) Debug.LogWarning("Get4Neighbour null occured");
                neighbours.Add(neighbourCell);
            }
        }
        neighbours.TrimExcess();
        return neighbours;
    }
    private void FillCells(IEnumerable<Cell> cells, CellState state){
        foreach (Cell cell in cells){
            cell.cellState = state;
        }
    }
    // make sure not to request too many amount that exceed max. possible cell
    // private List<Cell> GetRandomNeighbourAroundCells<T>(T cells, int amount) where T:IEnumerable<Cell>, IList<Cell>{
    //     if (amount > cells.Count*3) Debug.LogWarning("Requested neighbour amount exceeded safe level. Suggest lowering the amount requested.");
    //     HashSet<Cell> outputCellsSet = new HashSet<Cell>(amount);
    //     int safetyCount = 0;
    //     for (int i = 0; i < amount; i++){
    //         Cell newPos;
    //         List<Cell> neighbours = this.Get8Neighbour(cells[UnityEngine.Random.Range(0, cells.Count)]);
    //         // prevent duplicate with original cell
    //         neighbours.RemoveAll(cell => { return cells.Contains(cell);});
    //         if (neighbours.Count == 0){
    //             i--;
    //             if (safetyCount++ > 30) { Debug.LogError("Requested neighbour amount too large and caused infinite looping, Please reduces the amount requested."); break;}
    //             continue;
    //         };
    //         newPos = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
    //         outputCellsSet.Add(newPos);
    //     }
    //     List<Cell> outputCells = new List<Cell>(outputCellsSet);
    //     return outputCells;
    // }
    private List<Cell> GetRandomCellsAround(int amount, IEnumerable<Cell> pivotCells, int maxDist, Predicate<Cell> requirement){
        HashSet<Cell> evaluatedCells = new HashSet<Cell>(this._mapX + this._mapY);
        List<Cell> qualifiedCells = new List<Cell>(amount);
        List<Cell> curEdgeCells = new List<Cell>(this._mapX);
        HashSet<Cell> newEdgeCells = new HashSet<Cell>(this._mapX);
        int curDist = 0;
        curEdgeCells.AddRange(pivotCells);

        while (curDist++ < maxDist){
            foreach (Cell edgeCell in curEdgeCells){

                if (requirement!.Invoke(edgeCell)){
                    qualifiedCells.Add(edgeCell);
                }

                evaluatedCells.Add(edgeCell);

                foreach (Cell neighbour in this.Get4Neighbour(edgeCell)){
                    if (!evaluatedCells.Contains(neighbour)){
                        newEdgeCells.Add(neighbour);
                    }
                }

            }

            curEdgeCells.Clear();
            curEdgeCells.AddRange(newEdgeCells);
            newEdgeCells.Clear();
        }

        if (qualifiedCells.Count == amount){
            return qualifiedCells;
        }
        else if (qualifiedCells.Count > amount){
            int extraCount = qualifiedCells.Count - amount;
            for (int i = 0; i < extraCount; i++){
                qualifiedCells.Remove(qualifiedCells[UnityEngine.Random.Range(0,qualifiedCells.Count)]);
            }
            return qualifiedCells;
        }
        Debug.Log("Failed to find qualified cells around given cells in given distance");
        return qualifiedCells;
    }

    private List<Cell> GetRandomCells(int amount, int requestedMinTotalDist, int requestedMinDist, Predicate<Cell> requirement){
        List<Cell> cells = new List<Cell>(amount);
        for (int i = 0; i < amount; i++){
            cells.Add(this.GetCell(UnityEngine.Random.Range(0, this._mapX), UnityEngine.Random.Range(0, this._mapY)));
        }

        int idx = 0;
        int totalDist = 0;
        bool notFufillingMinDist = true;
        int safetyCount = 0;

        while (totalDist < requestedMinTotalDist | notFufillingMinDist | !cells.TrueForAll(requirement)){
            safetyCount++;
            if (safetyCount > 1000) { Debug.LogError("GetRandomCells failed to generate cells that fufils all provided requirements."); break; }
            // regenerate part of the existing location
            idx = (++idx)%amount;
            int randomX = UnityEngine.Random.Range(0, this._mapX);
            int randomY = UnityEngine.Random.Range(0, this._mapY);
            Cell curCell = this.GetCell(randomX, randomY);
            if (curCell == null) Debug.Log("GetRandomCell generated null cell");
            cells[idx] = curCell;

            // checking if it meets total distance and min distance
            totalDist = 0;
            notFufillingMinDist = false;
            for (int i1 = 0; i1 < cells.Count; i1++){
                for (int i2 = i1+1; i2 < cells.Count; i2++){
                    if (Cell.AbsCellDistance(cells[i1], cells[i2]) < requestedMinDist) { notFufillingMinDist = true; break; }
                    totalDist += Cell.AbsCellDistance(cells[i1], cells[i2]);
                }
            }
        }
        return cells;
    }
    private List<Cell> GetRandomCells(int amount, Predicate<Cell> requirement){
        Cell curCell;
        List<Cell> validCells = new List<Cell>(this._map.GetLength(0)*this._map.GetLength(1));
        for (int x = 0; x < this._mapX; x++){
            for (int y = 0; y < this._mapY; y++){
                curCell = this.GetCell(x, y);
                if (!requirement.Invoke(curCell)) continue;
                validCells.Add(curCell);
            }
        }
        if (validCells.Count < amount) { Debug.LogError("requested requirement too strict, please adjust requirement or amount requested"); return null; }
        for (int i = 0; i < validCells.Count - amount; i++){
            validCells.Remove(validCells[UnityEngine.Random.Range(0, validCells.Count-1)]);
        }
        validCells.TrimExcess();
        return validCells;
    }

    // return path in sequence from start to end, excluding start and end cell
    private List<Cell> GetPath(Cell startCell, Cell endCell, bool requireWalkable){
        // Debug.Log(String.Format("start cell: ({0},{1}), end cell: ({2},{3})", startCell.x, startCell.y, endCell.x, endCell.y));
        // reset cost
        for (int x = 0; x < this._mapX; x++){
            for (int y = 0; y < this._mapY; y++){
                this.GetCell(x,y).GCost = 0;
                this.GetCell(x,y).HCost = 0;
            }
        }
        if (startCell.PosEquals(endCell)) return new List<Cell>(0);

        // init lists/sets
        List<Cell> walkableCells = new List<Cell>(this._mapX);
        HashSet<Cell>  walkedCells = new HashSet<Cell> (this._mapX);
        AStarCellCostComparer comparer = new AStarCellCostComparer();
        Cell curCell;
        walkableCells.Add(startCell);

        while(true){
            // check if path exist
            if (requireWalkable && walkableCells.Count == 0) return new List<Cell>(0);

            // get the lowest cost cell among all walkable cells
            walkableCells.Sort(comparer);
            if (walkableCells.Count == 0) {
                foreach (Cell cell in walkedCells){
                    Debug.Log(String.Format("{0}, {1}", cell.x, cell.y));
                }
                Debug.Log(String.Format("no path available, start cell: ({0},{1}), end cell: ({2},{3})", startCell.x, startCell.y, endCell.x, endCell.y));
                Debug.Log(walkedCells.Count);
                return new List<Cell>(0);
            }
            curCell = walkableCells[0];
            // Debug.Log(curCell.x + " " + curCell.y);
            walkedCells.Add(curCell);
            walkableCells.Remove(curCell);

            // end if the lowest cost cell == end cell
            if (curCell.PosEquals(endCell)) break;

            // add suitable neighbour into walkable list
            foreach (Cell neighbour in this.Get4Neighbour(curCell)){
                if (walkedCells.Contains(neighbour) | (this.IsBorderCell(neighbour) && !neighbour.PosEquals(endCell))) continue;
                if (requireWalkable && neighbour.cellState != CellState.Walkable && !neighbour.PosEquals(endCell)) continue;

                if (!walkableCells.Contains(neighbour) | curCell.GCost + 1 < neighbour.GCost){
                    neighbour.GCost = curCell.GCost + 1;
                    neighbour.PrevPos = curCell;
                    neighbour.HCost = Cell.AbsCellDistance(neighbour, endCell);

                    if (!walkableCells.Contains(neighbour)) walkableCells.Add(neighbour);
                }
            }
        }

        // excluding end cell
        curCell = curCell.PrevPos;
        List<Cell> outputList = new List<Cell>(this._mapX);
        // excluding start cell
        while (!curCell.PosEquals(startCell)){
            outputList.Add(curCell);
            curCell = curCell.PrevPos;
            // if (this.IsBorderCell(curCell)) Debug.Log("border cell included into the path");
        }
        outputList.Reverse();
        outputList.TrimExcess();
        return outputList;
    }

    private void GenerateObstacle(Cell obstacleSeedCell, int obstacleMaxSize, int fillExtent){
        if (!(1<=fillExtent && fillExtent<=10)) { 
            Debug.LogError(String.Format("Invalid fillExtent (not in range) for GenerateObstacle (0 <= fillExtent <= 10): provided fillExtent: {0}", fillExtent));
            return;
        }

        List<Cell> prevObstacleEdgeCells = new List<Cell>(4 * obstacleMaxSize);
        HashSet<Cell> curObstacleEdgeCells = new HashSet<Cell>(4 * obstacleMaxSize);
        curObstacleEdgeCells.Add(obstacleSeedCell);
        for (int i = 0; i < obstacleMaxSize; i++){
            // try to write the changes into map
            foreach(Cell obstacleEdgeCell in curObstacleEdgeCells){
                // todo: check if cell exist + is state valid(?)
                if (obstacleEdgeCell.cellState != CellState.Walkable) continue; // just in case
                obstacleEdgeCell.cellState = CellState.Unwalkable;
            }
            // check if the changes is acceptable, revert changes if no
            if (!this.IsMapFullyAccessible()){
                foreach(Cell obstacleEdgeCell in curObstacleEdgeCells){
                    if (obstacleEdgeCell.cellState != CellState.Unwalkable) continue; // just in case
                    obstacleEdgeCell.cellState = CellState.Walkable;
                }
                break;
            }
            else { // else prepare for next iteration
                prevObstacleEdgeCells.AddRange(curObstacleEdgeCells);
                curObstacleEdgeCells.Clear();
                foreach (Cell prevObstacleEdgeCell in prevObstacleEdgeCells){
                    foreach (Cell neighbour in this.Get8Neighbour(prevObstacleEdgeCell)){
                        if ((UnityEngine.Random.Range(0, 10) < fillExtent) && neighbour.cellState == CellState.Walkable && !this.IsBorderCell(neighbour)){
                            curObstacleEdgeCells.Add(neighbour);
                        }
                    }
                }
            }
        }

    }

    private class AStarCellCostComparer: IComparer<Cell> {
        int IComparer<Cell>.Compare(Cell cell1, Cell cell2){
            if (cell1.GCost == cell2.GCost && cell1.HCost == cell2.HCost) return 0;
            else if (cell1.FCost == cell2.FCost && cell1.HCost < cell2.HCost) return -1;
            else if (cell1.FCost < cell2.FCost) return -1;
            return 1;
        }
    }
}