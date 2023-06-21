using UnityEngine;
using Unity;
using Unity.Netcode;

public class ServerTable: ServerHolder{
    private new ClientTable _client => (ClientTable)base._client;
    private new TableSO _info { get { return (TableSO)base._info; } set { base._info = value; } }
    public new TableSO Info => (TableSO)base._info;


}