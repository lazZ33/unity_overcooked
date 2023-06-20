using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ClientDishExit: ClientHolder{

    private new ServerDishExit _server => (ServerDishExit)base._server;
    public new DishExitSO Info { get { return (DishExitSO)base._info; } set { base._info = value; } }

}