using Unity;
using UnityEngine;
using System;

public interface IServerCombinable: IServerGrabbable {
	public new ICombinableSO Info { get; }

	public bool CanCombineWith(IServerCombinable targetCombineControl) => this.Info.CanCombineWith(targetCombineControl.Info);

	internal void OnCombineServerInternal();
}