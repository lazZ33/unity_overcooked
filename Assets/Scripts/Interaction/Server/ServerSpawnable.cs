using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ServerSpawnable : ServerInteractable {

    [SerializeField] private GameObject _CombinablePrefab;

    [NonSerialized] public CombinableSO SpawnningCombinableInfo = null;

    private new ClientSpawnable _client => (ClientSpawnable)base._client;
    private new SpawnableSO _info { get { return (SpawnableSO)base._info; } set { base._info = value; } }
    public new SpawnableSO Info => this._info;
    internal ServerCombinable SpawnCombinableServerInternal(){
        print("SpawnCombinableServerInternal");

        GameObject newCombinableObject = Instantiate(this._CombinablePrefab, this.transform.TransformPoint(this.Info.LocalSpawnPoint), Quaternion.identity);
        ServerCombinable newCombinableServer = newCombinableObject.GetComponent<ServerCombinable>();
        newCombinableServer.NetworkObjectBuf.Spawn(true);
        newCombinableServer.SetInfoServerInternal(this.SpawnningCombinableInfo.StrKey);
        print("spawned");
        return newCombinableServer;
    }

}