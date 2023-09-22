using Unity;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cell {
	public int x { get; }
	public int y { get; }
	public CellState cellState;

	// for A* algo
	public int GCost;
	public int HCost;
	public int FCost => this.GCost + this.HCost;
	public Cell PrevPos;


	public Cell(int x, int y) {
		this.x = x;
		this.y = y;
		this.GCost = 0;
		this.HCost = 0;
	}

	public static int AbsCellDistance(Cell pos1, Cell pos2) {
		return (int)Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y);
	}
	public bool PosEquals(Cell other) {
		return this.x == other.x && this.y == other.y;
	}
	// public bool PosNotEquals(Cell other){
	//     return this.x != other.x | this.y != other.y;
	// }

}