using Unity;

public interface IUsableSO: IInteractableSO
{
	public bool IsConverter { get; }
	public bool IsHoldToUse { get; }
	public double OnUsingUpdateInterval { get; }
}