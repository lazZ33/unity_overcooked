using System;

public class SpawnEventArgs : EventArgs
{
	internal SpawnEventArgs(IGrabbableSO spawnedGrabbableInfo) { this.SpawnedGrabbableInfo = spawnedGrabbableInfo; }
	public IGrabbableSO SpawnedGrabbableInfo;
}