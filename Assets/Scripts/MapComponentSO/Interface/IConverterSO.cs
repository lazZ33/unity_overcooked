using Unity;

public interface IConverterSO: IInteractableSO {
	public bool IsHoldToConvert { get; }
	public bool IsConvertToVoid { get; }
	public double OnConvertUpdateInterval { get; }
	public double ConvertDuration { get; }
}