using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ServerStationeryUtilityControl: ServerInteractControl{

    [SerializeField] private ClientUseControl _client;
    [SerializeField] private ServerInteractableSharedData _serverData;
    [SerializeField] private ServerInteractionManager _interactions;

    public NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate OnInfoChange { get { return this._serverData.InfoStrKey.OnValueChanged; } set { this._serverData.InfoStrKey.OnValueChanged += value; }}

    private double _useHoldStartTime;
    private double _useHoldCurrentTime => Time.fixedUnscaledTimeAsDouble;
    private double _onUsingLastUpdateTime;

    internal bool IsHoldingGrabbable => this._serverData.HoldGrabbable != null;
    internal bool IsHoldingUseButton => this._useHoldStartTime != 0;
    public InteractableCompositionSO Info => this._serverData.Info;

    void FixedUpdate(){
        if (!this._serverData.HoldGrabbable.IsUsable) return;

        if ((this.IsHoldingUseButton | !this.Info.IsHoldToUse) && this.IsHoldingGrabbable){
            double timePassed = this._useHoldCurrentTime - this._useHoldStartTime;
            double timeSinceLastUpdate = this._onUsingLastUpdateTime - this._useHoldCurrentTime;

            if (timePassed <= 0){
                // TODO: trigger event written in the GrabbableSO (e.g. fire hazard if any)
                this._interactions.ConvertServerInternal(this._serverData.HoldGrabbable, this);
            }
            else if (timeSinceLastUpdate > this.Info.OnUsingUpdateInterval){
                this._onUsingLastUpdateTime = this._useHoldCurrentTime;
                this._client.InteractionCallbackClientRpc(InteractionCallbackID.OnUsing, this._useHoldStartTime, this._useHoldCurrentTime);
            }
        }
    }
}