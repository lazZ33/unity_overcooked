using UnityEngine;
using Unity;
using Unity.Netcode;

public class ClientTable: ClientHolder{
    private new ServerTable _server => (ServerTable)base._server;
    private new TableSO _info => (TableSO)base._info;
    public new TableSO Info => (TableSO)base._info;    
}