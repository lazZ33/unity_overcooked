using Unity;
using UnityEngine;

public class GameFloorSO: ScriptableObject {
	[SerializeField] private Mesh _mesh;
	public Mesh Mesh => this._mesh;
	[SerializeField] private Material _material;
	public Material Material => this._material;
}