using UnityEngine;
using Unity;
using Unity.Netcode;

public class ServerTable: ServerHolder{
    private new ClientTable _client => (ClientTable)base._client;
    public new TableSO Info { get { return (TableSO)base._info; } set { base._info = value; } }    
 
}