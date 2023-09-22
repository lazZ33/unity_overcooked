using Unity;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "ServerInteractionManager", menuName = "ScriptableObject/ServerInteractionManager")]
public class ServerInteractionManager: ScriptableObject {
	public void GrabServerInternal(IServerHolder targetHolder, IServerGrabbable targetGrabbable) {
		if (targetHolder.IsHoldingGrabbable) { Debug.LogError("Attempt to grab when holding grabbable."); return; }
		if (targetGrabbable.IsGrabbedByPlayer) { Debug.LogError("Attempt to grab holded grabbable."); return; }

		targetGrabbable.OnDropServerInternal();
		targetHolder.OnHoldServerInternal(targetGrabbable);
	}

	public void DropServerInternal(IServerHolder targetHolder) {
		if (!targetHolder.IsHoldingGrabbable) { Debug.LogError("Attempt to drop when not holding grabbable."); return; }

		targetHolder.OnTakeServerInternal(out IServerGrabbable droppedGrabbable);
		droppedGrabbable.OnDropServerInternal();
	}

	public void SpawnAndGrabServerInternal(IServerSpawner targetSpawner, IServerHolder targetHolder) {
		if (targetHolder.IsHoldingGrabbable) { Debug.LogError("Attempt to spawn and grab when holding grabbable."); return; }

		IServerGrabbable newGrabbable = targetSpawner.OnSpawnServerInternal();
		targetHolder.OnHoldServerInternal(newGrabbable);
		newGrabbable.OnGrabServerInternal(targetHolder);
	}

	public void TransferServerInternal(IServerHolder targetHolder, IServerHolder sourceHolder) {
		if (!sourceHolder.IsHoldingGrabbable) { Debug.Log("sourceHolder not holding grabbable."); return; }

		sourceHolder.OnTakeServerInternal(out IServerGrabbable placedGrabbable);
		placedGrabbable.OnDropServerInternal();
		placedGrabbable.OnGrabServerInternal(targetHolder);
		targetHolder.OnHoldServerInternal(placedGrabbable);



		IServerConverter targetConverter = targetHolder as IServerConverter;
		IServerConverter sourceConverter = sourceHolder as IServerConverter;
		if (targetConverter != null) {
			if (!targetConverter.IsHoldToConvert)
				targetConverter.OnConvertStartServerInternal();
		}
		if (sourceConverter != null) {
			if (!sourceConverter.IsHoldToConvert)
				sourceConverter.OnConvertEndServerInternal();
		}
	}

	//public void CombineServerInternal(IServerCombinable retainedCombinable, IServerCombinable removedCombinable)
	//{
	//	if (!retainedCombinable.CanCombineWith(removedCombinable) && !removedCombinable.CanCombineWith(retainedCombinable)) { 
	//		Debug.LogError("Attempt to combine invalid combination.");
	//		return;
	//	}
	//	if (!retainedCombinable.CanCombineWith(removedCombinable) ^ !removedCombinable.CanCombineWith(retainedCombinable)) {
	//		Debug.LogError(String.Format("Found one-way combination of combinables, please fix. 1: {0}, 2: {1}", retainedCombinable, removedCombinable));
	//		return;
	//	}

	//	//removedCombinable.OnCombineServerInternal(); // is this needed?
	//	retainedCombinable.SetInfoServerInternal(ICombinableSO.GetNextSO(retainedCombinable.Info, removedCombinable.Info));
	//	removedCombinable.NetworkObjectBuf.Despawn();
	//	retainedCombinable.OnCombineServerInternal(); // invoke callbacks
	//}

	public void CombineOnHolderServerInternal(IServerCombinable targetCombinable, IServerHolder targetHolder) {
		IServerCombinable removedCombinable = targetHolder.HoldGrabbable as IServerCombinable;
		IServerCombinable retainedCombinable = targetCombinable;
		if (removedCombinable == null) {
			Debug.LogError("Attempt to combineOnHolder when targetHolder not holding a combinable.");
			return;
		}

		if (!retainedCombinable.CanCombineWith(removedCombinable) && !removedCombinable.CanCombineWith(retainedCombinable)) {
			Debug.LogError("Attempt to combine invalid combination.");
			return;
		}
		if (!retainedCombinable.CanCombineWith(removedCombinable) ^ !removedCombinable.CanCombineWith(retainedCombinable)) {
			Debug.LogError(String.Format("Found one-way combination of combinables, please fix. 1: {0}, 2: {1}", retainedCombinable, removedCombinable));
			return;
		}

		targetHolder.OnTakeServerInternal(out IServerGrabbable removedGrabbable);
		//removedCombinable.OnCombineServerInternal(); // is this needed?
		retainedCombinable.SetInfoServerInternal(ICombinableSO.GetNextSO(retainedCombinable.Info, removedCombinable.Info));
		removedCombinable.NetworkObjectBuf.Despawn();
		retainedCombinable.OnCombineServerInternal(); // invoke callbacks
	}
	public void CombineOnHolderServerInternal(IServerHolder retainedTargetHolder, IServerHolder removedTargetHolder) {
		IServerCombinable retainedCombinable = retainedTargetHolder.HoldGrabbable as IServerCombinable;
		IServerCombinable removedCombinable = removedTargetHolder.HoldGrabbable as IServerCombinable;
		if (retainedCombinable == null || removedCombinable == null) {
			Debug.LogError("Attempt to combineOnHolder when one of the targetHolder not holding a combinable.");
			return;
		}

		if (!retainedCombinable.CanCombineWith(removedCombinable) && !removedCombinable.CanCombineWith(retainedCombinable)) {
			Debug.LogError("Attempt to combine invalid combination.");
			return;
		}
		if (!retainedCombinable.CanCombineWith(removedCombinable) ^ !removedCombinable.CanCombineWith(retainedCombinable)) {
			Debug.LogError(String.Format("Found one-way combination of combinables, please fix. 1: {0}, 2: {1}", retainedCombinable, removedCombinable));
			return;
		}

		removedTargetHolder.OnTakeServerInternal(out IServerGrabbable removedGrabbable);
		//removedCombinable.OnCombineServerInternal(); // is this needed?
		retainedCombinable.SetInfoServerInternal(ICombinableSO.GetNextSO(retainedCombinable.Info, removedCombinable.Info));
		removedCombinable.NetworkObjectBuf.Despawn();
		retainedCombinable.OnCombineServerInternal(); // invoke callbacks
	}

	public void ConvertServerInternal(IServerCombinable targetCombinable, IServerConverter converter) {
		ICombinableSO newCombinableSO = ICombinableSO.TryGetNextSO(targetCombinable.Info, converter.Info);

		if (newCombinableSO == null) {
			Debug.LogError(String.Format("Convertion of given targetCombinable with given converter does not exist. target:{0}, converter: {1}", targetCombinable, converter));
			return;
		}

		targetCombinable.SetInfoServerInternal(newCombinableSO);
		converter.OnConvertEndServerInternal(); // TODO: likely not always the case, e.g. soup
	}

	public void ConvertToVoidServerInternal(IServerCombinable targetCombinable, IServerConverter converter) {
		IServerHolder holder = converter as IServerHolder;
		if (holder != null) {
			holder.OnTakeServerInternal(out IServerGrabbable targetGrabbable);
		}

		targetCombinable.NetworkObjectBuf.Despawn();
		converter.OnConvertEndServerInternal();
	}


	public void UseServerInternal(IServerConverter targetUsable) {
		targetUsable.OnConvertStartServerInternal();
	}

	public void UnuseServerInternal(IServerConverter targetUsable) {
		targetUsable.OnConvertEndServerInternal();
	}
}