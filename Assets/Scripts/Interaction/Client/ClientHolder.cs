using UnityEngine;
using Unity;
using Unity.Netcode;
using Unity.Collections;

public abstract class ClientHolder: ClientInteractable{
    private new ServerHolder _server => (ServerHolder)base._server;
    private new HolderSO _info => (HolderSO)base._info;
    public new HolderSO Info => (HolderSO)base._info;
}