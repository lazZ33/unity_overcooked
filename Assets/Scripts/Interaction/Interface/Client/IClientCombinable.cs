using Unity;
using UnityEngine;
using System;

public interface IClientCombinable: IClientInteractable {
	public new ICombinableSO Info { get; }

	public bool CanCombineWith(IServerCombinable targetCombineControl) => this.Info.CanCombineWith(targetCombineControl.Info);
}