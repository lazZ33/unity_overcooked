using Unity;
using UnityEngine;
using System;
using UnityEngine.WSA;

public class ServerInteractionManager
{
	// Singleton
	private static ServerInteractionManager _instance;
	public static ServerInteractionManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = new ServerInteractionManager();
			return _instance;
		}
	}

	public void GrabServerInternal(IServerHolder targetHolder, IServerGrabbable targetGrabbable)
	{
		if (targetHolder.IsHoldingGrabbable) { Debug.LogError("Attempt to grab when holding grabbable."); return; }
		if (targetGrabbable.IsGrabbedByPlayer) { Debug.LogError("Attempt to grab holded grabbable."); return; }

		targetGrabbable.OnDropServerInternal();
		targetHolder.OnHoldServerInternal(targetGrabbable);
	}

	public void DropServerInternal(IServerHolder targetHolder)
	{
		if (!targetHolder.IsHoldingGrabbable) { Debug.LogError("Attempt to drop when not holding grabbable."); return; }

		targetHolder.OnTakeServerInternal(out IServerGrabbable droppedGrabbable);
		droppedGrabbable.OnDropServerInternal();
	}

	public void TransferServerInternal(IServerHolder toHolder, IServerHolder fromHolder)
	{
		if (!fromHolder.IsHoldingGrabbable) { Debug.LogError("Attempt to place when not holding grabbable."); return; }

		fromHolder.OnTakeServerInternal(out IServerGrabbable placedGrabbable);
		placedGrabbable.OnDropServerInternal();
		placedGrabbable.OnGrabServerInternal(toHolder);
		toHolder.OnHoldServerInternal(placedGrabbable);
	}

	public void CombineServerInternal(IServerCombinable retainedCombinable, IServerCombinable removedCombinable)
	{
		if (!retainedCombinable.CanCombineWith(removedCombinable) && !removedCombinable.CanCombineWith(retainedCombinable)) { 
			Debug.LogError("Attempt to combine invalid combination.");
			return;
		}
		if (!retainedCombinable.CanCombineWith(removedCombinable) ^ !removedCombinable.CanCombineWith(retainedCombinable)) {
			Debug.LogError(String.Format("Found one-way combination of combinables, please fix. 1: {0}, 2: {1}", retainedCombinable, removedCombinable));
			return;
		}

		//removedCombinable.OnCombineServerInternal(); // is this needed?
		retainedCombinable.SetInfoServerInternal(ICombinableSO.GetNextSO(retainedCombinable.Info, removedCombinable.Info));
		removedCombinable.NetworkObjectBuf.Despawn();
		retainedCombinable.OnCombineServerInternal(); // invoke callbacks
	}

	public void ConvertServerInternal(IServerCombinable targetCombinable, IServerUsable converter)
	{
		if (!converter.Info.IsConverter) { Debug.LogError("Attempt to use ConvertServerInternal with a non-converter IUsable."); return;}

		ICombinableSO newCombinableSO = ICombinableSO.TryGetNextSO(targetCombinable.Info, converter.Info);

		if (newCombinableSO == null) {
			Debug.LogError(String.Format("Convertion of given targetCombinable with given converter does not exist. target:{0}, converter: {1}", targetCombinable, converter));
			return;
		}

		targetCombinable.SetInfoServerInternal(newCombinableSO);
	}

	public void UseServerInternal(IServerUsable targetUsable)
	{
		targetUsable.OnUseServerInternal();
	}

	public void UnuseServerInternal(IServerUsable targetUsable)
	{
		targetUsable.OnUnuseServerInternal();
	}
}