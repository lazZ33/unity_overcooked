using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ServerSpawnable : ServerInteractable {

    [SerializeField] private GameObject _CombinablePrefab;

    [NonSerialized] public CombinableSO SpawnningCombinableInfo = null;

    private new ClientSpawnable _client => (ClientSpawnable)base._client;
    public new SpawnableSO Info { get { return (SpawnableSO)base._info; } set { base._info = value; } }    


    internal ServerCombinable SpawnCombinableServerInternal(){
        // spawn target object
        GameObject newCombinableObject = Instantiate(this._CombinablePrefab, this.transform.TransformPoint(this.Info.LocalSpawnPoint), Quaternion.identity);
        ServerCombinable newCombinableServer = newCombinableObject.GetComponent<ServerCombinable>();
        newCombinableServer.NetworkObjectBuf.Spawn(true);
        newCombinableServer.SetInfoServerInternal(this.SpawnningCombinableInfo.StrKey);
        print("spawned");
        return newCombinableServer;
    }

}