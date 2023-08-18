using System.Collections.Generic;
using Unity;

public interface IGrabbableSO : IInteractableSO
{
	protected HashSet<IHolderSO> _existingPlaceableTo { get; }

	public bool CanPlaceOn(IHolderSO holder) => this._existingPlaceableTo.Contains(holder);
}