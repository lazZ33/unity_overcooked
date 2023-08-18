

using System.Collections.Generic;

public class ToolSO : InteractableSO, IGrabbableSO, IUsableSO
{
	// IGrabbableSO inplementation
	HashSet<IHolderSO> IGrabbableSO._existingPlaceableTo => throw new System.NotImplementedException();


	// IUsableSO implementation
	bool IUsableSO.IsConverter => throw new System.NotImplementedException();
	bool IUsableSO.IsHoldToUse => throw new System.NotImplementedException();
	double IUsableSO.OnUsingUpdateInterval => throw new System.NotImplementedException();


	public static new ToolSO GetSO(string strKey) => (ToolSO)InteractableSO.GetSO(strKey);
	public static new ToolSO TryGetSO(string strKey) => (ToolSO)InteractableSO.TryGetSO(strKey);
}