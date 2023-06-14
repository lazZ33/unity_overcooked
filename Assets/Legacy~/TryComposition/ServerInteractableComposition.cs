using UnityEngine;
using Unity.Netcode;

internal class ServerInteractableComposition: NetworkBehaviour{
    [SerializeField] internal ServerGrabDropControl grabDropControl;
    [SerializeField] internal ServerCombineControl combineControl;
    [SerializeField] internal ServerSpawnControl spawnControl;
    [SerializeField] internal ServerUseControl useControl;
    [SerializeField] private ServerInteractableSharedData _serverData;
    
    internal InteractableCompositionSO Info => this._serverData.Info;
    internal bool IsGrabbable => this.grabDropControl != null;
    internal bool IsCombinable => this.combineControl != null;
    internal bool IsSpawnable => this.spawnControl != null;
    internal bool IsUsable => this.useControl != null;
}