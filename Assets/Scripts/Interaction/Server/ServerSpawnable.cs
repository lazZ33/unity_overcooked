using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ServerSpawnable : ServerInteractable {
    // TODO: dynamically change _info according to generated map config, sync client's infoStrKey accordingly
    public static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";
    public NetworkVariable<FixedString128Bytes> InfoStrKey = new NetworkVariable<FixedString128Bytes>(ServerSpawnable.INFO_STR_KEY_DEFAULT, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private SpawnableSO _info;
    [SerializeField] private Transform _spawnningPoint = null;
    [SerializeField] private ClientSpawnable _client;
    [SerializeField] private GameObject _ingredientPrefab;

    protected override void Awake(){
        base.Awake();
        this.InfoStrKey.Value = this._info.name;
        this._info.SpawnningSO.RegisterObject();
    }
    public override void OnNetworkSpawn(){
        if (!this.IsServer) this.enabled = false;
        this._info = (SpawnableSO) Resources.Load("InteractableSO/Spawnable" + InfoStrKey.Value);
    }

    internal ServerGrabbable SpawnGrabbableServerInternal(){
        // spawn target object
        GameObject newGrabbableObject = Instantiate(this._ingredientPrefab, this._spawnningPoint.position, this._spawnningPoint.rotation);
        ServerGrabbable newGrabbable = newGrabbableObject.GetComponent<ServerGrabbable>();
        newGrabbable.NetworkObjectBuf.Spawn(true);
        newGrabbable.SetInfoServerRpc(new FixedString128Bytes(this._info.SpawnningSO.name));
        print("spawned");
        return newGrabbable;
    }

}