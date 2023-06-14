using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

internal class ServerSpawnControl: ServerInteractControl{
    [SerializeField] private ClientSpawnControl _client;
    [SerializeField] private ServerInteractableSharedData _data;
    [SerializeField] private Transform _spawnningPoint = null;
    [SerializeField] private GameObject _CombinablePrefab;
    public ServerCombinable SpawnningCombinableInfo = null;

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!this.IsServer | this.SpawnningCombinableInfo == null) return;
        this._data.InfoStrKey.Value = this.SpawnningCombinableInfo.name;
        // this._info = SpawnableSO.GetSO(this.SpawnningGrabbableInfo);
        // this._info.SpawnningSO.RegisterObject(); // server does not need to register object
    }
    public override void OnNetworkDespawn(){
        base.OnNetworkDespawn();
        this.SpawnningCombinableInfo = null;
    }

    internal ServerInteractControl SpawnGrabbableServerInternal(){
        // spawn target object
        GameObject newCombinableObject = Instantiate(this._CombinablePrefab, this._spawnningPoint.position, this._spawnningPoint.rotation);
        ServerCombineControl newCombineControl = newCombinableObject.GetComponent<ServerCombineControl>();
        newCombineControl.NetworkObjectBuf.Spawn(true);
        newCombineControl.SetInfoServerInternal(this.SpawnningCombinableInfo.name);
        print("spawned");
        return newCombineControl;
    }
}