using Unity;
using UnityEngine;
using System;

public interface IHolderSO: IInteractableSO
{
	public Vector3 LocalPlacePosition { get; }
	public Quaternion LocalPlaceRotation { get; }
	public IHolderSO BindingHolder { get; }
}